using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chance.MvvmCross.Plugins.UserInteraction
{
    public static class UserInteractionExtensions
    {
        public static void Confirm(this IUserInteraction userInteraction, string message, Action okClicked, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            userInteraction.Confirm(
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

        public static void Confirm(this IUserInteraction userInteraction, string message, Action<bool> answer, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            var task = userInteraction.ConfirmAsync(
                message,
                title,
                okButton,
                cancelButton);

            if (answer != null)
                task.ContinueWith((closureTask, closureAction) => ((Action<bool>)closureAction)(closureTask.Result), answer, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public static void Alert(this IUserInteraction userInteraction, string message, string title = "", string okButton = "OK", Action done = null)
        {
            var task = userInteraction.AlertAsync(
                message,
                title,
                okButton);

            if (done != null)
                task.ContinueWith((closureTask, closureAction) => ((Action)closureAction)(), done, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public static void Input(this IUserInteraction userInteraction, string message, Action<string> okClicked, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            userInteraction.Input(
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

        public static void Input(this IUserInteraction userInteraction, string message, Action<bool, string> answer, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            var task = userInteraction.InputTextAsync(message, placeholder, title, okButton, cancelButton, initialText);

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

        public static void ConfirmThreeButtons(this IUserInteraction userInteraction, string message, Action<ConfirmThreeButtonsResponse> answer, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
        {
            var task = userInteraction.ConfirmThreeButtonsAsync(
                message,
                title,
                positive,
                negative,
                neutral);

            if (answer != null)
                task.ContinueWith((closureTask, closureAction) => ((Action<ConfirmThreeButtonsResponse>)closureAction)(closureTask.Result), answer, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
