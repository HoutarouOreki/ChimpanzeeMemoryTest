using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using System;

namespace ChimpanzeeMemoryTest.Game
{
    internal class Cell : Container
    {
        private readonly SpriteText numberText;

        public event Action<Cell> Clicked;

        public readonly Box Background;

        public int Number
        {
            get => string.IsNullOrEmpty(numberText.Text) ? -1 : int.Parse(numberText.Text);
            set => numberText.Text = value.ToString();
        }

        public Cell()
        {
            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                Background = new Box
                {
                    Colour = FrameworkColour.BlueDark,
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both
                },
                new DrawSizePreservingFillContainer
                {
                    Strategy = DrawSizePreservationStrategy.Average,
                    TargetDrawSize = new osuTK.Vector2(30),
                    Child = numberText = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = new FontUsage("OpenSans-Bold"),
                        Alpha = 0
                    }
                }
            };
        }

        public void Show(bool onlyIfNumber, bool withNumber = false)
        {
            if (onlyIfNumber && Number == -1)
                return;
            Background.Show();
            if (withNumber)
                numberText.Show();
        }

        public void Hide(bool onlyLeaveBackgroundIfNumber)
        {
            numberText.Hide();
            if (onlyLeaveBackgroundIfNumber && Number != -1)
                return;
            Background.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            Clicked?.Invoke(this);
            return base.OnClick(e);
        }
    }
}
