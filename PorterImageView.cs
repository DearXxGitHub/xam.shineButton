using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using System;

namespace xam.shineButton
{
    public abstract class PorterImageView : ImageView
    {
        private static string TAG = "PorterImageView";

        private static PorterDuffXfermode PORTER_DUFF_XFERMODE = new PorterDuffXfermode(PorterDuff.Mode.DstIn);

        private Canvas maskCanvas;
        private Bitmap maskBitmap;
        private Paint maskPaint;

        private Canvas drawableCanvas;
        private Bitmap drawableBitmap;
        private Paint drawablePaint;

        Color paintColor = Color.Gray;

        private bool invalidated = true;

        public PorterImageView(Context context) : base(context)
        {
            setup(context, null, 0);
        }

        public PorterImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            setup(context, attrs, 0);
        }

        public PorterImageView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            setup(context, attrs, defStyle);
        }

        private void setup(Context context, IAttributeSet attrs, int defStyle)
        {
            if (GetScaleType() == ScaleType.FitCenter)
            {
                SetScaleType(ScaleType.CenterCrop);
            }
            maskPaint = new Paint(PaintFlags.AntiAlias);
            maskPaint.Color = Color.Black;
        }

        public void setSrcColor(Color color)
        {
            paintColor = color;
            SetImageDrawable(new ColorDrawable(color));
            if (drawablePaint != null)
            {
                drawablePaint.Color = color;
                invalidate();
            }
        }

        public void invalidate()
        {
            invalidated = true;
            base.Invalidate();
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            createMaskCanvas(w, h, oldw, oldh);
        }

        private void createMaskCanvas(int width, int height, int oldw, int oldh)
        {
            bool sizeChanged = width != oldw || height != oldh;
            bool isValid = width > 0 && height > 0;
            if (isValid && (maskCanvas == null || sizeChanged))
            {
                maskCanvas = new Canvas();
                maskBitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                maskCanvas.SetBitmap(maskBitmap);

                maskPaint.Reset();
                paintMaskCanvas(maskCanvas, maskPaint, width, height);

                drawableCanvas = new Canvas();
                drawableBitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                drawableCanvas.SetBitmap(drawableBitmap);
                drawablePaint = new Paint(PaintFlags.AntiAlias);
                drawablePaint.Color = paintColor;
                invalidated = true;
            }
        }

        protected abstract void paintMaskCanvas(Canvas maskCanvas, Paint maskPaint, int width, int height);

        protected override void OnDraw(Canvas canvas)
        {
            if (!IsInEditMode)
            {
                int saveCount = canvas.SaveLayer(0.0f, 0.0f, Width, Height, null, SaveFlags.All);
                try
                {
                    if (invalidated)
                    {
                        Drawable drawable = Drawable;
                        if (drawable != null)
                        {
                            invalidated = false;
                            Matrix imageMatrix = ImageMatrix;
                            if (imageMatrix == null)
                            {// && mPaddingTop == 0 && mPaddingLeft == 0) {
                                drawable.Draw(drawableCanvas);
                            }
                            else
                            {
                                int drawableSaveCount = drawableCanvas.SaveCount;
                                drawableCanvas.Save();
                                drawableCanvas.Concat(imageMatrix);
                                drawable.Draw(drawableCanvas);
                                drawableCanvas.RestoreToCount(drawableSaveCount);
                            }

                            drawablePaint.Reset();
                            drawablePaint.FilterBitmap = false;
                            drawablePaint.SetXfermode(PORTER_DUFF_XFERMODE);
                            drawableCanvas.DrawBitmap(maskBitmap, 0.0f, 0.0f, drawablePaint);
                        }
                    }

                    if (!invalidated)
                    {
                        drawablePaint.SetXfermode(null);
                        canvas.DrawBitmap(drawableBitmap, 0.0f, 0.0f, drawablePaint);
                    }
                }
                catch (Exception e)
                {
                    string log = "Exception occured while drawing " + Id;
                    Log.Error(TAG, log, e);
                }
                finally
                {
                    canvas.RestoreToCount(saveCount);
                }
            }
            else
            {
                base.OnDraw(canvas);
            }
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (widthMeasureSpec == 0)
            {
                widthMeasureSpec = 50;
            }
            if (heightMeasureSpec == 0)
            {
                heightMeasureSpec = 50;
            }
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}
