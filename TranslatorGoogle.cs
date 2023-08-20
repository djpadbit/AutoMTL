using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VPet.Plugin.AutoMTL
{
	class TranslatorGoogle : TranslatorBase
	{
		public static new string providerName = "Google Translate";
		public static new string providerId =  "google";
		public static new Dictionary<string, string> providedLanguages => JsonConvert.DeserializeObject<Dictionary<string, string>>(Constants.googleLangJSON);

		public TranslatorGoogle(string srcLang, string dstLang, string cacheBase) : base(srcLang,dstLang,cacheBase)
		{
		}

		// https://stackoverflow.com/questions/50963296/c-sharp-google-translate-without-api-and-with-unicode
		// https://github.com/dmytrovoytko/SublimeText-Translate/blob/master/Translator.py
		public override string TranslateString(string input)
		{
			try {
				string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", srcLang, dstLang, input);
				
				WebClient webClient = new WebClient();
				webClient.Encoding = System.Text.Encoding.UTF8;
				string result = webClient.DownloadString(url);
				JArray json = (JArray)JsonConvert.DeserializeObject(result);

				string output = "";
				foreach (JArray item in json[0])
					output += item[0].Value<string>();

				if (output == "")
					return null;

				return output;
			} catch (Exception exc) {
				string error = exc.ToString();
				//MessageBox.Show(error);
				return null;
			}

		}
	}
}
