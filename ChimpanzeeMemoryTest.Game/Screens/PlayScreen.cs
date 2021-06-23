using ChimpanzeeMemoryTest.Game.UI;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using System.Linq;

namespace ChimpanzeeMemoryTest.Game.Screens
{
    internal class PlayScreen : Screen
    {
        private const int collapsed_settings_height = 120;
        private const int expanded_settings_height = 170;
        private readonly Container gridContainer;
        private readonly CMTButton button;
        private readonly SpriteText sizeText;
        private readonly FillFlowContainer leftSettings;
        private readonly FillFlowContainer rightSettings;
        private readonly SpriteText amountOfNumbersText;
        private readonly BindableInt visibleBoxesBindable = new(1)
        {
            MinValue = 0,
            MaxValue = 2,
        };
        private readonly SpriteText visibleBoxesText;
        private readonly SpriteText previewText;
        private readonly CMTButton restartButton;
        private readonly FillFlowContainer bottomContainer;
        private readonly SpriteText roundsAmountText;
        private readonly SpriteText currentRoundText;
        private readonly SpriteText roundResultText;
        private readonly TextFlowContainer resultsText;
        private readonly SpriteText timeToMemorizeText;
        private readonly BindableFloat displayScale = new(1)
        {
            MinValue = 0.1f,
            MaxValue = 1,
            Precision = 0.05f
        };
        private readonly SpriteText displayScaleText;
        private readonly Container gridBorder;

        private Grid grid { get; } = new Grid();

