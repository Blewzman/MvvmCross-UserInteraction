using System.Linq;
using System.Threading;
using Android.Widget;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace Chance.MvvmCross.Plugins.UserInteraction.Droid
{
    public class UserInteraction : IUserInteraction
    {
        private readonly IShowDialogService _showDialogService;

        public UserInteraction(IShowDialogService showDialogService)
        {
            this._showDialogService = showDialogService;
        }

        public async Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel", CancellationToken ct = default(CancellationToken))
        {
            return await this._showDialogService.ShowAsync(message, title, null, okButton, cancelButton, null, ct).ConfigureAwait(true) == ConfirmThreeButtonsResponse.Positive;
        }

        public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe", CancellationToken ct = default(CancellationToken))
        {
            return this._showDialogService.ShowAsync(message, title, null, positive, negative, neutral, ct);
        }

        public Task AlertAsync(string message, string title = "", string okButton = "OK", CancellationToken ct = default(CancellationToken))
        {
            return this._showDialogService.ShowAsync(message, title, null, okButton, null, null, ct);
        }

        public async Task<InputResponse> InputTextAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null, CancellationToken ct = default(CancellationToken))
        {
            var context = this.GetContextOrThrow();

            var input = new EditText(context)
            {
                Hint = placeholder,
                Text = initialText
            };

            return new InputResponse
            {
                Ok = await this._showDialogService.ShowAsync(message, title, input, okButton, cancelButton, null, ct).ConfigureAwait(true) == ConfirmThreeButtonsResponse.Positive,
                Text = input.Text
            };
        }

        public async Task<InputResponse> InputNumberAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null, CancellationToken ct = default(CancellationToken))
        {
            var context = this.GetContextOrThrow();

            var input = new EditText(context)
            {
                Hint = placeholder,
                Text = initialText,
                InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal
            };

            return new InputResponse
            {
                Ok = await this._showDialogService.ShowAsync(message, title, input, okButton, cancelButton,  null, ct).ConfigureAwait(true) == ConfirmThreeButtonsResponse.Positive,
                Text = input.Text
            };
        }

        public async Task<int?> ChooseSingleAsync(string message, string[] options, int? chosenItem = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken ct = default(CancellationToken))
        {
            var context = this.GetContextOrThrow();

            var radioButtons = options
                .Select((option, i) =>
                {
                    var checkBox = new RadioButton(context)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                        {
                            Gravity = GravityFlags.CenterVertical
                        },

                        Id = i + 1,
                        Text = option,
                        Gravity = GravityFlags.Center,
                    };

                    checkBox.SetTextColor(Color.White);

                    return checkBox;
                })
                .ToArray();

            var radioGroup = new RadioGroup(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
                Orientation = Orientation.Vertical
            };

            foreach (var optionLayout in radioButtons)
            {
                radioGroup.AddView(optionLayout);
            }

            var scrollView = new ScrollView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
            };

            scrollView.AddView(radioGroup);

            return await this._showDialogService.ShowAsync(message, title, scrollView, okButton, cancelButton, null, ct).ConfigureAwait(true) == ConfirmThreeButtonsResponse.Positive
                ? (radioGroup.CheckedRadioButtonId > 0 ? radioGroup.CheckedRadioButtonId - 1 : (int?) null)
                : null;
        }

        public async Task<int[]> ChooseMultipleAsync(string message, string[] options, int[] selectedOptions, string title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken ct = default(CancellationToken))
        {
            CheckBox[] checkBoxes = null;
            var context = this.GetContextOrThrow();

            checkBoxes = options
                .Select(x =>
                {
                    var checkBox = new CheckBox(context)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                        {
                            Gravity = GravityFlags.CenterVertical
                        },

                        Gravity = GravityFlags.Center,
                    };

                    checkBox.SetTextColor(Color.White);

                    return checkBox;
                })
                .ToArray();

            var optionLayouts = options
                .Select((option, i) =>
                {
                    var optionLayout = new LinearLayout(context)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
                        Orientation = Orientation.Horizontal
                    };

                    optionLayout.AddView(checkBoxes[i]);
                    optionLayout.AddView(new TextView(context)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent)
                        {
                            Gravity = GravityFlags.CenterVertical,
                            LeftMargin = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 8, context.Resources.DisplayMetrics)
                        },

                        Text = option
                    });

                    return optionLayout;
                })
                .ToArray();

            var linearLayout = new LinearLayout(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
                Orientation = Orientation.Vertical
            };

            foreach (var optionLayout in optionLayouts)
            {
                linearLayout.AddView(optionLayout);
            }

            var scrollView = new ScrollView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
            };

            scrollView.AddView(linearLayout);

            return await this._showDialogService.ShowAsync(message, title, scrollView, okButton, cancelButton, null, ct).ConfigureAwait(true) == ConfirmThreeButtonsResponse.Positive
                ? options.Select((x, i) => checkBoxes[i].Checked ? i : -1).Where(x => x != -1).ToArray() 
                : new int[0];
        }

        private Context GetContextOrThrow()
        {
            var ret = this._showDialogService.CurrentActivity;
            if (ret == null)
                throw new TaskCanceledException();

            return ret;
        }
    }
}

