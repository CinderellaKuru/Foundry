﻿using System;
using System.IO;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

namespace KSoft.Phoenix.Resource.PKG
{
	public enum CaPackageFileBuilderOptions
	{
		AlwaysUseXmlOverXmb,

		[Obsolete(EnumBitEncoderBase.kObsoleteMsg, true)] kNumberOf,
	};

	public sealed class CaPackageFileBuilder
		: CaPackageFileUtil
	{
		/// <see cref="CaPackageFileBuilderOptions"/>
		public Collections.BitVector32 BuilderOptions;

		public CaPackageFileBuilder(string listingPath)
		{
			if (Path.GetExtension(listingPath) != CaPackageFileDefinition.kFileExtension)
				listingPath += CaPackageFileDefinition.kFileExtension;

			mSourceFile = listingPath;
		}

		#region Reading
		bool ReadInternal()
		{
			bool result = true;

			if (ProgressOutput != null)
				ProgressOutput.WriteLine("Trying to read source listing {0}...", mSourceFile);

			if (!File.Exists(mSourceFile))
				result = false;
			else
			{
				mPkgFile = new CaPackageFile();

				using (var xml = new IO.XmlElementStream(mSourceFile, FileAccess.Read, this))
				{
					xml.InitializeAtRootElement();
					PkgDefinition.Serialize(xml);
				}
			}

			if (result == false)
			{
				if (ProgressOutput != null)
					ProgressOutput.WriteLine("\tFailed!");
			}

			return result;
		}
		public bool Read() // read the listing definition
		{
			bool result = true;

			try { result &= ReadInternal(); }
			catch (Exception ex)
			{
				if (VerboseOutput != null)
					VerboseOutput.WriteLine("\tEncountered an error while trying to read listing: {0}", ex);
				result = false;
			}

			return result;
		}
		#endregion

		bool BuildInternal(string workPath, string pkgName, string outputPath)
		{
			string pkg_filename = Path.Combine(outputPath, pkgName);

			if (File.Exists(pkg_filename))
			{
				var attrs = File.GetAttributes(pkg_filename);
				if (attrs.HasFlag(FileAttributes.ReadOnly))
					throw new IOException("PKG file is readonly, can't build: " + pkg_filename);
			}

			{
				string sanitizedWorkPath = CaPackageFileDefinition.SanitizeWorkingEnvironmentPath(workPath);
				if (sanitizedWorkPath != workPath && VerboseOutput != null)
				{
					VerboseOutput.WriteLine("Sanitized work path:");
					VerboseOutput.WriteLine("\t{0} =>", workPath);
					VerboseOutput.WriteLine("\t{0}", sanitizedWorkPath);
					workPath = sanitizedWorkPath;
				}
			}

			if (ProgressOutput != null)
				ProgressOutput.WriteLine("Apply changes to PKG listing for work environment...");
			if (PkgDefinition.RedefineForWorkingEnvironment(workPath
				, BuilderOptions.Test(CaPackageFileBuilderOptions.AlwaysUseXmlOverXmb)
				, VerboseOutput))
			{

			}

			mPkgFile = new CaPackageFile();
			mPkgFile.SetupHeaderAndEntries(PkgDefinition);

			const int k_initial_buffer_size = 16 * IntegerMath.kMega; // 16MB

			if (ProgressOutput != null)
				ProgressOutput.WriteLine("Building {0} to {1}...", pkgName, outputPath);

			if (ProgressOutput != null)
				ProgressOutput.WriteLine("\tAllocating memory...");
			bool result = true;
			using (var ms = new MemoryStream(k_initial_buffer_size))
			using (var pkg_memory = new IO.EndianStream(ms, Shell.EndianFormat.Little, this, permissions: FileAccess.Write))
			{
				pkg_memory.StreamMode = FileAccess.Write;

				// #TODO:

				// create null bytes for the header and embedded file chunk descriptors
				// previously just used Seek to do this, but it doesn't update Length.
				long preamble_size = mPkgFile.CalculateHeaderAndFileChunksSize((CaPackageVersion)CaPackageFile.kCurrentVersion);
				ms.SetLength(preamble_size);
				ms.Seek(preamble_size, SeekOrigin.Begin);

				// now we can start embedding the files
				if (ProgressOutput != null)
					ProgressOutput.WriteLine("\tPacking files...");
				result = false;//result && mPkgFile.Build(pkg_memory, workPath);

				if (result)
				{
					if (ProgressOutput != null)
						ProgressOutput.WriteLine("\tFinializing...");

					// seek back to the start of the PKG and write out the finalized header and file entries
					ms.Seek(0, SeekOrigin.Begin);
					mPkgFile.Serialize(pkg_memory);

					Contract.Assert(pkg_memory.BaseStream.Position == preamble_size,
						"Written PKG header size is greater than what we calculated");

					using (var fs = new FileStream(pkg_filename, FileMode.Create, FileAccess.Write))
						ms.WriteTo(fs);
				}
			}
			return result;
		}
	};
}

