using Android.Animation;
using Android.Graphics;
using xam.EasingInterpolator;
using XEI = xam.EasingInterpolator.EasingInterpolator;

namespace xam.shineButton
{
    public class ShineAnimator : ValueAnimator
    {

        public float MAX_VALUE = 1.5f;
        public long ANIM_DURATION = 1500;
        public Canvas canvas;
        public ShineAnimator()
        {
            SetFloatValues(1f, MAX_VALUE);
            SetDuration(ANIM_DURATION);
            StartDelay = 200;
            SetInterpolator(new XEI(Ease.QUART_OUT));
        }
        public ShineAnimator(long duration, float max_value, long delay)
        {
            SetFloatValues(1f, max_value);
            SetDuration(duration);
            StartDelay = delay;
            SetInterpolator(new XEI(Ease.QUART_OUT));
        }

        public void startAnim(ShineView shineView, int centerAnimX, int centerAnimY)
        {

            Start();
        }

        public void setCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }


    }
}