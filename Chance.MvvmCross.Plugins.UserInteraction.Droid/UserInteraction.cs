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

        public void Confirm(string message, Action okClicked, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            this.Confirm(
                message, 
                confirmed =>
                {
                    if (confirmed)
                        okClicked();
                },
                title, 
                okButton, 
                cancelButton);
        }

        public void Confirm(string message, Action<bool> answer, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            var task = this.ConfirmAsync(
                message, 
                title, 
                okButton, 
                cancelButton);

            if (answer != null)
                task.ContinueWith((closureTask, closureAction) => ((Action<bool>)closureAction)(closureTask.Result), answer, TaskContinuationOptions.OnlyOnRanToCompletion);
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

        public void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
        {
            var task = this.ConfirmThreeButtonsAsync(
                message,
                title,
                positive,
                negative,
                neutral);

            if (answer != null)
                task.ContinueWith((closureTask, closureAction) => ((Action<ConfirmThreeButtonsResponse>)closureAction)(closureTask.Result), answer, TaskContinuationOptions.OnlyOnRanToCompletion);
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

        public void Alert(string message, Action done = null, string title = "", string okButton = "OK")
        {
            var task = this.AlertAsync(
                message,
                title,
                okButton);

            if (done != null)
                task.ContinueWith((closureTask, closureAction) => ((Action)closureAction)(), done, TaskContinuationOptions.OnlyOnRanToCompletion);
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

        public void Input(string message, Action<string> okClicked, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            this.Input(
                message, 
                (ok, text) => 
                {
                    if (ok)
                        okClicked(text);
                },
                placeholder, 
                title, 
                okButton, 
                cancelButton, 
                initialText);
        }

        public void Input(string message, Action<bool, string> answer, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            var task = this.InputAsync(message, placeholder, title, okButton, cancelButton, initialText);

            if (answer != null)
            {
                task.ContinueWith(
                    (closureTask, closureAction) =>
                    {
                        var response = closureTask.Result;

                        ((Action<bool, string>)closureAction)(response.Ok, response.Text);
                    },
                    answer,
                    TaskContinuationOptions.OnlyOnRanToCompletion);
            }
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

