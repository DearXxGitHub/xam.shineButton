using System;
using Android.Content;
using Android.Views;
using Android.Graphics;
using Android.Animation;
using Android.Util;
using xam.EasingInterpolator;
using XEI = xam.EasingInterpolator.EasingInterpolator;
namespace xam.shineButton
{
    public class ShineView : View
    {
        private static string TAG = "ShineView";

        private static long FRAME_REFRESH_DELAY = 25;//default 10ms ,change to 25ms for saving cpu.

        ShineAnimator shineAnimator;
        ValueAnimator clickAnimator;

        ShineButton shineButton;
        private Paint paint;
        private Paint paint2;
        private Paint paintSmall;

        int colorCount = 10;
        static Color[] colorRandom = new Color[10];

        //Customer property
        int shineCount;
        float smallOffsetAngle;
        float turnAngle;
        long animDuration;
        long clickAnimDuration;
        float shineDistanceMultiple;
        Color smallShineColor = colorRandom[0];
        Color bigShineColor = colorRandom[1];

        int shineSize = 0;

        bool allowRandomColor = false;
        bool enableFlashing = false;


        RectF rectF = new RectF();
        RectF rectFSmall = new RectF();

        Random random = new Random();
        int centerAnimX;
        int centerAnimY;
        int btnWidth;
        int btnHeight;

        double thirdLength;
        float value;
        float clickValue = 0;
        bool isRun = false;
        private float distanceOffset = 0.2f;


        public ShineView(Context context) : base(context) { }

        public ShineView(Context context, ShineButton shineButton, ShineParams shineParams) : base(context)
        {

            initShineParams(shineParams, shineButton);


            this.shineAnimator = new ShineAnimator(animDuration, shineDistanceMultiple, clickAnimDuration);
            ValueAnimator.FrameDelay = FRAME_REFRESH_DELAY;
            this.shineButton = shineButton;


            paint = new Paint();
            paint.Color = bigShineColor;
            paint.StrokeWidth = 20;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeCap = Paint.Cap.Round;

            paint2 = new Paint();
            paint2.Color = Color.White;
            paint2.StrokeWidth = 20;
            paint2.StrokeCap = Paint.Cap.Round;

            paintSmall = new Paint();
            paintSmall.Color = smallShineColor;
            paintSmall.StrokeWidth = 10;
            paintSmall.SetStyle(Paint.Style.Stroke);
            paintSmall.StrokeCap = Paint.Cap.Round;

            clickAnimator = ValueAnimator.OfFloat(0f, 1.1f);
            ValueAnimator.FrameDelay = FRAME_REFRESH_DELAY;
            clickAnimator.SetDuration(clickAnimDuration);
            clickAnimator.SetInterpolator(new XEI(Ease.QUART_OUT));

            clickAnimator.AddUpdateListener(new XAnimatorUpdateListener()
            {
                AnimationUpdate = (valueAnimator) =>
           {
               clickValue = (float)valueAnimator.AnimatedValue;
               Invalidate();
           }
            });

            clickAnimator.AddListener(new XAnimatorListener()
            {
                End = (animator) =>
                {
                    clickValue = 0;
                    Invalidate();
                }
            });

            shineAnimator.AddListener(new XAnimatorListener()
            {
                End = (animator) =>
                 {
                     shineButton.removeView(this);
                 }
            });
        }


        public ShineView(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public ShineView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }


