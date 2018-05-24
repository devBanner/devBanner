using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using devBanner.Exceptions;
using devBanner.Options;
using devRant.NET;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.Primitives;

namespace devBanner.Logic
{
    public class Banner
    {
        private const string DevrantAvatarBaseURL = "https://avatars.devrant.com";

        public static async Task<string> GenerateAsync(BannerOptions options, Profile profile, string subtext, int width = 800, int height = 192)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (width < 64)
            {
                width = 800;
                height = 192;
            }

            if (width > 2048)
            {
                width = 2048;
                height = (int)(width / options.WidthToHeightRatio);
            }

            // Avatar base url + avatar meta = rendered avatar url
            var avatarURL = $"{DevrantAvatarBaseURL}/{profile.Avatar.Image}";

            const string outputDir = "generated";
            const string avatarsDir = "avatars";

            var outputFileName = $"{profile.Username}.png";

            var workingDir = Directory.GetCurrentDirectory();
            var outputPath = Path.Combine(workingDir, outputDir);

            var avatarPath = Path.Combine(workingDir, avatarsDir);

            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(avatarPath);

            var avatarFile = Path.Combine(avatarPath, outputFileName);
            var outputFile = Path.Combine(outputPath, outputFileName);

            byte[] data;

            if (options.CacheAvatars && File.Exists(avatarFile) && File.GetCreationTime(avatarFile) > DateTime.Now.AddMinutes(-options.MaxCacheAvatarAge))
            {
                data = await File.ReadAllBytesAsync(avatarFile);
            }
            else
            {
                // Download rendered avatar
                var httpClient = new HttpClient();

                using (var response = await httpClient.GetAsync(avatarURL))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new AvatarNotFoundException(profile.Username);
                    }

                    response.EnsureSuccessStatusCode();

                    data = await response.Content.ReadAsByteArrayAsync();

                    if (options.CacheAvatars)
                    {
                        await File.WriteAllBytesAsync(avatarFile, data);
                    }
                }
            }

            using (var avatarImage = Image.Load(data))
            using (var banner = new Image<Rgba32>(width, height))
            {
                var fontCollection = new FontCollection();
                fontCollection.Install("fonts/Comfortaa-Regular.ttf");

                var fontSizeUsername = (int)(width * 0.08);
                var fontSizeSubtext = (int)(width * 0.04);
                var fontSizeDevrant = (int)(width * 0.02);       

                var fontUsername = fontCollection.CreateFont("Comfortaa", fontSizeUsername, FontStyle.Bold);
                var fontSubtext = fontCollection.CreateFont("Comfortaa", fontSizeSubtext, FontStyle.Regular);
                var fontDevrant = fontCollection.CreateFont("Comfortaa", fontSizeDevrant, FontStyle.Regular);

                var avatarHeight = banner.Height;
                var avatarWidth = avatarHeight;
                var avatarSize = new Size(avatarWidth, avatarHeight);

                var avatarTargetX = 15;
                var avatarTargetY = 0;
                var avatarTarget = new Point(avatarTargetX, avatarTargetY);

                var usernameTargetX = banner.Width / 3;
                var usernameTargetY = (banner.Height / 4f) - (fontSizeUsername / 2f);
                var usernameTarget = new PointF(usernameTargetX, usernameTargetY);

                var subtextTargetX = usernameTarget.X;
                var subtextTargetY = usernameTarget.Y + fontSizeUsername;
                var subtextTarget = new PointF(subtextTargetX, subtextTargetY + (fontSizeSubtext / 2f));
                var subTextWidth = banner.Width - subtextTargetX - 15;
                var subTextHeight = fontSizeSubtext;

                var devrantTargetX = banner.Width - (int)(width * 0.130);
                var devrantTargetY = banner.Height - (int)(width * 0.03);
                var devrantTarget = new Point(devrantTargetX, devrantTargetY);
                
                var backgroundColor = Rgba32.FromHex(profile.Avatar.Background);
                var foregroundColor = GetForegroundColor(backgroundColor);
                
                // Draw background
                banner.SetBGColor(backgroundColor);

                // Draw avatar
                banner.DrawImage(avatarImage, avatarSize, avatarTarget);

                // Draw username
                banner.DrawText(profile.Username, fontUsername, foregroundColor, usernameTarget, verticalAlignment: VerticalAlignment.Top);

                // Scale font size to subtext
                fontSubtext = fontSubtext.ScaleToText(subtext, new SizeF(subTextWidth, subTextHeight), subTextWidth);

                // Add subtext word wrapping
                subtext = subtext.AddWrap(fontSubtext, subTextWidth, options.MaxSubtextWraps);

                // Draw subtext
                banner.DrawText(subtext, fontSubtext, foregroundColor, subtextTarget, verticalAlignment: VerticalAlignment.Top);

                // Draw devrant text
                banner.DrawText("devrant.com", fontDevrant, foregroundColor, devrantTarget, HorizontalAlignment.Left, VerticalAlignment.Top);

                banner.Save(outputFile, new PngEncoder());
            }

            return outputFile;
        }

        /// <summary>
        /// Returns a foreground color based on the luminance of the background color. 
        /// </summary>
        /// <returns>Black color if the background is bright, white if the background is dark</returns>
        private static Rgba32 GetForegroundColor(Rgba32 backgroundColor)
        {
            // Calculating the perceptive luminance - human eye favors green color... Based on http://juicystudio.com/article/luminositycontrastratioalgorithm.php
            double perceptiveLuminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255;

            if (perceptiveLuminance > 0.7) // A lower value will trigger black font faster. (0.65 will trigger black font in light blue bg as well)
            {
                return Rgba32.Black; // bright colors --> black font
            }
            else
            {
                return Rgba32.White; // dark colors --> white font
            }

        }
    }
}
