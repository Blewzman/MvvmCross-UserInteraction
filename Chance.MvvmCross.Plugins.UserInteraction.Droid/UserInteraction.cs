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
        private readonly IShowDialogService _showDialogService;

        public UserInteraction(IShowDialogService showDialogService)
        {
            this._showDialogService = showDialogService;
        }

        public async Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel")
        {
            return await this._showDialogService.ShowAsync(message, title, null, okButton, cancelButton) == ConfirmThreeButtonsResponse.Positive;
        }

        public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
        {
            return this._showDialogService.ShowAsync(message, title, null, positive, negative, neutral);
        }

        public Task AlertAsync(string message, string title = "", string okButton = "OK")
        {
            return this._showDialogService.ShowAsync(message, title, null, okButton);
        }

        public async Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            var input = new EditText(this._showDialogService.CurrentActivity)
            {
                Hint = placeholder,
                Text = initialText
            };

            return new InputResponse
            {
                Ok = await this._showDialogService.ShowAsync(message, title, input, okButton, cancelButton) == ConfirmThreeButtonsResponse.Positive,
                Text = input.Text
            };
        }
    }
}

