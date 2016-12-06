using System.Threading.Tasks;
using Android.App;
using Android.Views;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid.Platform;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace Chance.MvvmCross.Plugins.UserInteraction.Droid
{
    public class ShowDialogService : IShowDialogService
    {
        public Task<ConfirmThreeButtonsResponse> ShowAsync(string message, string title = null, View view = null, string positive = null, string negative = null, string neutral = null)
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

        public Activity CurrentActivity => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
    }
}