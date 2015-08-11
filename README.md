UniversalRateReminder
=====================

A popup rate reminder for Universal Windows/Windows Phone XAML apps.

What is it?
-----------

A simple class for showing a rate app reminder after the user has launched the application a specific number of times. Works for Windows 8.1 and Windows Phone 8.1 XAML Universal Apps.

![Rate reminder in Windows and Windows Phone side by side](/Docs/Screenshot.png?raw=true "Sample screenshot")

Installing
-----------

Get it from [NuGet](https://www.nuget.org/packages/UniversalRateReminder/) or search for the package named **UniversalRateReminder**.

How to use
-----------

Add the reference to both projects and at the end of the **OnLaunched** function of your **App** class, call

    RatePopup.CheckRateReminder();

This will increment the counter that stores the number of times that the app has launched and will show the pop up asking the user for rating.

You can customize the title, text, rate button text, cancel button text, if you want to reset the count on a new version, and number of times the application must run before showing the pop up; just assign them before calling **CheckRateReminder**:

    RatePopup.LaunchLimit = 1;
    RatePopup.ResetCountOnNewVersion = false;
    RatePopup.RateButtonText = "rate";
    RatePopup.CancelButtonText = "cancel";
    RatePopup.Title = "Rate app";
    RatePopup.Content = "Would you like to rate this app?";
    RatePopup.CheckRateReminder();

And if you want to reset the counter/and or show the pop up again even after the user has dismissed it, call **ResetLaunchCount**:

    RatePopup.ResetLaunchCount();
