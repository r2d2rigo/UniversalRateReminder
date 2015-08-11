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
        private const string DefaultContent = "We are sorry you don't want to rate the application. Would you like to send us some valuable feedback?";

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
        /// Static constructor for initializing default values.
        /// </summary>
        static FeedbackPopup()
        {
            Title = DefaultTitle;
            Content = DefaultContent;
            SendFeedbackButtonText = "rate 5 stars";
            CancelButtonText = "no, thanks";
        }

        /// <summary>
        /// Increments the launch counter and if it is equal or greater than the current value of <see cref="RatePopup.LaunchCount"/>, shows the rating pop up. A flag will be set
        /// so the dialog only shows once.
        /// </summary>
        public static async Task ShowFeedbackDialogAsync()
        {
            MessageDialog rateDialog = new MessageDialog(Content, Title);

            var feedbackCommand = new UICommand(SendFeedbackButtonText, (command) =>
            {
                // TODO: send email
            });

            var dismissCommand = new UICommand(CancelButtonText, (command) =>
            {
            });

            rateDialog.Commands.Add(feedbackCommand);
            rateDialog.Commands.Add(dismissCommand);

            rateDialog.CancelCommandIndex = 1;
            rateDialog.DefaultCommandIndex = 0;

            await rateDialog.ShowAsync();
        }
    }
}
