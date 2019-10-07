using System;
using ChimpanzeeMemoryTest.Game.UI;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

namespace ChimpanzeeMemoryTest.Game.Screens
{
    internal class PlayScreen : Screen
    {
        private readonly Container gridContainer;
        private readonly CMTButton button;

        private Grid grid { get; } = new Grid();

        public PlayScreen()
        {
            Padding = new MarginPadding(20);
            InternalChildren = new Drawable[]
            {
                gridContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.7f,
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Y,
                    Y = 0.8f,
                    Height = 0.2f,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        button = new CMTButton
                        {
                            FillMode = FillMode.Fit,
                            FillAspectRatio = 2,
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            Action = OnButtonClicked,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        }
                    }
                }
            };
            grid.State.BindValueChanged(OnGridStateChange, true);
        }

        private void OnGridStateChange(ValueChangedEvent<GridState> obj) => UpdateButton();

        private void OnButtonClicked()
        {
            grid.Proceed();
            if (grid.State.Value == GridState.Playing)
                button.Hide();
        }

        private void UpdateButton()
        {
            switch (grid.State.Value)
            {
                case GridState.NotReady:
                    button.Text = "Generate grid";
                    button.Show();
                    button.Action = grid.Proceed;
                    break;
                case GridState.GeneratedAndWaiting:
                    button.Text = "Start by clicking the first box";
                    button.Show();
                    button.Action = null;
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
            UpdateButton();
        }
    }
}
