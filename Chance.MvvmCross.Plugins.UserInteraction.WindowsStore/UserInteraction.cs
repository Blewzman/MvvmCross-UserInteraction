using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Popups;
using WinRTXamlToolkit.Controls;

namespace Chance.MvvmCross.Plugins.UserInteraction.WindowsStore
{
    public class UserInteraction : IUserInteraction
    {
        public async Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel")
        {
            var result = false;
            var box = new MessageDialog(message, title);

            box.Commands.Add(new UICommand(okButton, delegate { result = true; }));
            box.Commands.Add(new UICommand(cancelButton, delegate { result = false; }));

            await box.ShowAsync();

            return result;
        }

        public async Task AlertAsync(string message, string title = "", string okButton = "OK")
        {
            var box = new MessageDialog(message, title);
            
            box.Commands.Add(new UICommand(okButton));

            await box.ShowAsync();
        }

        public async Task<InputResponse> InputTextAsync(string message, string placeholder = null, string title = null, string okButton = "OK",
            string cancelButton = "Cancel", string initialText = null)
        {
            var box = new InputDialog { InputText = initialText ?? string.Empty };
            var result = await box.ShowAsync(title ?? string.Empty, message, okButton, cancelButton);
            return new InputResponse() {Text = box.InputText, Ok = result == okButton};
        }

        public async Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
        {
            ConfirmThreeButtonsResponse result = ConfirmThreeButtonsResponse.Neutral;
            var box = new MessageDialog(message, title ?? string.Empty);

            box.Commands.Add(new UICommand(positive, delegate { result = ConfirmThreeButtonsResponse.Positive; }));
            box.Commands.Add(new UICommand(neutral, delegate { result = ConfirmThreeButtonsResponse.Neutral; }));
            box.Commands.Add(new UICommand(negative, delegate { result = ConfirmThreeButtonsResponse.Negative; }));

            await box.ShowAsync();

            return result;
        }

        public Task<int?> ChooseSingleAsync(string message, string[] options, int? chosenItem = null, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            throw new NotImplementedException();
        }

        public Task<int[]> ChooseMultipleAsync(string message, string[] options, int[] selectedOptions, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            throw new NotImplementedException();
        }

        public Task<InputResponse> InputNumberAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            throw new NotImplementedException();
        }
    }
}