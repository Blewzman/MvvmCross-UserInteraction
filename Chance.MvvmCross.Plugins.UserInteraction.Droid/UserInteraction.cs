using System;
using System.Linq;
using Android.Widget;
using System.Threading.Tasks;
using Android.App;
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

        public async Task<InputResponse> InputTextAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
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

        public async Task<InputResponse> InputNumberAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
        {
            var input = new EditText(this._showDialogService.CurrentActivity)
            {
                Hint = placeholder,
                Text = initialText,
                InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal
            };

            return new InputResponse
            {
                Ok = await this._showDialogService.ShowAsync(message, title, input, okButton, cancelButton) == ConfirmThreeButtonsResponse.Positive,
                Text = input.Text
            };
        }

        public async Task<int?> ChooseSingleAsync(string message, string[] options, int? chosenItem = null, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            var radioButtons = options
                .Select((option, i) =>
                {
                    var checkBox = new RadioButton(this._showDialogService.CurrentActivity)
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

            var radioGroup = new RadioGroup(this._showDialogService.CurrentActivity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
                Orientation = Orientation.Vertical
            };

            foreach (var optionLayout in radioButtons)
            {
                radioGroup.AddView(optionLayout);
            }

            var scrollView = new ScrollView(this._showDialogService.CurrentActivity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
            };

            scrollView.AddView(radioGroup);

            return await this._showDialogService.ShowAsync(message, title, scrollView, okButton, cancelButton) == ConfirmThreeButtonsResponse.Positive
                ? (radioGroup.CheckedRadioButtonId > 0 ? radioGroup.CheckedRadioButtonId - 1 : (int?) null)
                : null;
        }

        public async Task<int[]> ChooseMultipleAsync(string message, string[] options, int[] selectedOptions, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        {
            CheckBox[] checkBoxes = null;

            checkBoxes = options
                .Select(x =>
                {
                    var checkBox = new CheckBox(this._showDialogService.CurrentActivity)
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
                    var optionLayout = new LinearLayout(this._showDialogService.CurrentActivity)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
                        Orientation = Orientation.Horizontal
                    };

                    optionLayout.AddView(checkBoxes[i]);
                    optionLayout.AddView(new TextView(this._showDialogService.CurrentActivity)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent)
                        {
                            Gravity = GravityFlags.CenterVertical,
                            LeftMargin = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 8, this._showDialogService.CurrentActivity.Resources.DisplayMetrics)
                        },

                        Text = option
                    });

                    return optionLayout;
                })
                .ToArray();

            var linearLayout = new LinearLayout(this._showDialogService.CurrentActivity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
                Orientation = Orientation.Vertical
            };

            foreach (var optionLayout in optionLayouts)
            {
                linearLayout.AddView(optionLayout);
            }

            var scrollView = new ScrollView(this._showDialogService.CurrentActivity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent),
            };

            scrollView.AddView(linearLayout);

            return await this._showDialogService.ShowAsync(message, title, scrollView, okButton, cancelButton) == ConfirmThreeButtonsResponse.Positive
                ? options.Select((x, i) => checkBoxes[i].Checked ? i : -1).Where(x => x != -1).ToArray() 
                : new int[0];
        }
    }
}

