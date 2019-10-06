using ChimpanzeeMemoryTest.Game.UI;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.MathUtils;
using osu.Framework.Screens;
using System;
using System.Collections.Generic;

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
                            Text = "Generate grid",
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
        }

        private void OnButtonClicked()
        {
            grid.Proceed();
            if (grid.IsReady)
                button.Hide();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            gridContainer.Add(grid.Drawable);

        }
    }
}
