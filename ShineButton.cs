
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Util;
using Android.Graphics;
using Android.Content.Res;
using Android.Animation;
using Android.Views.Animations;

namespace xam.shineButton
{
    public class ShineButton : PorterShapeImageView
    {
        private static string TAG = "ShineButton";
        private bool _isChecked = false;

        private Color btnColor;
        private Color btnFillColor;

        int DEFAULT_WIDTH = 50;
        int DEFAULT_HEIGHT = 50;

        DisplayMetrics metrics = new DisplayMetrics();


        Activity activity;
        ShineView shineView;
        ValueAnimator shakeAnimator;
        ShineView.ShineParams shineParams = new ShineView.ShineParams();

        OnCheckedChangeListener listener;

        private int bottomHeight;

        public ShineButton(Context context) : base(context)
        {
            if (context is Activity)
            {
                init((Activity)context);
            }
        }

        public ShineButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            initButton(context, attrs);
        }


        public ShineButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            initButton(context, attrs);
        }

        private void initButton(Context context, IAttributeSet attrs)
        {

            if (context is Activity)
            {
                init((Activity)context);
            }

            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.ShineButton);
            btnColor = a.GetColor(Resource.Styleable.ShineButton_btn_color, Color.Gray);
            btnFillColor = a.GetColor(Resource.Styleable.ShineButton_btn_fill_color, Color.Black);
            shineParams.allowRandomColor = a.GetBoolean(Resource.Styleable.ShineButton_allow_random_color, false);
            shineParams.animDuration = a.GetInteger(Resource.Styleable.ShineButton_shine_animation_duration, (int)shineParams.animDuration);
            shineParams.bigShineColor = a.GetColor(Resource.Styleable.ShineButton_big_shine_color, shineParams.bigShineColor);
            shineParams.clickAnimDuration = a.GetInteger(Resource.Styleable.ShineButton_click_animation_duration, (int)shineParams.clickAnimDuration);
            shineParams.enableFlashing = a.GetBoolean(Resource.Styleable.ShineButton_enable_flashing, false);
            shineParams.shineCount = a.GetInteger(Resource.Styleable.ShineButton_shine_count, shineParams.shineCount);
            shineParams.shineDistanceMultiple = a.GetFloat(Resource.Styleable.ShineButton_shine_distance_multiple, shineParams.shineDistanceMultiple);
            shineParams.shineTurnAngle = a.GetFloat(Resource.Styleable.ShineButton_shine_turn_angle, shineParams.shineTurnAngle);
            shineParams.smallShineColor = a.GetColor(Resource.Styleable.ShineButton_small_shine_color, shineParams.smallShineColor);
            shineParams.smallShineOffsetAngle = a.GetFloat(Resource.Styleable.ShineButton_small_shine_offset_angle, shineParams.smallShineOffsetAngle);
            shineParams.shineSize = a.GetDimensionPixelSize(Resource.Styleable.ShineButton_shine_size, shineParams.shineSize);
            a.Recycle();
            setSrcColor(btnColor);
        }

        public int getBottomHeight()
        {
            return bottomHeight;
        }

        public Color getColor()
        {
            return btnFillColor;
        }

        public bool isChecked()
        {
            return _isChecked;
        }


        public void setBtnColor(Color btnColor)
        {
            this.btnColor = btnColor;
            setSrcColor(this.btnColor);
        }

        public void setBtnFillColor(Color btnFillColor)
        {
            this.btnFillColor = btnFillColor;
        }

        public void setChecked(bool _checked, bool anim)
        {
            _isChecked = _checked;
            if (_checked)
            {
                setSrcColor(btnFillColor);
                _isChecked = true;
                if (anim) showAnim();
            }
            else
            {
                setSrcColor(btnColor);
                _isChecked = false;
                if (anim) setCancel();
            }
            onListenerUpdate(_checked);
        }

        public void setChecked(bool _checked)
        {
            setChecked(_checked, false);
        }

        private void onListenerUpdate(bool _checked)
        {
            if (listener != null)
            {
                listener.onCheckedChanged(this, _checked);
            }
        }

        public void setCancel()
        {
            setSrcColor(btnColor);
            if (shakeAnimator != null)
            {
                shakeAnimator.End();
                shakeAnimator.Cancel();
            }
        }

        public void setAllowRandomColor(bool allowRandomColor)
        {
            shineParams.allowRandomColor = allowRandomColor;
        }

        public void setAnimDuration(int durationMs)
        {
            shineParams.animDuration = durationMs;
        }

        public void setBigShineColor(Color color)
        {
            shineParams.bigShineColor = color;
        }

        public void setClickAnimDuration(int durationMs)
        {
            shineParams.clickAnimDuration = durationMs;
        }

        public void enableFlashing(bool enable)
        {
            shineParams.enableFlashing = enable;
        }

        public void setShineCount(int count)
        {
            shineParams.shineCount = count;
        }

        public void setShineDistanceMultiple(float multiple)
        {
            shineParams.shineDistanceMultiple = multiple;
        }

        public void setShineTurnAngle(float angle)
        {
            shineParams.shineTurnAngle = angle;
        }

        public void setSmallShineColor(Color color)
        {
            shineParams.smallShineColor = color;
        }

        public void setSmallShineOffAngle(float angle)
        {
            shineParams.smallShineOffsetAngle = angle;
        }

        public void setShineSize(int size)
        {
            shineParams.shineSize = size;
        }

        OnButtonClickListener onButtonClickListener;
        public override void SetOnClickListener(IOnClickListener l)
        {
            if (l is OnButtonClickListener)
            {
                base.SetOnClickListener(l);
            }
            else
            {
                if (onButtonClickListener != null)
                {
                    onButtonClickListener.setListener(l);
                }
            }
        }


        public void setOnCheckStateChangeListener(OnCheckedChangeListener listener)
        {
            this.listener = listener;
        }



        public void init(Activity activity)
        {
            this.activity = activity;
            onButtonClickListener = new OnButtonClickListener();
            SetOnClickListener(onButtonClickListener);
        }


        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            calPixels();
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

        }

        public void showAnim()
        {
            if (activity != null)
            {
                ViewGroup rootView = (ViewGroup)activity.FindViewById(Window.IdAndroidContent);
                shineView = new ShineView(activity, this, shineParams);
                rootView.AddView(shineView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
                doShareAnim();
            }
            else
            {
                Log.Error(TAG, "Please init.");
            }
        }

        public void removeView(View view)
        {
            if (activity != null)
            {
                ViewGroup rootView = activity.FindViewById<ViewGroup>(Window.IdAndroidContent);
                rootView.RemoveView(view);
            }
            else
            {
                Log.Error(TAG, "Please init.");
            }
        }

        public void setShapeResource(int raw)
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                setShape(Resources.GetDrawable(raw, null));
            }
            else
            {
                setShape(Resources.GetDrawable(raw));
            }
        }

        private void doShareAnim()
        {
            shakeAnimator = ValueAnimator.OfFloat(0.4f, 1f, 0.9f, 1f);
            shakeAnimator.SetInterpolator(new LinearInterpolator());
            shakeAnimator.SetDuration(500);
            shakeAnimator.StartDelay = 180;
            invalidate();
            shakeAnimator.AddUpdateListener(new XAnimatorUpdateListener()
            {
                AnimationUpdate = (valueAnimator) =>
                {
                    ScaleX = (float)valueAnimator.AnimatedValue;
                    ScaleY = (float)valueAnimator.AnimatedValue;
                }
            });
            shakeAnimator.AddListener(new XAnimatorListener()
            {
                Start = (animator) =>
      {
          setSrcColor(btnFillColor);
      },
                End = (animator) =>
                  {
                      setSrcColor(_isChecked ? btnFillColor : btnColor);
                  },
                Cancel = (animator) =>
                  {
                      setSrcColor(btnColor);
                  }
            });
            shakeAnimator.Start();
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        private void calPixels()
        {
            if (activity != null && metrics != null)
            {
                activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
                int[] location = new int[2];
                GetLocationInWindow(location);
                bottomHeight = metrics.HeightPixels - location[1];
            }
        }

        public class OnButtonClickListener : Java.Lang.Object, IOnClickListener
        {
            public void setListener(IOnClickListener listener)
            {
                this.listener = listener;
            }
            IOnClickListener listener;

            public OnButtonClickListener()
            {
            }

            public OnButtonClickListener(IOnClickListener l)
            {
                listener = l;
            }


            public void OnClick(View view)
            {
                ShineButton btn = view as ShineButton;
                if (!btn.isChecked())
                {
                    btn._isChecked = true;
                    btn.showAnim();
                }
                else
                {
                    btn._isChecked = false;
                    btn.setCancel();
                }

                btn.onListenerUpdate(btn._isChecked);
                if (listener != null)
                {
                    listener.OnClick(view);
                }
            }
        }

        public interface OnCheckedChangeListener
        {
            void onCheckedChanged(View view, bool _checked);
        }

    }
}