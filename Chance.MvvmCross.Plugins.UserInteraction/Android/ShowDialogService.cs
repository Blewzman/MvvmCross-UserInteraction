using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace Chance.MvvmCross.Plugins.UserInteraction.Droid
{
    public class ShowDialogService : IShowDialogService
    {
        public Task<ConfirmThreeButtonsResponse> ShowAsync(string message, string title = null, View view = null, string positive = null, string negative = null, string neutral = null, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();

            Application.SynchronizationContext.Post(ignored =>
            {
                if (CurrentActivity == null)
                    tcs.TrySetCanceled();
                else
                {
                    var html = Build.VERSION.SdkInt >= BuildVersionCodes.N
                        ? Html.FromHtml(message, FromHtmlOptions.ModeLegacy)
                        #pragma warning disable CS0618 // Type or member is obsolete
                        : Html.FromHtml(message);
                        #pragma warning restore CS0618 // Type or member is obsolete

                    var builder = this.CreateBuilder()
                        .SetMessage(html)
                        .SetTitle(title);

                    if (view != null)
                        builder = builder.SetView(view);

                    if (positive != null)
                        builder = builder.SetPositiveButton(positive, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Positive));

                    if (negative != null)
                        builder = builder.SetNegativeButton(negative, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative));

                    if (neutral != null)
                        builder = builder.SetNeutralButton(neutral, (s, e) => tcs.TrySetResult(ConfirmThreeButtonsResponse.Neutral));

                    var dialog = builder
                        .Create();

                    if (ct.CanBeCanceled)
                    {
                        var subscription = ct.Register(() =>
                        {
                            Application.SynchronizationContext.Post(ignored2 => dialog.SafeDismiss(), null);
                            tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative);
                        });

                        // ReSharper disable once MethodSupportsCancellation
                        tcs.Task.ContinueWith(_ => subscription.Dispose());
                    }

                    dialog.Show();

                    if (dialog.FindViewById(Android.Resource.Id.Message) is TextView messageTextView)
                        messageTextView.MovementMethod = LinkMovementMethod.Instance;
                }
            }, null);

            return tcs.Task;
        }

        protected virtual AlertDialog.Builder CreateBuilder()
        {
            return new AlertDialog.Builder(this.CurrentActivity);
        }

        public Activity CurrentActivity => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
    }
}