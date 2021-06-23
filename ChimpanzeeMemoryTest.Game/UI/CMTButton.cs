using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace ChimpanzeeMemoryTest.Game.UI
{
    internal class CMTButton : BasicButton
    {
        private Color4 backgroundColour = FrameworkColour.GreenDark;

        public new Color4 BackgroundColour
        {
            get => backgroundColour;
            set {
                backgroundColour = value;
                UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            if (IsHovered && Enabled.Value)
                base.BackgroundColour = FrameworkColour.Blue;
            else
                base.BackgroundColour = BackgroundColour;
        }

        protected override bool OnHover(HoverEvent e)
        {
            UpdateLayout();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e) => UpdateLayout();

        protected override void LoadComplete()
        {
            UpdateLayout();
            base.LoadComplete();
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (Enabled.Value)
                Background.FlashColour(Color4.White, 500, Easing.OutQuint);
            return base.OnClick(e);
        }
    }
}
