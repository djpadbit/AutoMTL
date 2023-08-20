using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VPet.Plugin.AutoMTL
{
	// Not implemented yet, needs quite a bit more work than the google one
	// CBA to do it right now
	/*class TranslatorBing : TranslatorBase
	{
		public static new string providerName = "Bing Translate";
		public static new string providerId = "bing";
		public static new Dictionary<string, string> providedLanguages => JsonConvert.DeserializeObject<Dictionary<string, string>>(Constants.bingLangJSON);

		public TranslatorBing(string srcLang, string dstLang, string cacheBase) : base(srcLang, dstLang, cacheBase)
		{
		}

		// https://github.com/dmytrovoytko/SublimeText-Translate/blob/master/Translator.py
		public override string TranslateString(string input)
		{
			return null;
		}
	}*/
}
