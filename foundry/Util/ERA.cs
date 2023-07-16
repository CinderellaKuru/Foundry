using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Util
{
	public static class ERA
	{
		public static void ExpandERA(string eraPath, string outputDir)
		{
			KSoft.Phoenix.Resource.EraFileExpander expander = new KSoft.Phoenix.Resource.EraFileExpander(eraPath);

			var expanderOptions = new KSoft.Collections.BitVector32();
			expanderOptions.Set(KSoft.Phoenix.Resource.EraFileExpanderOptions.Decrypt);
			expander.ExpanderOptions = expanderOptions;

			var options = new KSoft.Collections.BitVector32();
			options.Set(KSoft.Phoenix.Resource.EraFileUtilOptions.x64);
			expander.Options = options;

			expander.Read();
			expander.ExpandTo(outputDir, Path.GetFileNameWithoutExtension(eraPath));
			expander.Dispose();
		}
	}
}
