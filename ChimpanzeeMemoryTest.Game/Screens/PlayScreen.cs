using System;
using ChimpanzeeMemoryTest.Game.UI;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;

namespace ChimpanzeeMemoryTest.Game.Screens
{
    internal class PlayScreen : Screen
    {
        private readonly Container gridContainer;
        private readonly CMTButton button;
        private readonly SpriteText sizeText;
        private readonly FillFlowContainer leftSettings;
        private readonly FillFlowContainer rightSettings;
        private readonly SpriteText amountOfNumbersText;
        private readonly BindableInt visibleBoxesBindable = new BindableInt(1)
        {
            MinValue = 0,
            MaxValue = 2,
        };
        private readonly SpriteText visibleBoxesText;
        private readonly SpriteText previewText;

        private Grid grid { get; } = new Grid();

        public PlayScreen()
        {
            Padding = new MarginPadding(20);
            InternalChildren = new Drawable[]
            {
                gridContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Bottom = 120 },
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
                        }
                    }
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    RelativePositionAxes = Axes.Y,
                    Height = 120,
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
                                }
                            }
                        }
                    }
                }
            };
            grid.State.BindValueChanged(OnGridStateChange, true);
            grid.SizeBindable.BindValueChanged(vc => sizeText.Text = $"Size: {vc.NewValue}x{vc.NewValue}", true);
            grid.AmountOfNumbers.BindValueChanged(vc => amountOfNumbersText.Text = $"Numbers: {vc.NewValue}", true);
            visibleBoxesBindable.BindValueChanged(OnVisibleBoxesSettingChange, true);
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
            switch (grid.State.Value)
            {
                case GridState.NotReady:
                    button.Text = "Generate grid";
                    button.Show();
                    button.Action = grid.Proceed;
                    leftSettings.Show();
                    rightSettings.Show();
                    previewText.FadeTo(0.3f);
                    break;
                case GridState.GeneratedAndWaiting:
                    button.Text = "Start by clicking the first box";
                    button.Show();
                    button.Action = null;
                    leftSettings.Hide();
                    rightSettings.Hide();
                    previewText.Hide();
                    break;
                case GridState.Playing:
                    button.Hide();
                    break;
                case GridState.Completed:
                case GridState.Failed:
                    button.Text = "Generate new grid";
                    button.Show();
                    button.Action = grid.Proceed;
                    break;
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            gridContainer.Add(grid.Drawable);
            UpdateLayout();
        }
    }
}
