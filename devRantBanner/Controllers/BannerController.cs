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
        // GET/POST banner/get
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Get(string username, string subtext)
        {
            var client = DevRantClient.Create(new HttpClient());

            // Convert username to userID
            var userId = await client.GetUserID(username);

            if (!userId.Success)
            {
                return BadRequest("User does not exist!");
            }
            
            if (subtext.Length > 56)
            {
                return BadRequest("Subtext too long");
            }
            
            // Use the userID to retrive the meta information about the avatar
            var user = await client.GetUser(userId.UserId);

            var userProfile = user.Profile;

            string banner;

            try
            {
                banner = await Banner.GenerateAsync(userProfile, string.IsNullOrEmpty(subtext) ? userProfile.About : subtext);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return PhysicalFile(banner, "image/png");
        }
    }
}
