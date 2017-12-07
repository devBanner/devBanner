using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Primitives;

namespace devBanner.Logic
{
    public static class RenderExtensions
    {
        public static Image<Rgba32> DrawText(
            this Image<Rgba32> canavas,
            string text,
            Font font,
            Rgba32 color,
            Point target,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Center)
        {
            canavas.Mutate(i => i.DrawText(text, font, color, target, new TextGraphicsOptions(true)
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment
            }));

            return canavas;
        }

        public static Image<Rgba32> SetBGColor(this Image<Rgba32> canavas, Rgba32 color)
        {
            canavas.Mutate(i => i.BackgroundColor(color));
            return canavas;
        }

        public static Image<Rgba32> DrawImage(this Image<Rgba32> canavas, Image<Rgba32> image, Size size, Point target, float alpha = 1)
        {
            canavas.Mutate(i => i.DrawImage(image, alpha, size, target));
            return canavas;
        }

        public static Font ScaleToText(this Font font, string text, SizeF desiredSize, float maxWidth)
        {
            SizeF size = TextMeasurer.Measure(text, new RendererOptions(font) { WrappingWidth = maxWidth });

            // Modified version of https://github.com/SixLabors/Samples/blob/master/ImageSharp/DrawWaterMarkOnImage/Program.cs#L92
            float targetHeight = desiredSize.Height;
            float targetMinHeight = desiredSize.Height + 15;

            var s = new SizeF(float.MaxValue, float.MaxValue);

            var scaledFont = font;

            float scaleFactor = Math.Min(desiredSize.Width / size.Width, desiredSize.Height / size.Height);

            int trapCount = (int)scaledFont.Size * 2;
            if (trapCount < 10)
            {
                trapCount = 10;
            }

            bool isTooSmall = false;

            while ((s.Height > targetHeight || s.Height < targetHeight) && trapCount > 0)
            {
                if (s.Height > targetHeight)
                {
                    if (isTooSmall)
                    {
                        scaleFactor = scaleFactor / 2;
                    }

                    scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
                    isTooSmall = false;
                }

                if (s.Height < targetMinHeight)
                {
                    if (!isTooSmall)
                    {
                        scaleFactor = scaleFactor / 2;
                    }

                    scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
                    isTooSmall = true;
                }

                trapCount--;

                s = TextMeasurer.Measure(text, new RendererOptions(scaledFont) { WrappingWidth = maxWidth });
            }

            return scaledFont;
        }

        public static string AddWrap(this string text, Font font, float maxWidth, int maxWraps = 5)
        {
            float textWidth = TextMeasurer.Measure(text, new RendererOptions(font)).Width;

            if (textWidth < maxWidth)
                return text;

            var sb = new StringBuilder();

            float currentWidth = 0;

            var currentWraps = 0;
            var words = text.Split(' ');

            foreach (var word in words)
            {
                var wordSize = TextMeasurer.Measure(word, new RendererOptions(font));

                if (wordSize.Width + currentWidth < maxWidth)
                {
                    currentWidth += wordSize.Width;
                }
                else
                {
                    if (currentWraps + 1 > maxWraps)
                    {
                        sb.Append("...");

                        break;
                    }

                    sb.AppendLine();

                    currentWraps++;
                    currentWidth = 0;
                }

                sb.Append(word);
                sb.Append(' ');
            }

            return sb.ToString();
        }
    }
}
