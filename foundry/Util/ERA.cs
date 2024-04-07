using KSoft.Phoenix.Resource;

namespace Foundry.Util
{
    public static class ERA
    {
        public static void ExpandERA(string eraPath, string outputDir)
        {
            using (EraFileExpander expander = new(eraPath))
            {
                expander.Options = new KSoft.Collections.BitVector32();
                expander.Options.Set(EraFileUtilOptions.x64);

                expander.ExpanderOptions = new KSoft.Collections.BitVector32();
                expander.ExpanderOptions.Set(EraFileExpanderOptions.Decrypt);
                expander.ExpanderOptions.Set(EraFileExpanderOptions.DontOverwriteExistingFiles);
                expander.ExpanderOptions.Set(EraFileExpanderOptions.ExpandAsDds);
                expander.ExpanderOptions.Set(EraFileExpanderOptions.RemoveXmb);
                // expander.ExpanderOptions.Set(EraFileExpanderOptions.DontLoadEntireEraIntoMemory);
                expander.ProgressOutput = null;
                expander.VerboseOutput = null;
                expander.DebugOutput = null;

#if DEBUG
                expander.ProgressOutput = Console.Out;
#endif

                expander.Read();
                expander.ExpandTo(outputDir, Path.GetFileNameWithoutExtension(eraPath));
            }

            GC.Collect();
        }
    }
}
