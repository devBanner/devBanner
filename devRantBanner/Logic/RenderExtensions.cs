using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Font ScaleToText(this Font font, string text, SizeF desiredSize)
        {
            SizeF size = TextMeasurer.Measure(text, new RendererOptions(font));

            // Calculate if we need to downscale the font size to fit the banner
            float scalingFactor = Math.Min(desiredSize.Width / size.Width, desiredSize.Height / size.Height);

            // Create scaled font new font
            return new Font(font, scalingFactor * font.Size);
        }

        public static string AddWrap(this string text, Font font, float maxWidth, int maxWraps = 5)
        {
            float textWidth = TextMeasurer.Measure(text, new RendererOptions(font)).Width;

            if(maxWraps == 0)
                return text;

            if (textWidth < maxWidth)
                return text;

            string fittingTextLine = String.Empty;

            List<string> words = text.Split(' ').ToList();
            var addedWords = 0;
            for (var i = 0; i < words.Count -1; i++)
            {
                var word = words[i];
                var fittingTextLineWidth = TextMeasurer.Measure(fittingTextLine, new RendererOptions(font)).Width;
                var wordWidth = TextMeasurer.Measure(word, new RendererOptions(font)).Width;

                if(fittingTextLineWidth + wordWidth <= maxWidth)
                {
                    fittingTextLine += $" {word}";
                    addedWords++;
                }
                else
                {
                    break;
                }
            }

            if(addedWords < words.Count)
            {
                fittingTextLine += "\n" + words.Skip(addedWords).Take(words.Count - addedWords).Aggregate((i, j) => i + " " + j).AddWrap(font, maxWidth, maxWraps -1);
            }

            // Remove first space
            fittingTextLine = fittingTextLine.Substring(1, fittingTextLine.Length - 2);

            return fittingTextLine;
        }
    }
}
