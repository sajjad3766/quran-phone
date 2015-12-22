// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the SettingsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Quran.Core.Data;
using Quran.Core.Properties;
using Quran.Core.Utils;

namespace Quran.Core.ViewModels
{
    

    /// <summary>
    /// Define the SettingsViewModel type.
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        { }

        #region Properties
        private string activeTranslation;
        public string ActiveTranslation
        {
            get { return activeTranslation; }
            set
            {
                if (value == activeTranslation)
                    return;

                activeTranslation = value;

                base.OnPropertyChanged(() => ActiveTranslation);
            }
        }

        private string activeReciter;
        public string ActiveReciter
        {
            get { return activeReciter; }
            set
            {
                if (value == activeReciter)
                    return;

                activeReciter = value;

                base.OnPropertyChanged(() => ActiveReciter);
            }
        }

        private double textSize;
        public double TextSize
        {
            get { return textSize; }
            set
            {
                if (value == textSize)
                    return;

                textSize = value;

                base.OnPropertyChanged(() => TextSize);
            }
        }
        
        private bool showArabicInTranslation;
        public bool ShowArabicInTranslation
        {
            get { return showArabicInTranslation; }
            set
            {
                if (value == showArabicInTranslation)
                    return;

                showArabicInTranslation = value;
                base.OnPropertyChanged(() => ShowArabicInTranslation);
            }
        }

        private bool altDownloadMethod;
        public bool AltDownloadMethod
        {
            get { return altDownloadMethod; }
            set
            {
                if (value == altDownloadMethod)
                    return;

                altDownloadMethod = value;
                base.OnPropertyChanged(() => AltDownloadMethod);
            }
        }
        
        private bool enableShowArabicInTranslation;
        public bool EnableShowArabicInTranslation
        {
            get { return enableShowArabicInTranslation; }
            set
            {
                if (value == enableShowArabicInTranslation)
                    return;

                enableShowArabicInTranslation = value;

                base.OnPropertyChanged(() => EnableShowArabicInTranslation);
            }
        }

        private bool preventPhoneFromSleeping;
        public bool PreventPhoneFromSleeping
        {
            get { return preventPhoneFromSleeping; }
            set
            {
                if (value == preventPhoneFromSleeping)
                    return;

                preventPhoneFromSleeping = value;
                var oldValue = SettingsUtils.Get<bool>(Constants.PREF_PREVENT_SLEEP);
                if (oldValue != value)
                {
                    QuranApp.NativeProvider.ToggleDeviceSleep(!value);
                }

                base.OnPropertyChanged(() => PreventPhoneFromSleeping);
            }
        }
        
        private bool nightMode;
        public bool NightMode
        {
            get { return nightMode; }
            set
            {
                if (value == nightMode)
                    return;

                nightMode = value;

                // saving to setting utils

                base.OnPropertyChanged(() => NightMode);
            }
        }

        private bool keepInfoOverlay;
        public bool KeepInfoOverlay
        {
            get { return keepInfoOverlay; }
            set
            {
                if (value == keepInfoOverlay)
                    return;

                keepInfoOverlay = value;

                // saving to setting utils

                base.OnPropertyChanged(() => KeepInfoOverlay);
            }
        }

        private bool repeatAudio;
        public bool RepeatAudio
        {
            get { return repeatAudio; }
            set
            {
                if (value == repeatAudio)
                    return;

                repeatAudio = value;

                // saving to setting utils

                base.OnPropertyChanged(() => RepeatAudio);
            }
        }
        
        private string selectedLanguage;
        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                if (value == selectedLanguage)
                    return;

                selectedLanguage = value;

                if (SettingsUtils.Get<string>(Constants.PREF_CULTURE_OVERRIDE) != value)
                {
                    QuranApp.NativeProvider.ShowInfoMessageBox(Resources.please_restart);
                }

