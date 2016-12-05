using Android.App;
using Cirrious.CrossCore;
using Android.Widget;
using Cirrious.CrossCore.Droid.Platform;
using System.Threading.Tasks;
using Android.Views;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace Chance.MvvmCross.Plugins.UserInteraction.Droid
{
    public class UserInteraction : IUserInteraction
    {
        protected Activity CurrentActivity
        {
            get { return Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity; }
        }

        public async Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel")
        {
            return await this.ShowAsync(message, title, null, okButton, cancelButton) == ConfirmThreeButtonsResponse.Positive;
        }

        public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
        {
            return this.ShowAsync(message, title, null, positive, negative, neutral);
        }

        public Task AlertAsync(string message, string title = "", string okButton = "OK")
        {
            return this.ShowAsync(message, title, null, okButton);
        }

        public async Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            var input = new EditText(CurrentActivity)
            {
                Hint = placeholder,
                Text = initialText
            };

            return new InputResponse
            {
                Ok = await this.ShowAsync(message, title, input, okButton, cancelButton) == ConfirmThreeButtonsResponse.Positive,
                Text = input.Text
            };
        }

        private Task<ConfirmThreeButtonsResponse> ShowAsync(string message, string title = null, View view = null, string positive = null, string negative = null, string neutral = null)
        {
            var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();

            Application.SynchronizationContext.Post(ignored =>
            {
                if (CurrentActivity == null)
                    tcs.TrySetCanceled();
                else
                {
                    var builder = new AlertDialog.Builder(CurrentActivity)
                        .SetMessage(message)
                        .SetTitle(title);

                    if (view != null)
                        builder = builder.SetView(view);

                    if (positive != null)
                        builder = builder.SetPositiveButton(positive, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Positive));

                    if (negative != null)
                        builder = builder.SetNegativeButton(negative, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative));

                    if (neutral != null)
                        builder = builder.SetNeutralButton(neutral, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Neutral));

                    builder
                        .Show();
                }
            }, null);

            return tcs.Task;
        }
    }
}

