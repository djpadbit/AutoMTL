using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VPet.Plugin.AutoMTL
{
	// This class finds all translation providers at startup and stores them
	// This allows to easly add translation providers by just adding a class that extends TranslatorBase
	// And that defines providerId, providerName and ProvidedLangs()
	public class TranslatorMananger
	{
		public static Dictionary<string, string> translatorNames = new Dictionary<string, string>();
		public static Dictionary<string, Dictionary<string, string>> translatorLangs = new Dictionary<string, Dictionary<string, string>>();
		public static Dictionary<string, Type> translatorClasses = new Dictionary<string, Type>();

		static TranslatorMananger() {
			foreach (Type type in Assembly.GetAssembly(typeof(TranslatorBase)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(TranslatorBase)))) {
				try { // Ingore broken providers
					string id = (string)type.GetField("providerId", BindingFlags.Public | BindingFlags.Static).GetValue(null);
					string name = (string)type.GetField("providerName", BindingFlags.Public | BindingFlags.Static).GetValue(null);
					Dictionary<string, string> langs = (Dictionary<string, string>)type.GetProperty("providedLanguages", BindingFlags.Public | BindingFlags.Static)
										.GetValue(null);

					translatorClasses[id] = type;
					translatorNames[id] = name;
					translatorLangs[id] = langs;
				} finally { }
			}
		}

		public static string GetProviderId(string name)
		{
			return translatorNames.FirstOrDefault(x => x.Value == name).Key;
		}

		public static string GetProviderId(TranslatorBase trans)
		{
			return (string)trans.GetType().GetField("providerId", BindingFlags.Public | BindingFlags.Static).GetValue(null);
		}

		public static string GetLangId(string providerId, string name)
		{
			Dictionary<string, string> langs = getTranslatorLangs(providerId);
			if (langs == null)
				return null;
			return langs.FirstOrDefault(x => x.Value == name).Key;
		}

		public static string getTranslatorName(string id)
		{
			if (!translatorNames.ContainsKey(id))
				return null;
			return translatorNames[id];
		}

		public static Dictionary<string, string> getTranslatorLangs(string id)
		{
			if (!translatorLangs.ContainsKey(id))
				return null;
			return translatorLangs[id];
		}

		public static TranslatorBase constructTranslator(string id, string srcLang, string dstLang, string cacheBase)
		{
			if (!translatorClasses.ContainsKey(id))
				return null;
			return (TranslatorBase)Activator.CreateInstance(translatorClasses[id], srcLang, dstLang, cacheBase);
		}
	}
}
