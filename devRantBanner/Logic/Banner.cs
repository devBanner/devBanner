using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace devBanner.Logic
{
    public class Banner
    {
        private enum AvatarMetaColors
        {
            Green = 1,
            Purple = 2,
            Orange = 3,
            Blue = 4,
            Red = 5,
            LightBlue = 6,
            Yellow = 7
        }

        private AvatarMetaColors BackgroundColorResolver(string meta)
        {
            // v-18_c-12_g-m_b-7
            // Different parameters are splitted by an underscore
            // Left side of dash defines parameter
            // Right side defines value
            var backgroundNum = Regex.Matches(meta, @"b-(\d+?)", RegexOptions.Compiled & RegexOptions.IgnoreCase).First().Groups[1].Value;
            return Enum.Parse<AvatarMetaColors>(backgroundNum);
        }

        public static string Generate(string avatarURL, string avatarBGColor, string username, string subtext)
        {
            var workingDir = Directory.GetCurrentDirectory();
            var outputPath = $"{workingDir}/generated/{username}.png";

            // Download rendered avatar
            var httpClient = new HttpClient();
            var responseStream = httpClient.GetStreamAsync(avatarURL).Result;
            
            var avatarImage = Image.Load(responseStream);
            
            System.IO.Directory.CreateDirectory("generated");
            using (Image<Rgba32> banner = new Image<Rgba32>(800, 192))
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

                var avatarTargetX = 0;
                var avatarTargetY = 0;
                var avatarTarget = new Point(avatarTargetX, avatarTargetY);
                
                var usernameTargetX = banner.Width / 3;
                var usernameTartgetY = banner.Height / 4;
                var usernameTarget = new Point(usernameTargetX, usernameTartgetY);

                var subtextTargetX = usernameTarget.X;
                var subtextTartgetY = usernameTarget.Y + fontSizeUsername;
                var subtextTarget = new Point(subtextTargetX, subtextTartgetY);

                var devrantTargetX = banner.Width - 108;
                var devrantTargetY = banner.Height - 4 - fontSizeDevrant;
                var devrantTarget = new Point(devrantTargetX, devrantTargetY);

                // Draw background
                banner.Mutate(i => i.BackgroundColor(Rgba32.FromHex(avatarBGColor)));

                // Draw avatar
                banner.Mutate(i => i.DrawImage(avatarImage, 1, avatarSize, avatarTarget));

                // Draw username
                banner.Mutate(i => i.DrawText(username, fontUsername, Rgba32.White, usernameTarget, new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                }));

                // Draw subtext
                banner.Mutate(i => i.DrawText(subtext, fontSubtext, Rgba32.White, subtextTarget, new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                }));

                // Draw devrant text
                banner.Mutate(i => i.DrawText("devrant.com", fontDevrant, Rgba32.White, devrantTarget, new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                }));

                banner.Save(outputPath);
            }

            responseStream.Close();
            return outputPath;
        }
    }
}
