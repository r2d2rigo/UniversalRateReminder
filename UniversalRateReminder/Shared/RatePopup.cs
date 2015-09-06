using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.ApplicationModel.Email;

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
        /// If the users dismisses the rate dialog, displays a second one asking for feedback
        /// </summary>
        public static bool AskForFeedback
        {
            get;
            set;
        }

        /// <summary>
        /// Contact email address for sending feedback
        /// </summary>
        public static string ContactEmail
        {
            get;
            set;
        }

        /// <summary>
        /// The title for the feedback pop up. The default value is "Help us!".
        /// </summary>
        public static string FeedbackTitle
        {
            get;
            set;
        }

        /// <summary>
        /// The text content for the feedback pop up. The default value is "Your feedback helps you improve this app. If you like it, please take a minute and tell us what we can improve.".
        /// </summary>
        public static string FeedbackContent
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
        /// The text for the feedback button. The default value is "send".
        /// </summary>
        public static string FeedbackButtonText
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
        /// The message written in the feedback email. You can include the app version number and device info
        // </summary>
        public static string FeedbackEmailMessage 
        {
            get;
            set;
        }
        
        /// <summary>
        /// The subject of the feedback email. The default value is "Feedback".
        // </summary>
        public static string FeedbackEmailSubject
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
            FeedbackButtonText = "send":
            FeedbackEmailSubject = "Feedback";
            FeedbackTitle = "Help us!";
            FeedbackContent="Your feedback helps you improve this app. If you like it, please take a minute and tell us what we can improve.";
            LaunchLimit = DefaultLaunchLimitForReminder;

            if (!ApplicationData.Current.LocalSettings.Containers.ContainsKey(UniversalRateReminderContainerName))
            {
                ResetLaunchCount();
            }

            reminderContainer = ApplicationData.Current.LocalSettings.Containers[UniversalRateReminderContainerName];
        }

        /// <summary>
        /// Increments the launch counter and if it is equal or greater than the current value of <see cref="RatePopup.LaunchCount"/>, shows the rating pop up. A flag will be set
        /// so the dialog only shows once.
        /// </summary>
        public static async Task CheckRateReminderAsync()
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
#if WINDOWS_UWP
                        Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=" + Package.Current.Id.FamilyName));
#else
                        bool runningOnPhone = true;

                        // Ugly hack for detecting running platform at runtime
                        try
                        {
                            object brush = Windows.UI.Xaml.Application.Current.Resources["PhoneAccentBrush"];
                        }
                        catch (Exception e)
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

                    if (rateResult == dismissCommand && AskForFeedback)
                    {
                        MessageDialog feedbackDialog = new MessageDialog(FeedbackContent, FeedbackTitle);

                        var sendCommand = new UICommand(FeedbackButtonText, (command) =>
                        {
                            EmailRecipient sendTo = new EmailRecipient()
                            {
                            Address = ContactEmail;
                            };
                            //generate mail object
                            EmailMessage mail = new EmailMessage();
                            mail.Subject = FeedbackEmailSubject;
                            //add recipients to the mail object
                            mail.To.Add(sendTo);
                            //open the share contract with Mail only:
                            await EmailManager.ShowComposeNewEmailAsync(mail);
                        });
                        
                        var dismissCommand = new UICommand(CancelButtonText, (command) => 
                        {
                        });
                        
                        feedbackDialog.Commands.Add(sendCommand);
                        feedbackDialog.Commands.Add(dismissCommand);
                        
                        feedbackDialog.CancelCommandIndex = 1;
                        feedbackDialog.DefaultCommandIndex = 0;
                        
                        var feedbackResult = await feedbackDialog.ShowAsync();
                    }
                }
            }
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
