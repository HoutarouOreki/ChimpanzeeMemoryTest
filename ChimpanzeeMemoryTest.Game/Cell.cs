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

        public int? Number
        {
            get => string.IsNullOrEmpty(numberText.Text) ? (int?)null : int.Parse(numberText.Text);
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

        public void ShowNumber() => numberText.Show();

        public void ShowBackground() => Background.Show();

        public void HideNumber() => numberText.Hide();

        public void HideBackground() => Background.Hide();

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            Clicked?.Invoke(this);
            return base.OnMouseDown(e);
        }
    }
}