        public PlayScreen()
        {
            Padding = new MarginPadding(20);
            InternalChildren = new Drawable[]
            {
                gridContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        previewText = new SpriteText
                        {
                            Text = "preview",
                            Font = new FontUsage("OpenSans-Bold", 86),
                            Rotation = 15,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Depth = -1
                        },
                        resultsText = new TextFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Alpha = 0,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    }
                },
                roundResultText = new SpriteText
                {
                    Font = new FontUsage("OpenSans-Bold", 86),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Depth = -1
                },
                bottomContainer = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    RelativePositionAxes = Axes.Y,
                    Height = collapsed_settings_height,
                    Direction = FillDirection.Horizontal,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Padding = new MarginPadding(20),
                    Spacing = new osuTK.Vector2(20),
                    Children = new Drawable[]
                    {
                        leftSettings = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.33f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Spacing = new osuTK.Vector2(20),
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 30,
                                    Children = new Drawable[]
                                    {
                                        new BasicSliderBar<int>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 30,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Current = grid.SizeBindable,
                                            BackgroundColour = FrameworkColour.BlueDark,
                                            SelectionColour = FrameworkColour.BlueGreen
                                        },
                                        sizeText = new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 30,
                                    Children = new Drawable[]
                                    {
                                        new BasicSliderBar<int>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 30,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Current = grid.AmountOfNumbers,
                                            BackgroundColour = FrameworkColour.BlueDark,
                                            SelectionColour = FrameworkColour.BlueGreen
                                        },
                                        amountOfNumbersText = new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 30,
                                    Children = new Drawable[]
                                    {
                                        new BasicSliderBar<float>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 30,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Current = grid.TimeToMemorize,
                                            BackgroundColour = FrameworkColour.BlueDark,
                                            SelectionColour = FrameworkColour.BlueGreen
                                        },
                                        timeToMemorizeText = new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                        }
                                    }
                                }
                            }
                        },
                        button = new CMTButton
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.34f,
                            Masking = true,
                            Action = OnButtonClicked,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        },
                        restartButton = new CMTButton
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.34f,
                            Masking = true,
                            Action = grid.Retry,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = "Try again",
                        },
                        rightSettings = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.33f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Spacing = new osuTK.Vector2(20),
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 30,
                                    Children = new Drawable[]
                                    {
                                        new BasicSliderBar<int>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 30,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Current = visibleBoxesBindable,
                                            BackgroundColour = FrameworkColour.BlueDark,
                                            SelectionColour = FrameworkColour.BlueGreen
                                        },
                                        visibleBoxesText = new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 30,
                                    Children = new Drawable[]
                                    {
                                        new BasicSliderBar<int>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 30,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Current = grid.RoundsAmount,
                                            BackgroundColour = FrameworkColour.BlueDark,
                                            SelectionColour = FrameworkColour.BlueGreen
                                        },
                                        roundsAmountText = new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 30,
                                    Children = new Drawable[]
                                    {
                                        new BasicSliderBar<float>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 30,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Current = displayScale,
                                            BackgroundColour = FrameworkColour.BlueDark,
                                            SelectionColour = FrameworkColour.BlueGreen
                                        },
                                        displayScaleText = new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                        }
                                    }
                                },
                            }
                        }
                    }
                },
                currentRoundText = new SpriteText()
            };
            grid.State.BindValueChanged(OnGridStateChange, true);
            grid.SizeBindable.BindValueChanged(vc => sizeText.Text = $"Size: {vc.NewValue}x{vc.NewValue}", true);
            grid.AmountOfNumbers.BindValueChanged(vc => amountOfNumbersText.Text = $"Numbers: {vc.NewValue}", true);
            visibleBoxesBindable.BindValueChanged(OnVisibleBoxesSettingChange, true);
            grid.RoundsAmount.BindValueChanged(vc => roundsAmountText.Text = $"Rounds: {vc.NewValue}", true);
            grid.TimeToMemorize.BindValueChanged(vc => timeToMemorizeText.Text = $"Memorization time limit: {(vc.NewValue == 0 ? "infinite" : vc.NewValue.ToString() + "s")}", true);
            displayScale.BindValueChanged(vc =>
            {
                grid.Drawable.Scale = new osuTK.Vector2(vc.NewValue);
                displayScaleText.Text = $"Grid scale: {vc.NewValue:0.00}";
            }, true);


            gridContainer.Add(gridBorder = new Container
            {
                Masking = true,
                BorderColour = FrameworkColour.Blue,
                BorderThickness = 2,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                MaskingSmoothness = 0,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true
                }
            });
            gridContainer.Add(grid.Drawable);
        }

        private void OnVisibleBoxesSettingChange(ValueChangedEvent<int> obj)
        {
            var s = string.Empty;
            switch (obj.NewValue)
            {
                case 0:
                    s = "none";
                    break;
                case 1:
                    s = "with numbers";
                    break;
                case 2:
                    s = "all";
                    break;
            }
            visibleBoxesText.Text = $"Visible boxes: {s}";
            grid.VisibleBoxes.Value = (BoxVisibility)obj.NewValue;
        }

        private void OnGridStateChange(ValueChangedEvent<GridState> obj) => UpdateLayout();

        private void OnButtonClicked()
        {
            grid.Proceed();
            if (grid.State.Value == GridState.Playing)
                button.Hide();
        }

        private void UpdateLayout()
        {
            restartButton.Hide();
            const int settings_resize_duration = 0;
            currentRoundText.Text = $"{grid.CurrentRound}/{grid.RoundsAmount.Value} ({grid.State.Value})";
            resultsText.Hide();
            switch (grid.State.Value)
            {
                case GridState.NotReady:
                    button.Text = "Generate grid";
                    button.Show();
                    button.Action = grid.Proceed;
                    leftSettings.Show();
                    rightSettings.Show();
                    previewText.FadeTo(0.3f);
                    bottomContainer.ResizeHeightTo(expanded_settings_height, settings_resize_duration, Easing.OutQuint);
                    grid.Drawable.Show();
                    break;
                case GridState.GeneratedAndWaiting:
                    button.Text = "Start by clicking the first box";
                    button.Show();
                    button.Action = null;
                    leftSettings.Hide();
                    rightSettings.Hide();
                    previewText.Hide();
                    bottomContainer.ResizeHeightTo(collapsed_settings_height, settings_resize_duration, Easing.OutQuint);
                    break;
                case GridState.Playing:
                    button.Hide();
                    break;
                case GridState.RoundCompleted:
                case GridState.RoundFailed:
                    roundResultText.Text = grid.State.Value == GridState.RoundCompleted ? "OK" : "X";
                    roundResultText.Colour = grid.State.Value == GridState.RoundCompleted ? FrameworkColour.Blue : FrameworkColour.Yellow;
                    roundResultText.FadeIn().Delay(1000).Then().FadeOut().Then().OnComplete(s => grid.NextRound());
                    break;
                case GridState.GameFinished:
                    grid.Drawable.Hide();
                    button.Text = "Change settings";
                    button.Show();
                    button.Action = grid.Proceed;
                    restartButton.Show();
                    GenerateResultsText();
                    break;
            }
        }

        private void GenerateResultsText()
        {
            resultsText.Text = "";

            resultsText.AddParagraph($"Successful rounds: {grid.GameStatistics.RoundsCompleted}/{grid.GameStatistics.RoundsCompleted + grid.GameStatistics.RoundsFailed}");

            resultsText.AddParagraph("Memorization times: ");
            resultsText.AddText(string.Join("  ", grid.GameStatistics.MemorizationTimes.Select(t => t.ToString("0.0"))));

            resultsText.AddParagraph("Solve times: ");
            resultsText.AddText(string.Join("  ", grid.GameStatistics.ActionTimes.Select(t => t.ToString("0.0"))));

            resultsText.Show();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            UpdateLayout();
        }

        protected override void Update()
        {
            base.Update();
            gridContainer.Padding = new MarginPadding { Bottom = bottomContainer.Height };
            gridBorder.Size = grid.Drawable.DrawSize * grid.Drawable.Scale;
        }
    }
}
