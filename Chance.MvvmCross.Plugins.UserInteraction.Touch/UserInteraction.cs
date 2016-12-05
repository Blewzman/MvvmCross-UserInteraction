using System;
using UIKit;
using System.Threading.Tasks;

namespace Chance.MvvmCross.Plugins.UserInteraction.Touch
{
	public class UserInteraction : IUserInteraction
	{
		public void Confirm(string message, Action okClicked, string title = "", string okButton = "OK", string cancelButton = "Cancel")
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

		public void Confirm(string message, Action<bool> answer, string title = "", string okButton = "OK", string cancelButton = "Cancel")
		{
            var task = this.ConfirmAsync(message, title, okButton, cancelButton);

            if (answer != null)
            {
                task.ContinueWith(
                    (closureTask, closureAction) => ((Action<bool>)closureAction)(closureTask.Result),
                    answer,
                    TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }

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

		public void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
		{
            var task = this.ConfirmThreeButtonsAsync(message, title, positive, negative, neutral);

			if (answer != null)
			{
                task.ContinueWith(
                   (closureTask, closureAction) => ((Action<ConfirmThreeButtonsResponse>)closureAction)(closureTask.Result),
                   answer,
                   TaskContinuationOptions.OnlyOnRanToCompletion);
			}
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

		public void Alert(string message, Action done = null, string title = "", string okButton = "OK")
		{
            var task = this.AlertAsync(message, title, okButton);
            if (done != null)
            {
                task.ContinueWith(
                   (closureTask, closureAction) => ((Action)closureAction)(),
                   done,
                   TaskContinuationOptions.OnlyOnRanToCompletion);
            }
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
            var task = this.InputAsync(message, title, okButton);
            if (answer != null)
            {
                task.ContinueWith(
                   (closureTask, closureAction) => ((Action<bool, string>)closureAction)(closureTask.Result.Ok, closureTask.Result.Text),
                   answer,
                   TaskContinuationOptions.OnlyOnRanToCompletion);
            }
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
