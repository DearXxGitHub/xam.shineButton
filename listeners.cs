using System;
using Android.Animation;
using static Android.Animation.Animator;

namespace xam.shineButton
{

    public class XAnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
    {
       public Action<ValueAnimator> AnimationUpdate { get; set; }
        public void OnAnimationUpdate(ValueAnimator animation)
        {
            AnimationUpdate?.Invoke(animation);
        }
    }

    public class XAnimatorListener : Java.Lang.Object, IAnimatorListener
    {
       public Action<Animator> Cancel { get; set; }
        public void OnAnimationCancel(Animator animation)
        {
            Cancel?.Invoke(animation);
        }

        public Action<Animator> End { get; set; }
        public void OnAnimationEnd(Animator animation)
        {
            End?.Invoke(animation);
        }
        public Action<Animator> Repeat { get; set; }
        public void OnAnimationRepeat(Animator animation)
        {
            Repeat?.Invoke(animation);
        }
        public Action<Animator> Start { get; set; }
        public void OnAnimationStart(Animator animation)
        {
            Start?.Invoke(animation);
        }
    }
}