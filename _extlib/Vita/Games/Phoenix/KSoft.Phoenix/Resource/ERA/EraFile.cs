using KSoft.IO;
using KSoft.Shell;
using System;
using System.Collections.Generic;
using System.IO;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

namespace KSoft.Phoenix.Resource
{
    public sealed class EraFile
        : IEndianStreamSerializable
        , IDisposable
    {
        public const string kExtensionEncrypted = ".era";
        public const string kExtensionDecrypted = ".bin";
        private const int kAlignmentBit = IntegerMath.kFourKiloAlignmentBit;
        private const string kFileNamesTableName = "_file_names.bin";
        private static readonly Memory.Strings.StringMemoryPoolSettings kFileNamesTablePoolConfig = new Memory.Strings.
            StringMemoryPoolSettings(Memory.Strings.StringStorage.CStringAscii, false);

        public string FileName { get; set; }

        private readonly EraFileHeader mHeader = new EraFileHeader();
        private readonly List<EraFileEntryChunk> mFiles = new List<EraFileEntryChunk>();
        private readonly Dictionary<string, string> mLocalFiles = new Dictionary<string, string>();
        private readonly Dictionary<string, EraFileEntryChunk> mFileNameToChunk = new Dictionary<string, EraFileEntryChunk>();

        public DateTime BuildModeDefaultTimestamp { get; set; }
            = DateTime.Now;

        public Security.Cryptography.TigerHashBase TigerHasher { get; private set; }

        private int FileChunksFirstIndex =>
                // First comes the filenames table in mFiles, then all the files defined in the listing
                1;
        /// <summary>All files destined for the ERA, excluding the internal filenames table</summary>
        //private IEnumerable<EraFileEntryChunk> FileChunks
        //{
        //	get
        //	{
        //		// Skip the first chunk, as it is the filenames table
        //		return Enumerable.Skip(mFiles, FileChunksFirstIndex);
        //	}
        //}
        /// <summary>Number of files destined for the ERA, excluding the internal filenames table</summary>
        private int FileChunksCount =>
                // Exclude the first chunk from the count, as it is the filenames table
                mFiles.Count - FileChunksFirstIndex;

        public EraFile()
        {
            TigerHasher = Security.Cryptography.PhxHash.CreateHaloWarsTigerHash();
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (TigerHasher != null)
            {
                TigerHasher.Dispose();
                TigerHasher = null;
            }
        }
        #endregion

        public int CalculateHeaderAndFileChunksSize()
        {
            return
                EraFileHeader.CalculateHeaderSize() +
                EraFileEntryChunk.CalculateFileChunksSize(mFiles.Count);
        }

        private void ValidateAdler32(EraFileEntryChunk fileEntry, EndianStream blockStream)
        {
            uint actual_adler = fileEntry.ComputeAdler32(blockStream);

            if (actual_adler != fileEntry.Adler32)
            {
                string chunk_name = !string.IsNullOrEmpty(fileEntry.FileName)
                    ? fileEntry.FileName
                    : "FileNames";//fileEntry.EntryId.ToString("X16");

                throw new InvalidDataException(string.Format(
                    "Invalid chunk adler32 for '{0}' offset={1} size={2} " +
                    "expected {3} but got {4}",
                    chunk_name, fileEntry.DataOffset, fileEntry.DataSize.ToString("X8"),
                    fileEntry.Adler32.ToString("X8"),
                    actual_adler.ToString("X8")
                    ));
            }
        }

        private void ValidateHashes(EraFileEntryChunk fileEntry, EndianStream blockStream)
        {
            fileEntry.ComputeHash(blockStream, TigerHasher);

            if (!fileEntry.CompressedDataTiger128.EqualsArray(TigerHasher.Hash))
            {
                string chunk_name = !string.IsNullOrEmpty(fileEntry.FileName)
                    ? fileEntry.FileName
                    : "FileNames";//fileEntry.EntryId.ToString("X16");

                throw new System.IO.InvalidDataException(string.Format(
                    "Invalid chunk hash for '{0}' offset={1} size={2} " +
                    "expected {3} but got {4}",
                    chunk_name, fileEntry.DataOffset, fileEntry.DataSize.ToString("X8"),
                    Text.Util.ByteArrayToString(fileEntry.CompressedDataTiger128),
                    Text.Util.ByteArrayToString(TigerHasher.Hash, 0, EraFileEntryChunk.kCompresssedDataTigerHashSize)
                    ));
            }

            if (fileEntry.CompressionType == ECF.EcfCompressionType.Stored)
            {
                _ = TigerHasher.TryGetAsTiger64(out ulong tiger64);

                if (fileEntry.DecompressedDataTiger64 != tiger64)
                {
                    string chunk_name = !string.IsNullOrEmpty(fileEntry.FileName)
                        ? fileEntry.FileName
                        : "FileNames";//fileEntry.EntryId.ToString("X16");

                    throw new System.IO.InvalidDataException(string.Format(
                        "Chunk id mismatch for '{0}' offset={1} size={2} " +
                        "expected {3} but got {4}",
                        chunk_name, fileEntry.DataOffset, fileEntry.DataSize.ToString("X8"),
                        fileEntry.DecompressedDataTiger64.ToString("X16"),
                        Text.Util.ByteArrayToString(TigerHasher.Hash, 0, sizeof(ulong))
                        ));
                }
            }
        }

        private int FileIndexToListingIndex(int fileIndex)
        {
            return fileIndex - 1;
        }

        private void BuildFileNameMaps(System.IO.TextWriter verboseOutput)
        {
            for (int x = FileChunksFirstIndex; x < mFiles.Count;)
            {
                EraFileEntryChunk file = mFiles[x];
                if (mFileNameToChunk.TryGetValue(file.FileName, out _))
                {
                    verboseOutput?.WriteLine("Removing duplicate {0} entry at #{1}",
                            file.FileName, FileIndexToListingIndex(x));
                    mFiles.RemoveAt(x);
                    continue;
                }

                mFileNameToChunk.Add(file.FileName, file);
                x++;
            }
        }

        private void RemoveXmbFilesWhereXmlExists(TextWriter verboseOutput)
        {
            for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
            {
                EraFileEntryChunk file = mFiles[x];
                if (!ResourceUtils.IsXmbFile(file.FileName))
                {
                    continue;
                }

                string xml_name = file.FileName;
                ResourceUtils.RemoveXmbExtension(ref xml_name);
                if (!mFileNameToChunk.TryGetValue(xml_name, out EraFileEntryChunk xml_file))
                {
                    continue;
                }

                verboseOutput?.WriteLine("\tRemoving XMB file #{0} '{1}' from listing since its XML already exists {2}",
                        FileIndexToListingIndex(x),
                        file.FileName,
                        xml_file.FileName);

                mFiles.RemoveAt(x);
                x--;
            }
        }

        private void RemoveXmlFilesWhereXmbExists(System.IO.TextWriter verboseOutput)
        {
            for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
            {
                EraFileEntryChunk file = mFiles[x];
                if (!ResourceUtils.IsXmlBasedFile(file.FileName))
                {
                    continue;
                }

                string xmb_name = file.FileName;
                xmb_name += Xmb.XmbFile.kFileExt;
                if (!mFileNameToChunk.TryGetValue(xmb_name, out EraFileEntryChunk xmb_file))
                {
                    continue;
                }

                verboseOutput?.WriteLine("\tRemoving XML file #{0} '{1}' from listing since its XMB already exists {2}",
                        FileIndexToListingIndex(x),
                        file.FileName,
                        xmb_file.FileName);

                mFiles.RemoveAt(x);
                x--;
            }
        }

        public void TryToReferenceXmlOverXmbFies(string workPath, TextWriter verboseOutput)
        {
            for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
            {
                EraFileEntryChunk file = mFiles[x];
                if (!ResourceUtils.IsXmbFile(file.FileName))
                {
                    continue;
                }

                string xml_name = file.FileName;
                ResourceUtils.RemoveXmbExtension(ref xml_name);

                // if the user already references the XML file too, just skip doing anything
                if (mFileNameToChunk.TryGetValue(xml_name, out _))
                {
                    continue;
                }

                // does the XML file exist?
                string xml_path = Path.Combine(workPath, xml_name);
                if (!File.Exists(xml_path))
                {
                    continue;
                }

                verboseOutput?.WriteLine("\tReplacing XMB ref with {0}",
                        xml_name);

                // right now, all we should need to do to update things is remove the XMB mapping and replace it with the XML we found
                bool removed = mFileNameToChunk.Remove(file.FileName);
                file.FileName = xml_name;
                if (removed)
                {
                    mFileNameToChunk.Add(xml_name, file);
                }
            }
        }

        #region Xml definition Streaming
        private static EraFileEntryChunk GenerateFileNamesTableEntryChunk()
        {
            EraFileEntryChunk chunk = new EraFileEntryChunk
            {
                CompressionType = ECF.EcfCompressionType.DeflateStream
            };

            return chunk;
        }
        private void ReadChunks(XmlElementStream s)
        {
            foreach (System.Xml.XmlElement n in s.ElementsByName(ECF.EcfChunk.kXmlElementStreamName))
            {
                EraFileEntryChunk f = new EraFileEntryChunk();
                using (s.EnterCursorBookmark(n))
                {
                    f.Read(s, false);
                }

                mFiles.Add(f);
            }
        }
        private void ReadLocalFiles(XmlElementStream s)
        {
            foreach (System.Xml.XmlElement n in s.ElementsByName("file"))
            {
                using (s.EnterCursorBookmark(n))
                {
                    string file_name = null;
                    s.ReadAttribute("name", ref file_name);

                    string file_data = "";
                    s.ReadCursor(ref file_data);

                    if (!string.IsNullOrEmpty(file_name))
                    {
                        mLocalFiles[file_name] = file_data;
                    }
                }
            }
        }

        private void WriteChunks(XmlElementStream s)
        {
            for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
            {
                mFiles[x].Write(s, false);
            }
        }
        private void WriteLocalFiles(XmlElementStream s)
        {
            foreach (KeyValuePair<string, string> kvp in mLocalFiles)
            {
                string file_name = kvp.Key;
                string file_data = kvp.Value;

                using (s.EnterCursorBookmark("file"))
                {
                    s.WriteAttribute("name", file_name);
                    s.WriteCursor(file_data);
                }
            }
        }

        public bool ReadDefinition(XmlElementStream s)
        {
            mFiles.Clear();

            // first entry should always be the null terminated filenames table
            mFiles.Add(GenerateFileNamesTableEntryChunk());

            using (s.EnterCursorBookmark("Files"))
            {
                ReadChunks(s);
            }

            using (TagElementStreamBookmark<System.Xml.XmlDocument, System.Xml.XmlElement, string> bm = s.EnterCursorBookmarkOpt("LocalFiles"))
            {
                if (bm.IsNotNull)
                {
                    ReadLocalFiles(s);
                }
            }

            AddVersionFile();

            // there should be at least one file destined for the ERA, excluding the filenames table
            return FileChunksCount != 0;
        }
        public void WriteDefinition(XmlElementStream s)
        {
            using (s.EnterCursorBookmark("Files"))
            {
                WriteChunks(s);
            }

            using (TagElementStreamBookmark<System.Xml.XmlDocument, System.Xml.XmlElement, string> bm = s.EnterCursorBookmarkOpt("LocalFiles", mLocalFiles, Predicates.HasItems))
            {
                if (bm.IsNotNull)
                {
                    WriteLocalFiles(s);
                }
            }
        }
        #endregion

        #region Expand
        public void ExpandTo(EndianStream blockStream, string workPath)
        {
            Contract.Requires(blockStream.IsReading);

            EraFileExpander eraExpander = (EraFileExpander)blockStream.Owner;

            eraExpander.ProgressOutput?.WriteLine("\tUnpacking files...");

            for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
            {
                EraFileEntryChunk file = mFiles[x];

                eraExpander.ProgressOutput?.Write("\r\t\t{0} ", file.EntryId.ToString("X16"));
                _ = TryUnpack(blockStream, workPath, eraExpander, file);
            }

            if (eraExpander.ProgressOutput != null)
            {
                eraExpander.ProgressOutput.Write("\r\t\t{0} \r", new string(' ', 16));
                eraExpander.ProgressOutput.WriteLine("\t\tDone");
            }

            mDirsThatExistForUnpacking = null;
        }

        private bool TryUnpack(EndianStream blockStream, string workPath, EraFileExpander expander, EraFileEntryChunk file)
        {
            if (IsIgnoredLocalFile(file.FileName))
            {
                return false;
            }

            string full_path = Path.Combine(workPath, file.FileName);

            if (ResourceUtils.IsLocalScenarioFile(file.FileName))
            {
                return false;
            }
            else if (!ShouldUnpack(expander, full_path))
            {
                return false;
            }

            CreatePathForUnpacking(full_path);

            UnpackToDisk(blockStream, full_path, expander, file);
            return true;
        }

        private void UnpackToDisk(EndianStream blockStream, string fullPath, EraFileExpander expander, EraFileEntryChunk file)
        {
            byte[] buffer = file.GetBuffer(blockStream);

            if (expander.ExpanderOptions.Test(EraFileExpanderOptions.ExpandAsDds) && Path.GetExtension(fullPath).ToLowerInvariant() == ".ddx")
            {
                fullPath = Path.ChangeExtension(fullPath, ".dds");
            }

            if (ResourceUtils.IsXmbFile(fullPath))
            {
                if (!expander.ExpanderOptions.Test(EraFileExpanderOptions.DontTranslateXmbFiles))
                {
                    ProcessorSize va_size = ProcessorSize.x32;
                    bool builtFor64Bit = expander.Options.Test(EraFileUtilOptions.x64);
                    if (builtFor64Bit)
                    {
                        va_size = ProcessorSize.x64;
                    }

                    TransformXmbToXml(buffer, fullPath, blockStream.ByteOrder, va_size);
                }
            }

            using (FileStream fs = File.Create(fullPath))
            {
                fs.Write(buffer, 0, buffer.Length);
            }

            File.SetCreationTimeUtc(fullPath, file.FileDateTime);
            File.SetLastWriteTimeUtc(fullPath, file.FileDateTime);


            if (ResourceUtils.IsScaleformFile(fullPath))
            {
                if (expander.ExpanderOptions.Test(EraFileExpanderOptions.DecompressUIFiles))
                {
                    bool success;
                    try
                    {
                        success = DecompressUIFileToDisk(buffer, fullPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.Trace.Resource.TraceEvent(System.Diagnostics.TraceEventType.Error, TypeExtensions.kNone,
                            "Exception during {0} of '{1}': {2}",
                            EraFileExpanderOptions.DecompressUIFiles, fullPath, ex);
                        success = false;
                    }

                    if (!success && expander.VerboseOutput != null)
                    {
                        expander.VerboseOutput.WriteLine("Option {0} failed on '{1}'",
                            EraFileExpanderOptions.DecompressUIFiles, fullPath);
                    }
                }
                if (expander.ExpanderOptions.Test(EraFileExpanderOptions.TranslateGfxFiles))
                {
                    TransformGfxToSwfFileResult result = TransformGfxToSwfFileResult.Failed;

                    try
                    {
                        result = TransformGfxToSwfFile(buffer, fullPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.Trace.Resource.TraceEvent(System.Diagnostics.TraceEventType.Error, TypeExtensions.kNone,
                            "Exception during {0} of '{1}': {2}",
                            EraFileExpanderOptions.TranslateGfxFiles, fullPath, ex);
                    }

                    if (expander.VerboseOutput != null)
                    {
                        if (result == TransformGfxToSwfFileResult.Failed)
                        {
                            expander.VerboseOutput.WriteLine("Option {0} failed on '{1}'",
                                EraFileExpanderOptions.TranslateGfxFiles, fullPath);
                        }
                        else if (result == TransformGfxToSwfFileResult.InputIsAlreadySwf)
                        {
                            expander.VerboseOutput.WriteLine("Option {0} skipped on '{1}', it is already an SWF-based file",
                                EraFileExpanderOptions.TranslateGfxFiles, fullPath);
                        }
                    }
                }
            }

            /// <summary>
            /// With how the era is processed we can't ignore out xmb files, but we can delete them directly after they're processed.
            /// </summary>
            if (expander.ExpanderOptions.Test(EraFileExpanderOptions.RemoveXmb))
            {
                if (Path.GetExtension(fullPath) == ".xmb")
                {
                    File.Delete(fullPath);
                }
            }
        }

        private void TransformXmbToXml(byte[] eraFileEntryBuffer, string fullPath, EndianFormat byteOrder, ProcessorSize vaSize)
        {
            byte[] xmb_buffer;

            using (ECF.EcfFileXmb xmb = new ECF.EcfFileXmb())
            using (MemoryStream ms = new MemoryStream(eraFileEntryBuffer))
            using (EndianStream es = new EndianStream(ms, byteOrder, permissions: System.IO.FileAccess.Read))
            {
                es.StreamMode = FileAccess.Read;
                xmb.Serialize(es);

                xmb_buffer = xmb.FileData;
            }

            string xmb_path = fullPath;
            ResourceUtils.RemoveXmbExtension(ref xmb_path);

            Xmb.XmbFileContext context = new Xmb.XmbFileContext()
            {
                PointerSize = vaSize,
            };

            using (MemoryStream ms = new MemoryStream(xmb_buffer, false))
            using (EndianReader s = new EndianReader(ms, byteOrder))
            {
                s.UserData = context;

                using (Xmb.XmbFile xmbf = new Xmb.XmbFile())
                {
                    xmbf.Read(s);
                    xmbf.ToXml(xmb_path);
                }
            }
        }

        private bool DecompressUIFileToDisk(byte[] eraFileEntryBuffer, string fullPath)
        {
            bool success = false;

            using (MemoryStream ms = new System.IO.MemoryStream(eraFileEntryBuffer, false))
            using (EndianReader s = new EndianReader(ms, Shell.EndianFormat.Little))
            {
                if (ResourceUtils.IsScaleformBuffer(s, out uint buffer_signature))
                {
                    int decompressed_size = s.ReadInt32();
                    int compressed_size = (int)(ms.Length - ms.Position);

                    byte[] decompressed_data = ResourceUtils.DecompressScaleform(eraFileEntryBuffer, decompressed_size);
                    using (FileStream fs = System.IO.File.Create(fullPath + ".bin"))
                    {
                        fs.Write(decompressed_data, 0, decompressed_data.Length);
                    }

                    success = true;
                }
            }

            return success;
        }

        private enum TransformGfxToSwfFileResult
        {
            Success,
            Failed,
            InputIsAlreadySwf,
        }
        private TransformGfxToSwfFileResult TransformGfxToSwfFile(byte[] eraFileEntryBuffer, string fullPath)
        {
            TransformGfxToSwfFileResult result = TransformGfxToSwfFileResult.Failed;

            using (MemoryStream ms = new System.IO.MemoryStream(eraFileEntryBuffer, false))
            using (EndianReader s = new EndianReader(ms, Shell.EndianFormat.Little))
            {
                if (ResourceUtils.IsScaleformBuffer(s, out uint buffer_signature))
                {
                    if (ResourceUtils.IsSwfHeader(buffer_signature))
                    {
                        result = TransformGfxToSwfFileResult.InputIsAlreadySwf;
                    }
                    else
                    {
                        TransformGfxToSwfFileInternal(eraFileEntryBuffer, fullPath, buffer_signature);
                        result = TransformGfxToSwfFileResult.Success;
                    }
                }
            }

            return result;
        }
        private void TransformGfxToSwfFileInternal(byte[] eraFileEntryBuffer, string fullPath, uint bufferSignature)
        {
            uint swf_signature = ResourceUtils.GfxHeaderToSwf(bufferSignature);
            using (FileStream fs = System.IO.File.Create(fullPath + ".swf"))
            using (EndianWriter out_s = new EndianWriter(fs, Shell.EndianFormat.Little))
            {
                out_s.Write(swf_signature);
                out_s.Write(eraFileEntryBuffer, sizeof(uint), eraFileEntryBuffer.Length - sizeof(uint));
            }
        }

        private HashSet<string> mDirsThatExistForUnpacking;
        private void CreatePathForUnpacking(string full_path)
        {
            if (mDirsThatExistForUnpacking == null)
            {
                mDirsThatExistForUnpacking = new HashSet<string>();
            }

            string folder = System.IO.Path.GetDirectoryName(full_path);
            // don't bother checking the file system if we've already encountered this folder
            if (mDirsThatExistForUnpacking.Add(folder))
            {
                if (!System.IO.Directory.Exists(folder))
                {
                    _ = System.IO.Directory.CreateDirectory(folder);
                }
            }
        }

        private bool ShouldUnpack(EraFileExpander expander, string path)
        {
            if (expander.ExpanderOptions.Test(EraFileExpanderOptions.DontOverwriteExistingFiles))
            {
                // it's an XMB file and the user didn't say NOT to translate them
                if (ResourceUtils.IsXmbFile(path) && !expander.ExpanderOptions.Test(EraFileExpanderOptions.DontTranslateXmbFiles))
                {
                    ResourceUtils.RemoveXmbExtension(ref path);
                }

                if (System.IO.File.Exists(path))
                {
                    return false;
                }
            }

            if (expander.ExpanderOptions.Test(EraFileExpanderOptions.IgnoreNonDataFiles))
            {
                if (!ResourceUtils.IsDataBasedFile(path))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Build
        private bool BuildFileNamesTable(EndianStream blockStream)
        {
            Contract.Requires(blockStream.IsWriting);

            using (MemoryStream ms = new System.IO.MemoryStream(mFiles.Count * 128))
            using (EndianWriter s = new EndianWriter(ms, blockStream.ByteOrder))
            {
                Memory.Strings.StringMemoryPool smp = new Memory.Strings.StringMemoryPool(kFileNamesTablePoolConfig);
                for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
                {
                    EraFileEntryChunk file = mFiles[x];

                    file.FileNameOffset = smp.Add(file.FileName).u32;
                }
                smp.WriteStrings(s);

                EraFileEntryChunk filenames_chunk = mFiles[0];
                PackFileNames(blockStream, ms, filenames_chunk);

                return true;
            }
        }

        public bool Build(EndianStream blockStream, string workPath)
        {
            Contract.Requires(blockStream.IsWriting);

            EraFileBuilder builder = blockStream.Owner as EraFileBuilder;

            Contract.Assert(blockStream.BaseStream.Position == CalculateHeaderAndFileChunksSize());

            BuildFileNameMaps(builder?.VerboseOutput);
            bool success = BuildFileNamesTable(blockStream);
            for (int x = FileChunksFirstIndex; x < mFiles.Count && success; x++)
            {
                EraFileEntryChunk file = mFiles[x];
                if (builder != null && builder.ProgressOutput != null)
                {
                    builder.ProgressOutput.Write("\r\t\t{0} ", file.EntryId.ToString("X16"));
                }

                success &= TryPack(blockStream, workPath, file);

                if (!success &&
                    builder != null && builder.VerboseOutput != null)
                {
                    builder.VerboseOutput.WriteLine("Couldn't pack file into {0}: {1}",
                        FileName, file.FileName);
                }
            }

            if (builder != null && builder.ProgressOutput != null &&
                success)
            {
                builder.ProgressOutput.Write("\r\t\t{0} \r", new string(' ', 16));
            }

            if (success)
            {
                blockStream.AlignToBoundry(kAlignmentBit);
            }

#if false
			if (success)
			{
				if (builder != null && !builder.Options.Test(EraFileUtilOptions.SkipVerification))
				{
					var filenames_chunk = mFiles[0];

					ValidateAdler32(filenames_chunk, blockStream);
					ValidateHashes(filenames_chunk, blockStream);

					ValidateFileHashes(blockStream);
				}
			}
#endif

            return success;
        }

        private void PackFileData(EndianStream blockStream, System.IO.Stream source, EraFileEntryChunk file)
        {
            file.BuildBuffer(blockStream, source, TigerHasher);

#if false
			ValidateAdler32(file, blockStream);
			ValidateHashes(file, blockStream);
#endif
        }

        private void PackFileNames(EndianStream blockStream, System.IO.MemoryStream source, EraFileEntryChunk file)
        {
            file.FileDateTime = BuildModeDefaultTimestamp;
            PackFileData(blockStream, source, file);
        }

        private bool TryPack(EndianStream blockStream, string workPath,
            EraFileEntryChunk file)
        {
            return mLocalFiles.ContainsKey(file.FileName)
                ? TryPackLocalFile(blockStream, file)
                : TryPackFileFromDisk(blockStream, workPath, file);
        }

        private bool TryPackLocalFile(EndianStream blockStream,
            EraFileEntryChunk file)
        {
            if (!mLocalFiles.TryGetValue(file.FileName, out string file_data))
            {
                Debug.Trace.Resource.TraceInformation("Couldn't pack local-file into {0}, local-file does not exist: {1}",
                    FileName, file.FileName);
                return false;
            }

            byte[] file_bytes = System.Text.Encoding.ASCII.GetBytes(file_data);
            using (MemoryStream ms = new MemoryStream(file_bytes, false))
            {
                PackFileData(blockStream, ms, file);
            }

            return true;
        }

        private bool TryPackFileFromDisk(EndianStream blockStream, string workPath,
            EraFileEntryChunk file)
        {
            string path = Path.Combine(workPath, file.FileName);
            if (!File.Exists(path))
            {
                Debug.Trace.Resource.TraceInformation("Couldn't pack file into {0}, file does not exist: {1}",
                    FileName, file.FileName);
                return false;
            }

            try
            {
                file.FileDateTime = GetMostRecentTimeStamp(path);

                byte[] file_bytes = File.ReadAllBytes(path);
                using (MemoryStream ms = new MemoryStream(file_bytes, false))
                {
                    PackFileData(blockStream, ms, file);
                }
            }
            catch (Exception ex)
            {
                Debug.Trace.Resource.TraceData(System.Diagnostics.TraceEventType.Error, TypeExtensions.kNone,
                    string.Format("Couldn't pack file into {0}, encountered exception dealing with {1}", FileName, file.FileName),
                    ex);
                return false;
            }

            return true;
        }
        #endregion

        #region IEndianStreamSerializable Members
        public void ReadPostprocess(EndianStream s)
        {
            if (mFiles.Count == 0)
            {
                return;
            }

            EraFileExpander expander = s.Owner as EraFileExpander;
            TextWriter progressOutput = expander?.ProgressOutput;
            TextWriter verboseOutput = expander?.VerboseOutput;

            ReadFileNamesChunk(s);
            ValidateFileHashes(s);

            BuildFileNameMaps(verboseOutput);

            if (expander != null && !expander.ExpanderOptions.Test(EraFileExpanderOptions.DontRemoveXmlOrXmbFiles))
            {
                if (expander.ExpanderOptions.Test(EraFileExpanderOptions.DontTranslateXmbFiles))
                {
                    progressOutput?.WriteLine("Removing any XML files if their XMB counterpart exists...");

                    RemoveXmlFilesWhereXmbExists(verboseOutput);
                }
                else
                {
                    progressOutput?.WriteLine("Removing any XMB files if their XML counterpart exists...");

                    RemoveXmbFilesWhereXmlExists(verboseOutput);
                }
            }

            BuildLocalFiles(s);
        }

        private void ReadFileNamesChunk(EndianStream s)
        {
            EraFileEntryChunk filenames_chunk = mFiles[0];

            if (s.Owner is EraFileUtil eraUtil &&
                !eraUtil.Options.Test(EraFileUtilOptions.SkipVerification))
            {
                ValidateAdler32(filenames_chunk, s);
                ValidateHashes(filenames_chunk, s);
            }

            filenames_chunk.FileName = kFileNamesTableName;

            byte[] filenames_buffer = filenames_chunk.GetBuffer(s);
            using (MemoryStream ms = new System.IO.MemoryStream(filenames_buffer, false))
            using (EndianReader er = new EndianReader(ms, s.ByteOrder))
            {
                for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
                {
                    EraFileEntryChunk file = mFiles[x];

                    if (file.FileNameOffset != er.BaseStream.Position)
                    {
                        throw new System.IO.InvalidDataException(string.Format(
                            "#{0} {1} has bad filename offset {2} != {3}",
                            FileIndexToListingIndex(x),
                            file.EntryId.ToString("X16"),
                            file.FileNameOffset.ToString("X8"),
                            er.BaseStream.Position.ToString("X8")
                            ));
                    }

                    file.FileName = er.ReadString(Memory.Strings.StringStorage.CStringAscii);
                }
            }
        }

        private void ValidateFileHashes(EndianStream s)
        {
            EraFileUtil eraUtil = s.Owner as EraFileUtil;

            if (eraUtil != null &&
                eraUtil.Options.Test(EraFileUtilOptions.SkipVerification))
            {
                return;
            }

            if (eraUtil != null && eraUtil.ProgressOutput != null)
            {
                eraUtil.ProgressOutput.WriteLine("\tVerifying file hashes...");
            }

            for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
            {
                EraFileEntryChunk file = mFiles[x];

                if (eraUtil != null && eraUtil.ProgressOutput != null)
                {
                    eraUtil.ProgressOutput.Write("\r\t\t{0} ", file.EntryId.ToString("X16"));
                }

                ValidateAdler32(file, s);
                ValidateHashes(file, s);
            }

            if (eraUtil != null && eraUtil.ProgressOutput != null)
            {
                eraUtil.ProgressOutput.Write("\r\t\t{0} \r", new string(' ', 16));
                eraUtil.ProgressOutput.WriteLine("\t\tDone");
            }
        }

        private void BuildLocalFiles(EndianStream s)
        {
            for (int x = FileChunksFirstIndex; x < mFiles.Count; x++)
            {
                EraFileEntryChunk file = mFiles[x];
                if (!ResourceUtils.IsLocalScenarioFile(file.FileName))
                {
                    continue;
                }

                byte[] file_bytes = file.GetBuffer(s);
                using (MemoryStream ms = new MemoryStream(file_bytes, false))
                using (StreamReader sr = new StreamReader(ms))
                {
                    string file_data = sr.ReadToEnd();

                    mLocalFiles[file.FileName] = file_data;
                }
            }
        }

        //void CalculateFileCompressedDataHashes(EndianStream s)
        //{
        //	for (int x = 0/*FileChunksFirstIndex*/; x < mFiles.Count; x++)
        //	{
        //		var file = mFiles[x];

        //		file.ComputeHash(s, TigerHasher);
        //		System.Array.Copy(TigerHasher.Hash,
        //			file.CompressedDataTiger128, file.CompressedDataTiger128.Length);
        //	}
        //}

        public void Serialize(EndianStream s)
        {
            EraFileUtil eraUtil = s.Owner as EraFileUtil;

            if (s.IsWriting)
            {
                mHeader.UpdateFileCount(mFiles.Count);
            }

            mHeader.Serialize(s);

            if (eraUtil != null && eraUtil.DebugOutput != null)
            {
                eraUtil.DebugOutput.WriteLine("Header position end: {0}",
                    s.BaseStream.Position);
                eraUtil.DebugOutput.WriteLine();
            }

            SerializeFileEntryChunks(s);

            if (eraUtil != null && eraUtil.DebugOutput != null)
            {
                eraUtil.DebugOutput.WriteLine();
            }
        }

        public void SerializeFileEntryChunks(EndianStream s)
        {
            if (s.IsReading)
            {
                mFiles.Capacity = mHeader.FileCount;

                for (int x = 0; x < mFiles.Capacity; x++)
                {
                    EraFileEntryChunk file = new EraFileEntryChunk();
                    file.Serialize(s);

                    mFiles.Add(file);
                }
            }
            else if (s.IsWriting)
            {
                foreach (EraFileEntryChunk f in mFiles)
                {
                    f.Serialize(s);
                }
            }
        }
        #endregion

        #region Local file utils
        private static bool IsIgnoredLocalFile(string fileName)
        {
            return 0 == string.Compare(fileName, "version.txt", System.StringComparison.OrdinalIgnoreCase);
        }

        private void AddVersionFile()
        {
            EraFileEntryChunk file = new EraFileEntryChunk
            {
                CompressionType = ECF.EcfCompressionType.Stored
            };
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            file.FileName = "version.txt";
            file.FileDateTime = BuildModeDefaultTimestamp;
            string version = string.Format("{0}\n{1}\n{2}",
                assembly.FullName,
                assembly.GetName().Version,
                System.Reflection.Assembly.GetEntryAssembly().FullName);
            mLocalFiles[file.FileName] = version;
            mFiles.Add(file);
        }
        #endregion

        internal static DateTime GetMostRecentTimeStamp(string path)
        {
            DateTime creation_time = File.GetCreationTimeUtc(path);
            DateTime write_time = File.GetLastWriteTimeUtc(path);
            return write_time > creation_time
                ? write_time
                : creation_time;
        }
    };
}
