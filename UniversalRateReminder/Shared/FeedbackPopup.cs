using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Popups;

namespace UniversalRateReminder
{
    /// <summary>
    /// A static class for showing a pop up for sending email feedback.
    /// </summary>
    public static class FeedbackPopup
    {
        /// <summary>
        /// Default title for feedback pop up.
        /// </summary>
        private const string DefaultTitle = "Send feedback?";

        /// <summary>
        /// Default content for feedback pop up.
        /// </summary>
        private const string DefaultContent = "We are sorry you don't want to rate the application. Would you like to send us an email with some valuable feedback?";

        /// <summary>
        /// The title for the feedback pop up. The default value is <see cref="FeedbackPopup.DefaultTitle"/>.
        /// </summary>
        public static string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The text content for the feedback pop up. The default value is <see cref="FeedbackPopup.DefaultContent"/>.
        /// </summary>
        public static string Content
        {
            get;
            set;
        }

        /// <summary>
        /// The text for the rate button. The default value is "rate 5 stars".
        /// </summary>
        public static string SendFeedbackButtonText
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
        /// The contact email for sending feedback.
        /// </summary>
        public static string ContactEmail
        {
            get;
            set;
        }

        /// <summary>
        /// The default email subject.
        /// </summary>
        public static string EmailSubject
        {
            get;
            set;
        }

        /// <summary>
        /// The default email body, if any.
        /// </summary>
        public static string EmailBody
        {
            get;
            set;
        }

        /// <summary>
        /// Static constructor for initializing default values.
        /// </summary>
        static FeedbackPopup()
        {
            Title = DefaultTitle;
            Content = DefaultContent;
            SendFeedbackButtonText = "rate 5 stars";
            CancelButtonText = "no, thanks";
            ContactEmail = string.Empty;
            EmailSubject = string.Empty;
            EmailBody = string.Empty;
        }

        /// <summary>
        /// Increments the launch counter and if it is equal or greater than the current value of <see cref="RatePopup.LaunchCount"/>, shows the rating pop up. A flag will be set
        /// so the dialog only shows once.
        /// </summary>
        public static async Task ShowFeedbackDialogAsync()
        {
            MessageDialog feedbackDialog = new MessageDialog(Content, Title);

            var feedbackCommand = new UICommand(SendFeedbackButtonText, (command) =>
            {
                if (string.IsNullOrWhiteSpace(ContactEmail))
                {
                    throw new InvalidOperationException("Please set an email address before calling this method.");
                }

                if (string.IsNullOrWhiteSpace(EmailSubject))
                {
                    throw new InvalidOperationException("Please set an email subject before calling this method.");
                }

                var mailtoUri = new Uri("mailto:?to=" + ContactEmail + "&subject=" + EmailSubject + "&body=" + EmailBody);

                var launchOptions = new LauncherOptions
                {
                    DisplayApplicationPicker = true,
                };

                Launcher.LaunchUriAsync(mailtoUri, launchOptions);
            });

            var dismissCommand = new UICommand(CancelButtonText, (command) =>
            {
            });

            feedbackDialog.Commands.Add(feedbackCommand);
            feedbackDialog.Commands.Add(dismissCommand);

            feedbackDialog.CancelCommandIndex = 1;
            feedbackDialog.DefaultCommandIndex = 0;

            await feedbackDialog.ShowAsync();
        }
    }
}
