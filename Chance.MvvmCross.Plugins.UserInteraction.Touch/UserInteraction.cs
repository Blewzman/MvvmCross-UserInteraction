using System;
using UIKit;
using System.Threading.Tasks;

namespace Chance.MvvmCross.Plugins.UserInteraction.Touch
{
    public class UserInteraction : IUserInteraction
    {
        public Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel")
        {
            var tcs = new TaskCompletionSource<bool>();

            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var confirm = new UIAlertView(
                    title ?? string.Empty, 
                    message,
                    null, 
                    cancelButton, 
                    okButton);

                confirm.Dismissed += (sender, args) => tcs.TrySetResult(confirm.CancelButtonIndex != args.ButtonIndex);
                confirm.Show();
            });

            return tcs.Task;
        }

        public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
        {
            var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();
            var confirm = new UIAlertView(title ?? string.Empty, message, null, negative, positive, neutral);

            confirm.Dismissed += (sender, args) =>
            {
                var buttonIndex = args.ButtonIndex;
                if (buttonIndex == confirm.CancelButtonIndex)
                    tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative);
                else if (buttonIndex == confirm.FirstOtherButtonIndex)
                    tcs.TrySetResult(ConfirmThreeButtonsResponse.Positive);
                else
                    tcs.TrySetResult(ConfirmThreeButtonsResponse.Neutral);
            };

            confirm.Show();

            return tcs.Task;
        }

        public Task AlertAsync(string message, string title = "", string okButton = "OK")
        {
            var tcs = new TaskCompletionSource<object>();

            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var alert = new UIAlertView(title ?? string.Empty, message, null, okButton);
                alert.Dismissed += (sender, args) => tcs.TrySetResult(null);
                alert.Show();
            });

            return tcs.Task;
        }

        public Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            var tcs = new TaskCompletionSource<InputResponse>();

            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var input = new UIAlertView(title ?? string.Empty, message, null, cancelButton, okButton);
                input.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
                var textField = input.GetTextField(0);
                textField.Placeholder = placeholder;
                textField.Text = initialText;

                input.Dismissed += (sender, args) => tcs.TrySetResult(new InputResponse { Ok = input.CancelButtonIndex != args.ButtonIndex, Text = textField.Text });
                input.Show();
            });

            return tcs.Task;
        }
    }
}
