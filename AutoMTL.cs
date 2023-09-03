using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LinePutScript;
using LinePutScript.Converter;
using LinePutScript.Localization.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VPet_Simulator.Core;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.AutoMTL
{
	public class AutoMTL : MainPlugin
	{
		public override string PluginName => "AutoMTL";

		public Setting settings;
		private Func<string, string> oldTranslateFunc;
		private TranslatorBase translator;

		public AutoMTL(IMainWindow mainwin) : base(mainwin)
		{
			// We shouldn't do this here, but some stuff gets translated earlier
			// Than the initialisation, so just setup right the hook after construction
			// Getting the settings seems to work fine...
			ILine line = MW.Set.FindLine("AutoMTL");
			if (line == null)
				settings = new Setting();
			else
				settings = LPSConvert.DeserializeObject<Setting>(line);

			InitTranslationHook();
			// Try to notify the bindings to get some translation events...
			// I don't actually know if it does much
			LocalizeCore.LoadCulture(mainwin.Set.Language);
			// Potentially sift through all translated text to find fake translations and remove them?
		}

		public override void LoadPlugin()
		{
			// We already setup the translation hook and loaded the config,
			// So just setup the menu item for the settings window here
			MenuItem modConfig = MW.Main.ToolBar.MenuMODConfig;
			modConfig.Visibility = Visibility.Visible;
			MenuItem menuItem = new MenuItem()
			{
				Header = "AutoMTL",
				HorizontalContentAlignment = HorizontalAlignment.Center,
			};
			menuItem.Click += (s, e) => { Setting(); };
			modConfig.Items.Add(menuItem);
		}

		private void InitTranslationHook()
		{
			// Hook LocalizeCore
			// Keep a reference to the old function, just in case
			this.oldTranslateFunc = LocalizeCore.TranslateFunc;
			LocalizeCore.TranslateFunc = TranslateFunc;

			//Init our Translator of choice
			updateTranslator();
		}

		public void clearCache()
		{
			translator.ClearCache();
			foreach (var tmpfile in Directory.GetFiles(GraphCore.CachePath + @"\mtl")) {
				try {
					File.Delete(tmpfile);
				} finally { }
			}
		}

		public void updateTranslator()
		{
			translator = settings.createTranslator(GraphCore.CachePath);
		}

		private string TranslateFunc(string input)
		{
			// Compatibility with other plugins ?, run other function translate function before
			if (oldTranslateFunc != null) {
				string output = oldTranslateFunc(input);
				if (!settings.enabled)
					return output;
				if (output != null)
					input = output;
			}
			if (!settings.enabled)
				return null;
			if (translator == null) // In case we can't load it somehow, don't crash the app
				return null;
			return translator.Translate(input);
		}

		public SettingsWindow settingsWindow;
		public override void Setting()
		{
			if (settingsWindow == null) {
				settingsWindow = new SettingsWindow(this);
				settingsWindow.Show();
			} else {
				settingsWindow.Topmost = true;
			}
		}
	}
}
