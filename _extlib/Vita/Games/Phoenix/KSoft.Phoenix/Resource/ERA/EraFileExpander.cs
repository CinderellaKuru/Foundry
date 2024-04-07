﻿using KSoft.IO;
using System;
using System.IO;
using FA = System.IO.FileAccess;

namespace KSoft.Phoenix.Resource
{
    public enum EraFileExpanderOptions
    {
        /// <summary>Only the ERA's file listing (.xml) is generated</summary>
        OnlyDumpListing,
        /// <summary>Files that already exist in the output directory will be skipped</summary>
        DontOverwriteExistingFiles,
        /// <summary>Don't perform XMB to XML translations</summary>
        DontTranslateXmbFiles,
        /// <summary>Decompresses Scaleform data</summary>
        DecompressUIFiles,
        /// <summary>Translates GFX files to SWF</summary>
        TranslateGfxFiles,
        Decrypt,
        DontLoadEntireEraIntoMemory,
        DontRemoveXmlOrXmbFiles,
        IgnoreNonDataFiles,
        RemoveXmb,
        ExpandAsDds,

        [Obsolete(EnumBitEncoderBase.kObsoleteMsg, true)] kNumberOf,
    };

    public sealed class EraFileExpander
        : EraFileUtil
    {
        public const string kNameExtension = ".era.bin";
        private Stream mEraBaseStream;
        private EndianStream mEraStream;

        /// <see cref="EraFileExpanderOptions"/>
        public Collections.BitVector32 ExpanderOptions;

        public EraFileExpander(string eraPath)
        {
            mSourceFile = eraPath;
        }

        public override void Dispose()
        {
            base.Dispose();

            Util.DisposeAndNull(ref mEraStream);
            Util.DisposeAndNull(ref mEraBaseStream);
        }

        private bool ReadEraFromStream()
        {
            bool result = EraFileHeader.VerifyIsEraAndDecrypted(mEraStream.Reader);
            if (!result)
            {
                VerboseOutput?.WriteLine("\tFailed: File is either not decrypted, corrupt, or not even an ERA");
            }
            else
            {
                mEraStream.VirtualAddressTranslationInitialize(Shell.ProcessorSize.x32);

                mEraFile = new EraFile
                {
                    FileName = mSourceFile
                };
                mEraFile.Serialize(mEraStream);
                mEraFile.ReadPostprocess(mEraStream);
            }

            return result;
        }

        private bool ReadEraFromFile()
        {
            ProgressOutput?.WriteLine("Opening and reading ERA file {0}...",
                    mSourceFile);

            if (ExpanderOptions.Test(EraFileExpanderOptions.DontLoadEntireEraIntoMemory))
            {
                mEraBaseStream = File.OpenRead(mSourceFile);
            }
            else
            {
                byte[] era_bytes = File.ReadAllBytes(mSourceFile);
                if (ExpanderOptions.Test(EraFileExpanderOptions.Decrypt))
                {
                    ProgressOutput?.WriteLine("Decrypting...");

                    DecryptFileBytes(era_bytes);
                }

                mEraBaseStream = new MemoryStream(era_bytes, writable: false);
            }

            mEraStream = new EndianStream(mEraBaseStream, Shell.EndianFormat.Big, this, permissions: FA.Read)
            {
                StreamMode = FA.Read
            };

            return ReadEraFromStream();
        }

        private void DecryptFileBytes(byte[] eraBytes)
        {
            using (MemoryStream era_in_ms = new MemoryStream(eraBytes, writable: false))
            using (MemoryStream era_out_ms = new MemoryStream(eraBytes, writable: true))
            using (EndianReader era_reader = new EndianReader(era_in_ms, Shell.EndianFormat.Big))
            using (EndianWriter era_writer = new EndianWriter(era_out_ms, Shell.EndianFormat.Big))
            {
                // "Halo Wars Alpha 093106 Feb 21 2009" was released pre-decrypted, so try and detect if the file is already decrypted first
                if (!EraFileHeader.VerifyIsEraAndDecrypted(era_reader))
                {
                    CryptStream(era_reader, era_writer,
                        Security.Cryptography.CryptographyTransformType.Decrypt);
                }
            }
        }

        public bool Read()
        {
            bool result = true;

            try { result &= ReadEraFromFile(); }
            catch (Exception ex)
            {
                VerboseOutput?.WriteLine("\tEncountered an error while trying to read the ERA: {0}", ex);
                result = false;
            }

            return result;
        }

        private void SaveListing(string workPath, string listingName)
        {
            string listing_filename = Path.Combine(workPath, listingName);

            using (XmlElementStream xml = XmlElementStream.CreateForWrite("EraArchive", this))
            {
                xml.InitializeAtRootElement();
                xml.StreamMode = FA.Write;

                mEraFile.WriteDefinition(xml);

                xml.Document.Save(listing_filename + EraFileBuilder.kNameExtension);
            }
        }
        public bool ExpandTo(string workPath, string listingName)
        {
            if (mEraFile == null)
            {
                return false;
            }

            if (!Directory.Exists(workPath))
            {
                _ = Directory.CreateDirectory(workPath);
            }

            bool result = true;

            ProgressOutput?.WriteLine("Outputting listing...");

            try { SaveListing(workPath, listingName); }
            catch (Exception ex)
            {
                VerboseOutput?.WriteLine("\tEncountered an error while outputting listing: {0}", ex);
                result = false;
            }

            if (result && !ExpanderOptions.Test(EraFileExpanderOptions.OnlyDumpListing))
            {
                ProgressOutput?.WriteLine("Expanding archive to {0}...", workPath);

                try { mEraFile.ExpandTo(mEraStream, workPath); }
                catch (Exception ex)
                {
                    VerboseOutput?.WriteLine("\tEncountered an error while expanding archive: {0}", ex);
                    result = false;
                }

                ProgressOutput?.WriteLine("Done");
            }

            mEraStream.Close();

            return result;
        }
    };
}
