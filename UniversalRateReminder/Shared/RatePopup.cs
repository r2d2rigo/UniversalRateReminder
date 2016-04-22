using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;

namespace UniversalRateReminder
{
    /// <summary>
    /// A static class for showing a pop up for rating the app after the app has been launched a specific number of times.
    /// </summary>
    public static class RatePopup
    {
        /// <summary>
        /// Name of the container for holding the settings.
        /// </summary>
        private static readonly string UniversalRateReminderContainerName = "UniversalRateReminder";

        /// <summary>
        /// Launch count setting property name.
        /// </summary>
        private static readonly string CountPropertyName = "Count";

        /// <summary>
        /// Dismissed setting property name.
        /// </summary>
        private static readonly string DismissedPropertyName = "Dismissed";

        /// <summary>
        /// Container for saving the settings.
        /// </summary>
        private static ApplicationDataContainer reminderContainer;

        /// <summary>
        /// The default number of times the application needs to be launched before showing the reminder. The default value is 5.
        /// </summary>
        public static readonly int DefaultLaunchLimitForReminder = 5;


        /// <summary>
        /// App version property name
        /// </summary>
        private static readonly string AppVersionPropertyName = "AppVersion";

        /// <summary>
        /// The title for the rate pop up. The default value is "Rate us!".
        /// </summary>
        public static string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The text content for the rate pop up. The default value is "Your feedback helps you improve this app. If you like it, please take a minute and rate it with five stars so we can continue working on new features and updates.".
        /// </summary>
        public static string Content
        {
            get;
            set;
        }

        /// <summary>
        /// The text for the rate button. The default value is "rate 5 stars".
        /// </summary>
        public static string RateButtonText
        {
            get;
            set;
        }

        /// <summary>
        /// The text for the cancel button. The default value is "no, thanks".
        /// </summary>
        public static string CancelButtonText
        {
            get;
            set;
        }

        /// <summary>
        /// The number of times the applications needs to be launched before showing the reminder. The default value is <see cref="RatePopup.DefaultLaunchLimitForReminder"/>.
        /// </summary>
        public static int LaunchLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Allows developer to reset the launch count when a new version is released
        /// </summary>
        public static bool ResetCountOnNewVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Static constructor for initializing default values.
        /// </summary>
        static RatePopup()
        {
            Title = "Rate us!";
            Content = "Your feedback helps us improve this app. If you like it, please take a minute and rate it with five stars so we can continue working on new features and updates.";
            RateButtonText = "rate 5 stars";
            CancelButtonText = "no, thanks";
            LaunchLimit = DefaultLaunchLimitForReminder;

            if (!ApplicationData.Current.LocalSettings.Containers.ContainsKey(UniversalRateReminderContainerName))
            {
                ResetLaunchCount();
            }

            reminderContainer = ApplicationData.Current.LocalSettings.Containers[UniversalRateReminderContainerName];
        }

        /// <summary>
        /// Increments the launch counter and if it is equal or less than the current value of <see cref="RatePopup.LaunchLimit"/>, shows the rating pop up. A flag will be set
        /// so the dialog only shows once.
        /// </summary>
        public static async Task<RateReminderResult> CheckRateReminderAsync()
        {
            // We need to check the app version and for the new version reset regardless if they said no on a prior version
            string currentAppversion = GetAppVersion();
            string storedAppVersion = (string)reminderContainer.Values[AppVersionPropertyName];
            if (currentAppversion != storedAppVersion && ResetCountOnNewVersion == true)
            {
                ResetLaunchCount();
            }

            if (((bool)reminderContainer.Values[DismissedPropertyName]) == false)
            {
                reminderContainer.Values[AppVersionPropertyName] = currentAppversion;

                int launchCount = (int)reminderContainer.Values[CountPropertyName];
                launchCount++;
                reminderContainer.Values[CountPropertyName] = launchCount;

                if (launchCount >= LaunchLimit)
                {
                    MessageDialog rateDialog = new MessageDialog(Content, Title);

                    var rateCommand = new UICommand(RateButtonText, (command) =>
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#if WINDOWS_UWP
                        Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName));
#else
                        bool runningOnPhone = true;

                        // Ugly hack for detecting running platform at runtime
                        try
                        {
                            object brush = Windows.UI.Xaml.Application.Current.Resources["PhoneAccentBrush"];
                        }
                        catch (Exception)
                        {
                            runningOnPhone = false;
                        }

                        if (runningOnPhone)
                        {
                            Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
                        }
                        else
                        {
                            Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName));
                        }
#endif
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        reminderContainer.Values[DismissedPropertyName] = true;
                    });

                    var dismissCommand = new UICommand(CancelButtonText, (command) =>
                    {
                        reminderContainer.Values[DismissedPropertyName] = true;
                    });

                    rateDialog.Commands.Add(rateCommand);
                    rateDialog.Commands.Add(dismissCommand);

                    rateDialog.CancelCommandIndex = 1;
                    rateDialog.DefaultCommandIndex = 0;

                    var rateResult = await rateDialog.ShowAsync();

                    if (rateResult == dismissCommand)
                    {
                        return RateReminderResult.Dismissed;
                    }

                    return RateReminderResult.Rated;
                }
            }

            return RateReminderResult.NotShown;
        }

        /// <summary>
        /// Resets the stored launch count to zero, and resets the flag that prevents the pop up from showing more than once.
        /// </summary>
        public static void ResetLaunchCount()
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey(UniversalRateReminderContainerName))
            {
                ApplicationData.Current.LocalSettings.DeleteContainer(UniversalRateReminderContainerName);
            }

            reminderContainer = ApplicationData.Current.LocalSettings.CreateContainer(UniversalRateReminderContainerName, ApplicationDataCreateDisposition.Always);
            reminderContainer.Values.Add(CountPropertyName, (int)0);
            reminderContainer.Values.Add(DismissedPropertyName, (bool)false);
        }

        /// <summary>
        /// Get's the application version
        /// </summary>
        public static string GetAppVersion()
        {
            PackageVersion pv = Package.Current.Id.Version;
            Version version = new Version(Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Revision,
                Package.Current.Id.Version.Build);
            string appVersion = version.Major + "." + version.Minor + "." + version.MinorRevision + "." + version.Build;
            return appVersion;
        }
    }
}
