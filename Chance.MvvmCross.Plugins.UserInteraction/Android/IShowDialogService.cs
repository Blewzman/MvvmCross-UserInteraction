using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Views;

namespace Chance.MvvmCross.Plugins.UserInteraction.Droid
{
    public interface IShowDialogService
    {
        Task<ConfirmThreeButtonsResponse> ShowAsync(string message, string title = null, View view = null, string positive = null, string negative = null, string neutral = null, CancellationToken ct = default(CancellationToken));
    }
}