        public void showAnimation(ShineButton shineButton)
        {
            btnWidth = shineButton.Width;
            btnHeight = shineButton.Height;
            thirdLength = getThirdLength(btnHeight, btnWidth);
            int[] location = new int[2];
            shineButton.GetLocationInWindow(location);
            centerAnimX = location[0] + btnWidth / 2;
            centerAnimY = MeasuredHeight - shineButton.getBottomHeight() + btnHeight / 2;
            shineAnimator.AddUpdateListener(new XAnimatorUpdateListener()
            {
                AnimationUpdate = (valueAnimator) =>
          {
              value = (float)valueAnimator.AnimatedValue;
              if (shineSize != 0 && shineSize > 0)
              {
                  paint.StrokeWidth = (shineSize) * (shineDistanceMultiple - value);
                  paintSmall.StrokeWidth = ((float)shineSize / 3 * 2) * (shineDistanceMultiple - value);
              }
              else
              {
                  paint.StrokeWidth = (btnWidth / 2) * (shineDistanceMultiple - value);
                  paintSmall.StrokeWidth = (btnWidth / 3) * (shineDistanceMultiple - value);
              }


              rectF.Set(centerAnimX - (btnWidth / (3 - shineDistanceMultiple) * value), centerAnimY - (btnHeight / (3 - shineDistanceMultiple) * value), centerAnimX + (btnWidth / (3 - shineDistanceMultiple) * value), centerAnimY + (btnHeight / (3 - shineDistanceMultiple) * value));
              rectFSmall.Set(centerAnimX - (btnWidth / ((3 - shineDistanceMultiple) + distanceOffset) * value), centerAnimY - (btnHeight / ((3 - shineDistanceMultiple) + distanceOffset) * value), centerAnimX + (btnWidth / ((3 - shineDistanceMultiple) + distanceOffset) * value), centerAnimY + (btnHeight / ((3 - shineDistanceMultiple) + distanceOffset) * value));

              Invalidate();
          }
            });
            shineAnimator.startAnim(this, centerAnimX, centerAnimY);
            clickAnimator.Start();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            for (int i = 0; i < shineCount; i++)
            {
                if (allowRandomColor)
                {
                    paint.Color = colorRandom[Math.Abs(colorCount / 2 - i) >= colorCount ? colorCount - 1 : Math.Abs(colorCount / 2 - i)];
                }
                canvas.DrawArc(rectF, 360f / shineCount * i + 1 + ((value - 1) * turnAngle), 0.1f, false, getConfigPaint(paint));
            }

            for (int i = 0; i < shineCount; i++)
            {
                if (allowRandomColor)
                {
                    paint.Color = colorRandom[Math.Abs(colorCount / 2 - i) >= colorCount ? colorCount - 1 : Math.Abs(colorCount / 2 - i)];
                }
                canvas.DrawArc(rectFSmall, 360f / shineCount * i + 1 - smallOffsetAngle + ((value - 1) * turnAngle), 0.1f, false, getConfigPaint(paintSmall));

            }
            paint.StrokeWidth = btnWidth * (clickValue) * (shineDistanceMultiple - distanceOffset);
            if (clickValue != 0)
            {
                paint2.StrokeWidth = btnWidth * (clickValue) * (shineDistanceMultiple - distanceOffset) - 8;
            }
            else
            {
                paint2.StrokeWidth = 0;
            }
            canvas.DrawPoint(centerAnimX, centerAnimY, paint);
            canvas.DrawPoint(centerAnimX, centerAnimY, paint2);
            if (shineAnimator != null && !isRun)
            {
                isRun = true;
                showAnimation(shineButton);
            }
        }

        private Paint getConfigPaint(Paint paint)
        {
            if (enableFlashing)
            {
                paint.Color = colorRandom[random.Next(colorCount - 1)];
            }
            return paint;
        }

        private double getThirdLength(int btnHeight, int btnWidth)
        {
            int all = btnHeight * btnHeight + btnWidth * btnWidth;
            return Math.Sqrt(all);
        }

        public class ShineParams
        {
            public ShineParams()
            {
                colorRandom[0] = Color.ParseColor("#FFFF99");
                colorRandom[1] = Color.ParseColor("#FFCCCC");
                colorRandom[2] = Color.ParseColor("#996699");
                colorRandom[3] = Color.ParseColor("#FF6666");
                colorRandom[4] = Color.ParseColor("#FFFF66");
                colorRandom[5] = Color.ParseColor("#F44336");
                colorRandom[6] = Color.ParseColor("#666666");
                colorRandom[7] = Color.ParseColor("#CCCC00");
                colorRandom[8] = Color.ParseColor("#666666");
                colorRandom[9] = Color.ParseColor("#999933");
            }

            public bool allowRandomColor = false;
            public long animDuration = 1500;
            public Color bigShineColor = Color.Black;
            public long clickAnimDuration = 200;
            public bool enableFlashing = false;
            public int shineCount = 7;
            public float shineTurnAngle = 20;
            public float shineDistanceMultiple = 1.5f;
            public float smallShineOffsetAngle = 20;
            public Color smallShineColor = Color.Black;
            public int shineSize = 0;
        }

        private void initShineParams(ShineParams shineParams, ShineButton shineButton)
        {
            shineCount = shineParams.shineCount;
            turnAngle = shineParams.shineTurnAngle;
            smallOffsetAngle = shineParams.smallShineOffsetAngle;
            enableFlashing = shineParams.enableFlashing;
            allowRandomColor = shineParams.allowRandomColor;
            shineDistanceMultiple = shineParams.shineDistanceMultiple;
            animDuration = shineParams.animDuration;
            clickAnimDuration = shineParams.clickAnimDuration;
            smallShineColor = shineParams.smallShineColor;
            bigShineColor = shineParams.bigShineColor;
            shineSize = shineParams.shineSize;
            if (smallShineColor == 0)
            {
                smallShineColor = colorRandom[6];
            }

            if (bigShineColor == 0)
            {
                bigShineColor = shineButton.getColor();
            }

        }
    }
}