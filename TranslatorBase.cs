using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VPet.Plugin.AutoMTL
{
    public abstract class TranslatorBase
    {
        // Stuff that needs to be implemented in class that adds a translator
        // cf. TranslatorGoogle
        public static string providerName = "None";
        public static string providerId = "none";

        // Provider needs to implement this function, actually translates a string
        // This class takes care of all the caching and rate limiting
        public abstract string TranslateString(string input);
        // This is the getter that returns all the languages that can be used
        public static Dictionary<string, string> providedLanguages => null;

        // Other stuff

        // Can be changed on the fly
        public long msBetweenCalls;
        // Can't be changed on the fly, needs to clear cache and restart
        public bool titleCase;

        public readonly string srcLang;
        public readonly string dstLang;

        // Private stuff

        private readonly string cacheBase;
        private readonly string cacheFile;
        private TextInfo txtInfo;
        private Dictionary<string, string> cache;
        private long lastTime;

        public TranslatorBase(string srcLang, string dstLang, string cacheBase)
        {
            this.lastTime = 0;
            this.titleCase = true;
            this.msBetweenCalls = 20;
            this.srcLang = srcLang;
            this.dstLang = dstLang;
            this.cacheBase = cacheBase + @"\mtl";
            string providerId = TranslatorMananger.GetProviderId(this);
            this.cacheFile = this.cacheBase + String.Format(@"\{0}-{1}-{2}.json", providerId, srcLang, dstLang);
            this.txtInfo = new CultureInfo("en-US", false).TextInfo;
            InitCache();
        }

        void InitCache()
        {
            if (!Directory.Exists(cacheBase))
                Directory.CreateDirectory(cacheBase);

            if (File.Exists(cacheFile))
                cache = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(cacheFile));
            else
                cache = new Dictionary<string, string>();
        }

        void SaveCache()
        {
            File.WriteAllText(cacheFile, JsonConvert.SerializeObject(cache, Formatting.Indented));
        }

        public void ClearCache()
        {
            cache.Clear();
            if (File.Exists(cacheFile))
                File.Delete(cacheFile);
        }

        public string Translate(string input)
        {
            // Already in cache
            if (cache.ContainsKey(input))
                return cache[input];

            // Do rate limiting if needed
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long timeDiff = currentTime - lastTime;
            if (timeDiff < msBetweenCalls)
                Thread.Sleep((int)timeDiff);
            lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Run the translator
            string output = TranslateString(input);
            if (output == null)
                return null;

            // Check if it's the same as the input (could be same language)
            if (output.ToLower().Trim() == input.ToLower().Trim())
                return null;

            // Format if needed
            if (titleCase)
                output = txtInfo.ToTitleCase(output);

            // Cache the output and return
            cache[input] = output;
            // Saving at every change is not great...
            SaveCache();
            return output;
        }
    }
}
