using System;
using Android.App;
using Cirrious.CrossCore;
using Android.Widget;
using Cirrious.CrossCore.Droid.Platform;
using System.Threading.Tasks;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace Chance.MvvmCross.Plugins.UserInteraction.Droid
{
    public class UserInteraction : IUserInteraction
    {
        protected Activity CurrentActivity
        {
            get { return Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity; }
        }

        public Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel")
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.SynchronizationContext.Post(
                ignored => 
                {
                    if (CurrentActivity == null)
                        tcs.TrySetCanceled();
                    else
                    {
                        new AlertDialog.Builder(CurrentActivity)
                            .SetMessage(message)
                            .SetTitle(title)
                            .SetPositiveButton(okButton, (s, e) => tcs.TrySetResult(true))
                            .SetNegativeButton(cancelButton, (s, e) => tcs.TrySetResult(false))
                            .Show();
                    }
                }, 
                null);

            return tcs.Task;
        }

        public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null, string positive = "Yes", string negative = "No",
            string neutral = "Maybe")
        {
            var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();

            Application.SynchronizationContext.Post(ignored =>
            {
                if (CurrentActivity == null)
                    tcs.TrySetCanceled();
                else
                {
                    new AlertDialog.Builder(CurrentActivity)
                        .SetMessage(message)
                        .SetTitle(title)
                        .SetPositiveButton(positive, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Positive))
                        .SetNegativeButton(negative, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative))
                        .SetNeutralButton(neutral, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Neutral))
                        .Show();
                }
            }, null);

            return tcs.Task;
        }

        public Task AlertAsync(string message, string title = "", string okButton = "OK")
        {
            var tcs = new TaskCompletionSource<object>();

            Application.SynchronizationContext.Post(ignored => 
            {
                if (CurrentActivity == null)
                    tcs.TrySetCanceled();
                else
                {
                    new AlertDialog.Builder(CurrentActivity)
                        .SetMessage(message)
                        .SetTitle(title)
                        .SetPositiveButton(okButton, (s, e) => tcs.TrySetResult(null))
                        .Show();
                }
            }, null);

            return tcs.Task;
        }

        public Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            var tcs = new TaskCompletionSource<InputResponse>();

            Application.SynchronizationContext.Post(
                ignored =>
                {
                    if (CurrentActivity == null)
                        tcs.TrySetCanceled();
                    else
                    {
                        var input = new EditText(CurrentActivity) { Hint = placeholder, Text = initialText };

                        new AlertDialog.Builder(CurrentActivity)
                            .SetMessage(message)
                            .SetTitle(title)
                            .SetView(input)
                            .SetPositiveButton(okButton, (s, e) => tcs.TrySetResult(new InputResponse { Ok = true, Text = input.Text }))
                            .SetNegativeButton(cancelButton, (s, e) => tcs.TrySetResult(new InputResponse { Ok = false, Text = input.Text }))
                            .Show();
                    }
                },
                null);

            return tcs.Task;
        }
    }
}