                base.OnPropertyChanged(() => SelectedLanguage);
            }
        }

        private string selectedAudioBlock = "Page";
        public string SelectedAudioBlock
        {
            get { return selectedAudioBlock; }
            set
            {
                if (value == selectedAudioBlock)
                    return;

                selectedAudioBlock = value;

                base.OnPropertyChanged(() => SelectedAudioBlock);
            }
        }

        private KeyValuePair<RepeatAmount, string> selectedRepeatAmount = new KeyValuePair<RepeatAmount,string>((RepeatAmount)100, "");
        public KeyValuePair<RepeatAmount, string> SelectedRepeatAmount
        {
            get { return selectedRepeatAmount; }
            set
            {
                if (value.Key == selectedRepeatAmount.Key)
                    return;

                selectedRepeatAmount = value;

                base.OnPropertyChanged(() => SelectedRepeatAmount);
            }
        }

        private KeyValuePair<int, string> selectedRepeatTimes = new KeyValuePair<int,string>(-1, "");
        public KeyValuePair<int, string> SelectedRepeatTimes
        {
            get { return selectedRepeatTimes; }
            set
            {
                if (value.Key == selectedRepeatTimes.Key)
                    return;

                selectedRepeatTimes = value;

                base.OnPropertyChanged(() => SelectedRepeatTimes);
            }
        }
        
        public ObservableCollection<KeyValuePair<string, string>> SupportedLanguages { get; private set; }

        public ObservableCollection<KeyValuePair<string, string>> SupportedAudioBlocks { get; private set; }

        public ObservableCollection<KeyValuePair<RepeatAmount, string>> SupportedRepeatAmount { get; private set; }

        public ObservableCollection<KeyValuePair<int, string>> SupportedRepeatTimes { get; private set; }
        
        public bool CanGenerateDuaDownload
        {
            get
            {
                return true;
            }
        }
        #endregion Properties
        public override Task Initialize()
        {
            return Refresh();
        }

        public override async Task Refresh()
        {
            SupportedLanguages = new ObservableCollection<KeyValuePair<string, string>>();
            foreach (var lang in GetSupportedLanguages())
            {
                SupportedLanguages.Add(lang);
            }
            SupportedAudioBlocks = new ObservableCollection<KeyValuePair<string, string>>();
            foreach (var enumValue in Enum.GetNames(typeof(AudioDownloadAmount)))
            {
                SupportedAudioBlocks.Add(new KeyValuePair<string, string>(enumValue, enumValue));
            }
            SupportedRepeatAmount = new ObservableCollection<KeyValuePair<RepeatAmount, string>>();
            foreach (var repeatValue in GetSupportedRepeatAmounts())
            {
                SupportedRepeatAmount.Add(new KeyValuePair<RepeatAmount, string>(repeatValue.Key, repeatValue.Value));
            }
            SupportedRepeatTimes = new ObservableCollection<KeyValuePair<int, string>>();
            foreach (var repeatValue in GetSupportedRepeatTimes())
            {
                SupportedRepeatTimes.Add(new KeyValuePair<int, string>(repeatValue.Key, repeatValue.Value));
            }

            var translation = SettingsUtils.Get<string>(Constants.PREF_ACTIVE_TRANSLATION);
            if (!string.IsNullOrEmpty(translation) && translation.Contains("|"))
                ActiveTranslation = translation.Split('|')[1];
            else
                ActiveTranslation = "None";

            var reciter = SettingsUtils.Get<string>(Constants.PREF_ACTIVE_QARI);
            if (!string.IsNullOrEmpty(reciter))
                ActiveReciter = reciter;
            else
                ActiveReciter = "None";

            SelectedLanguage = SettingsUtils.Get<string>(Constants.PREF_CULTURE_OVERRIDE);
            SelectedAudioBlock = SettingsUtils.Get<AudioDownloadAmount>(Constants.PREF_DOWNLOAD_AMOUNT).ToString();
            SelectedRepeatAmount = SupportedRepeatAmount.First(kv => kv.Key == SettingsUtils.Get<RepeatAmount>(Constants.PREF_REPEAT_AMOUNT));
            SelectedRepeatTimes = SupportedRepeatTimes.First(kv => kv.Key == SettingsUtils.Get<int>(Constants.PREF_REPEAT_TIMES));
            RepeatAudio = SettingsUtils.Get<bool>(Constants.PREF_AUDIO_REPEAT);
            TextSize = SettingsUtils.Get<double>(Constants.PREF_TRANSLATION_TEXT_SIZE);
            ShowArabicInTranslation = SettingsUtils.Get<bool>(Constants.PREF_SHOW_ARABIC_IN_TRANSLATION);
            AltDownloadMethod = SettingsUtils.Get<bool>(Constants.PREF_ALT_DOWNLOAD);
            PreventPhoneFromSleeping = SettingsUtils.Get<bool>(Constants.PREF_PREVENT_SLEEP);
            KeepInfoOverlay = SettingsUtils.Get<bool>(Constants.PREF_KEEP_INFO_OVERLAY);
            NightMode = SettingsUtils.Get<bool>(Constants.PREF_NIGHT_MODE);

            if (await FileUtils.HaveArabicSearchFile())
            {
                EnableShowArabicInTranslation = true;
            }
            else
            {
                EnableShowArabicInTranslation = false;
            }
        }

        public void SaveSettings()
        {
            SettingsUtils.Set(Constants.PREF_REPEAT_TIMES, SelectedRepeatTimes.Key);
            SettingsUtils.Set(Constants.PREF_REPEAT_AMOUNT, SelectedRepeatAmount.Key);
            SettingsUtils.Set(Constants.PREF_DOWNLOAD_AMOUNT, (AudioDownloadAmount)Enum.Parse(typeof(AudioDownloadAmount), SelectedAudioBlock, true));
            SettingsUtils.Set(Constants.PREF_CULTURE_OVERRIDE, SelectedLanguage);
            SettingsUtils.Set(Constants.PREF_AUDIO_REPEAT, RepeatAudio);
            SettingsUtils.Set(Constants.PREF_KEEP_INFO_OVERLAY, KeepInfoOverlay);
            SettingsUtils.Set(Constants.PREF_NIGHT_MODE, NightMode);
            SettingsUtils.Set(Constants.PREF_PREVENT_SLEEP, PreventPhoneFromSleeping);
            SettingsUtils.Set(Constants.PREF_SHOW_ARABIC_IN_TRANSLATION, ShowArabicInTranslation);
            SettingsUtils.Set(Constants.PREF_ALT_DOWNLOAD, AltDownloadMethod);
            SettingsUtils.Set(Constants.PREF_TRANSLATION_TEXT_SIZE, TextSize);
        }


        public void GenerateDua()
        {
            DuaGenerator.Generate();
        }

        public void ContactUs()
        {
            QuranApp.NativeProvider.ComposeEmail("quran.phone@gmail.com", "Email from QuranPhone");
        }

        private void Navigate(string link)
        {
            if (link != null)
            {
                QuranApp.NativeProvider.LaunchWebBrowser(link);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetSupportedLanguages()
        {
            yield return new KeyValuePair<string, string>("", "Default");
            var cultures = new string[] { "ar", "en", "id", "ru" };
            foreach (var c in cultures)
            {
                CultureInfo cultureInfo = null;
                try
                {
                    cultureInfo = new CultureInfo(c);
                }
                catch
                {
                    // Ignore
                }
                if (cultureInfo != null)
                {
                    yield return new KeyValuePair<string, string>(c, cultureInfo.NativeName);
                }
            }
        }

        private IEnumerable<KeyValuePair<int, string>> GetSupportedRepeatTimes()
        {
            yield return new KeyValuePair<int, string>(0, Resources.none);
            yield return new KeyValuePair<int, string>(1, "1");
            yield return new KeyValuePair<int, string>(2, "2");
            yield return new KeyValuePair<int, string>(3, "3");
            yield return new KeyValuePair<int, string>(5, "5");
            yield return new KeyValuePair<int, string>(10, "10");
            yield return new KeyValuePair<int, string>(int.MaxValue, Resources.unlimited);
        }

        private IEnumerable<KeyValuePair<RepeatAmount, string>> GetSupportedRepeatAmounts()
        {
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.None, Resources.none);
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.OneAyah, "1 " + QuranUtils.GetAyahTitle());
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.ThreeAyah, "3 " + QuranUtils.GetAyahTitle());
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.FiveAyah, "5 " + QuranUtils.GetAyahTitle());
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.TenAyah, "10 " + QuranUtils.GetAyahTitle());
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.Page, Resources.quran_page);
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.Surah, Resources.quran_sura_lower);
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.Rub, Resources.quran_rub3);
            yield return new KeyValuePair<RepeatAmount, string>(RepeatAmount.Juz, Resources.quran_juz2_lower);
        }
    }
}
