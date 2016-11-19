using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Util;
using Android.Content.Res;

namespace xam.shineButton
{

    public class PorterShapeImageView : PorterImageView
    {
        private Drawable shape;
        private Matrix matrix;
        private Matrix drawMatrix;

        public PorterShapeImageView(Context context) : base(context)
        {
            setup(context, null, 0);
        }

        public PorterShapeImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            setup(context, attrs, 0);
        }

        public PorterShapeImageView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            setup(context, attrs, defStyle);
        }

        private void setup(Context context, IAttributeSet attrs, int defStyle)
        {
            if (attrs != null)
            {
                TypedArray typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.PorterImageView, defStyle, 0);
                shape = typedArray.GetDrawable(Resource.Styleable.PorterImageView_siShape);
                typedArray.Recycle();
            }
            matrix = new Matrix();
        }

       public void setShape(Drawable drawable)
        {
            shape = drawable;
            invalidate();
        }


        protected override void paintMaskCanvas(Canvas maskCanvas, Paint maskPaint, int width, int height)
        {
            if (shape != null)
            {
                if (shape is BitmapDrawable)
                {
                    configureBitmapBounds(Width, Height);
                    if (drawMatrix != null)
                    {
                        int drawableSaveCount = maskCanvas.SaveCount;
                        maskCanvas.Save();
                        maskCanvas.Concat(matrix);
                        shape.Draw(maskCanvas);
                        maskCanvas.RestoreToCount(drawableSaveCount);
                        return;
                    }
                }
                shape.SetBounds(0, 0, Width, Height);
                shape.Draw(maskCanvas);
            }
        }

        private void configureBitmapBounds(int viewWidth, int viewHeight)
        {
            drawMatrix = null;
            int drawableWidth = shape.IntrinsicWidth;
            int drawableHeight = shape.IntrinsicHeight;
            bool fits = viewWidth == drawableWidth && viewHeight == drawableHeight;

            if (drawableWidth > 0 && drawableHeight > 0 && !fits)
            {
                shape.SetBounds(0, 0, drawableWidth, drawableHeight);
                float widthRatio = (float)viewWidth / (float)drawableWidth;
                float heightRatio = (float)viewHeight / (float)drawableHeight;
                float scale = Math.Min(widthRatio, heightRatio);
                float dx = (int)((viewWidth - drawableWidth * scale) * 0.5f + 0.5f);
                float dy = (int)((viewHeight - drawableHeight * scale) * 0.5f + 0.5f);

                matrix.SetScale(scale, scale);
                matrix.PostTranslate(dx, dy);
            }
        }
    }
}