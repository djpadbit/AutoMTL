using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript.Converter;

namespace VPet.Plugin.AutoMTL
{
	public class Setting
	{
		/// <summary>
		/// Enables title case for the returned translations (needs restart + cache clear)
		/// </summary>
		[Line]
		public bool titleCase { get; set; } = true;
		/// <summary>
		/// The time between translation requests in ms
		/// </summary>
		[Line]
		public long waitMs { get; set; } = 20;
		/// <summary>
		/// Translation provider Id
		/// </summary>
		[Line]
		public string providerId { get; set; } = "google";
		/// <summary>
		/// Source Language
		/// </summary>
		[Line]
		public string srcLangId { get; set; } = "auto";
		/// <summary>
		/// Destination Language
		/// </summary>
		[Line]
		public string dstLangId { get; set; } = "en";
		/// <summary>
		/// Enable the Automatic Machine Translation (needs restart)
		/// </summary>
		[Line]
		public bool enabled { get; set; } = true;

		public void sanitize()
		{
			if (!TranslatorMananger.translatorNames.ContainsKey(providerId))
				providerId = "google";
			Dictionary<string, string> langs = TranslatorMananger.getTranslatorLangs(providerId);
			if (!langs.ContainsKey(srcLangId))
				srcLangId= "auto";
			if (!langs.ContainsKey(dstLangId))
				srcLangId = "en";
		}

		public TranslatorBase createTranslator(string cacheBase)
		{
			TranslatorBase trans = TranslatorMananger.constructTranslator(providerId, srcLangId, dstLangId, cacheBase);
			trans.msBetweenCalls = waitMs;
			trans.titleCase = titleCase;
			return trans;
		}
	}
}
