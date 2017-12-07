using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using devBanner.Exceptions;
using devBanner.Options;
using devRant.NET;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace devBanner.Logic
{
    public class Banner
    {
        private const string DevrantAvatarBaseURL = "https://avatars.devrant.com";

        public static async Task<string> GenerateAsync(BannerOptions options, Profile profile, string subtext)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
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

            if (options.CacheAvatars && File.Exists(avatarFile))
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
            using (var banner = new Image<Rgba32>(800, 192))
            {
                var fontCollection = new FontCollection();
                fontCollection.Install("fonts/Comfortaa-Regular.ttf");

                var fontSizeUsername = 64;
                var fontSizeSubtext = fontSizeUsername / 2;
                var fontSizeDevrant = 16;

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
                var usernameTartgetY = banner.Height / 4;
                var usernameTarget = new Point(usernameTargetX, usernameTartgetY);

                var subtextTargetX = usernameTarget.X;
                var subtextTartgetY = usernameTarget.Y + fontSizeUsername;
                var subtextTarget = new Point(subtextTargetX, subtextTartgetY);
                var subTextWidth = banner.Width - subtextTargetX - 15;
                var subTextHeight = fontSizeSubtext;

                var devrantTargetX = banner.Width - 108;
                var devrantTargetY = banner.Height - 4 - fontSizeDevrant;
                var devrantTarget = new Point(devrantTargetX, devrantTargetY);

                // Draw background
                banner.SetBGColor(Rgba32.FromHex(profile.Avatar.Background));

                // Draw avatar
                banner.DrawImage(avatarImage, avatarSize, avatarTarget);

                // Draw username
                banner.DrawText(profile.Username, fontUsername, Rgba32.White, usernameTarget);

                // Scale font size to subtext
                fontSubtext = fontSubtext.ScaleToText(subtext, new SizeF(subTextWidth, subTextHeight), options.MaxSubtextWidth);

                subtext = subtext.AddWrap(fontSubtext, options.MaxSubtextWidth, options.MaxSubtextWraps);

                // Draw subtext
                banner.DrawText(subtext, fontSubtext, Rgba32.White, subtextTarget);

                // Draw devrant text
                banner.DrawText("devrant.com", fontDevrant, Rgba32.White, devrantTarget, HorizontalAlignment.Left, VerticalAlignment.Top);

                banner.Save(outputFile, new PngEncoder());
            }

            return outputFile;
        }
    }
}
