using System.Threading;
using System.Threading.Tasks;

namespace Chance.MvvmCross.Plugins.UserInteraction
{
    public interface IUserInteraction
    {
        Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel", CancellationToken ct = default(CancellationToken));

        Task AlertAsync(string message, string title = "", string okButton = "OK", CancellationToken ct = default(CancellationToken));

        Task<InputResponse> InputTextAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null, CancellationToken ct = default(CancellationToken));

        Task<InputResponse> InputNumberAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null, CancellationToken ct = default(CancellationToken));

        Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe", CancellationToken ct = default(CancellationToken));

        Task<int?> ChooseSingleAsync(string message, string[] options, int? chosenItem = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken ct = default(CancellationToken));

        Task<int[]> ChooseMultipleAsync(string message, string[] options, int[] selectedOptions, string title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken ct = default(CancellationToken));
    }
}

