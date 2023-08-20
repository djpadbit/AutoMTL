using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LinePutScript.Converter;

namespace VPet.Plugin.AutoMTL
{
	/// <summary>
	/// SettingsWindow.xaml Logic
	/// </summary>
	public partial class SettingsWindow : Window
	{
		AutoMTL amtl;

		public SettingsWindow(AutoMTL amtl)
		{
			InitializeComponent();
			this.amtl = amtl;
			Setting cfg = amtl.settings;
			cfg.sanitize();

			SwitchOn.IsChecked = cfg.enabled;
			SwitchTitleCase.IsChecked = cfg.titleCase;
			WaitSlider.Value = cfg.waitMs;
			Dictionary<string, string> providers = TranslatorMananger.translatorNames;
			CombMTLProvider.Items.Clear();
			foreach (string name in providers.Values)
				CombMTLProvider.Items.Add(name);
			CombMTLProvider.Text = TranslatorMananger.getTranslatorName(cfg.providerId);
			UpdateLangs(cfg.providerId);
			Dictionary<string, string> langs = TranslatorMananger.getTranslatorLangs(cfg.providerId);
			CombSrcLang.Text = langs[cfg.srcLangId];
			CombDstLang.Text = langs[cfg.dstLangId];
		}

		private void UpdateLangs(string providerId)
		{
			CombSrcLang.Items.Clear();
			CombDstLang.Items.Clear();
			Dictionary<string, string> langs = TranslatorMananger.getTranslatorLangs(providerId);
			foreach (string name in langs.Values) {
				CombSrcLang.Items.Add(name);
				if (name != "Automatic")
					CombDstLang.Items.Add(name);
			}
		}

		private void Provider_Changed(object sender, SelectionChangedEventArgs e)
		{
			UpdateLangs(TranslatorMananger.GetProviderId(CombMTLProvider.Text));
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			amtl.settings.enabled = SwitchOn.IsChecked.Value;
			amtl.settings.titleCase = SwitchTitleCase.IsChecked.Value;
			amtl.settings.waitMs = (long)WaitSlider.Value;
			string providerId = TranslatorMananger.GetProviderId(CombMTLProvider.Text);
			amtl.settings.providerId = providerId;
			amtl.settings.srcLangId = TranslatorMananger.GetLangId(providerId, CombSrcLang.Text);
			amtl.settings.dstLangId = TranslatorMananger.GetLangId(providerId, CombDstLang.Text);
			// Apply changes to the translator
			amtl.updateTranslator();
			// Save changes
			amtl.MW.Set.Remove("AutoMTL");
			amtl.MW.Set.Add(LPSConvert.SerializeObject(amtl.settings, "AutoMTL"));
			this.Close();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			amtl.settingsWindow = null;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			amtl.clearCache();
		}
	}
}
