UniversalRateReminder
=====================

A popup rate reminder for Windows 10 UWP and Universal Windows/Windows Phone 8.1 XAML apps.

What is it?
-----------

A simple class for showing a rate app reminder after the user has launched the application a specific number of times. Works for Windows 10 UWP, Windows 8.1 and Windows Phone 8.1 XAML Universal Apps.

![Rate reminder in Windows and Windows Phone side by side](/Docs/Screenshot.png?raw=true "Sample screenshot")

Installing
-----------

Get it from [NuGet](https://www.nuget.org/packages/UniversalRateReminder/) or search for the package named **UniversalRateReminder**.

How to use
-----------

Add the reference to both projects and at the end of the **OnLaunched** function of your **App** class, call

    RatePopup.CheckRateReminderAsync();

This will increment the counter that stores the number of times that the app has launched and will show the pop up asking the user for rating.

You can customize the title, text, rate button text, cancel button text, if you want to reset the count on a new version, and number of times the application must run before showing the pop up; just assign them before calling **RatePopup.CheckRateReminderAsync**:

    RatePopup.LaunchLimit = 1;
    RatePopup.ResetCountOnNewVersion = false;
    RatePopup.RateButtonText = "rate";
    RatePopup.CancelButtonText = "cancel";
    RatePopup.Title = "Rate app";
    RatePopup.Content = "Would you like to rate this app?";
    RatePopup.CheckRateReminderAsync();

Note that `CheckRateReminderAsync` will return different values based on how the user interacted with the popup:
- `RateReminderResult.NotShown`: the popup wasn't displayed because the app launch count wasn't reached.
- `RateReminderResult.Rated`: the user accepted to rate the app.
- `RateReminderResult.Dismissed`: the user canceled or dismissed the rate popup.
	
And if you want to reset the counter/and or show the pop up again even after the user has dismissed it, call **ResetLaunchCount**:

    RatePopup.ResetLaunchCount();

There is a feedback popup too - you can show this for asking the user for feedback, for example when he/she has declined to rate the app (`RatePopup.CheckRateReminderAsync` returned `RateReminderResult.Dismissed`), and it's customizable too:

	FeedbackPopup.ContactEmail = "contact@yourdomain.com";
	FeedbackPopup.EmailSubject = "Feedback for my app";
	FeedbackPopup.EmailBody = "Default body";
	FeedbackPopup.Title = "Would you like to send feedback?";
	FeedbackPopup.Content = "Maybe you want to send us an email with feedback regarding your experience with the app?";
	FeedbackPopup.SendFeedbackButtonText = "yes, send feedback";
	FeedbackPopup.CancelButtonText = "no, thanks";
	await FeedbackPopup.ShowFeedbackDialogAsync();