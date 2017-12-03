using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devBanner.Logic;
using devRant.NET;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace devBanner.Controllers
{
    [Route("generate/[controller]")]
    public class BannerController : Controller
    {
        private string DevrantAvatarBaseURL { get; set; } = "https://avatars.devrant.com";

        // GET banner/get
        [HttpGet()]
        [HttpPost()]
        public object Get(string username, string subtext)
        {
            try
            {
                if (subtext.Length > 56)
                    throw new ArgumentException("Subtext too long");

                // Convert username to userID
                var client = DevRantClient.Create(new HttpClient());
                var userId = client.GetUserID(username).Result.UserId;

                // Use the userID to retrive the meta information about the avatar
                var userProfile = client.GetUser(userId).Result.Profile;
                var avatar = userProfile.Avatar;
                // Avatar base url + avatar meta = rendered avatar url
                var avatarPath = $"{this.DevrantAvatarBaseURL}/{avatar.Image}";

                var banner = Banner.Generate(avatarPath, userProfile, (String.IsNullOrEmpty(subtext) ? userProfile.About : subtext));

                return base.PhysicalFile(banner, "image/png");
            }
            catch (ArgumentException)
            {
                return "Subtext exceeds max subtext lenght (56 chars)";
            }
            catch
            {
                return "Couldn't create banner. (Wrong username?)";
            }
        }
    }
}
