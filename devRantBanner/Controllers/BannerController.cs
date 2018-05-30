using System;
using System.Threading.Tasks;
using devBanner.Logic;
using devRant.NET;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using devBanner.Exceptions;
using devBanner.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace devBanner.Controllers
{
    [Route("[controller]")]
    public class BannerController : Controller
    {
        private readonly ILogger _logger;
        private readonly BannerOptions _bannerOptions;

        public BannerController(ILogger<BannerController> logger,
            IOptionsSnapshot<BannerOptions> bannerOptions)
        {
            _logger = logger;
            _bannerOptions = bannerOptions.Value;
        }

        // GET/POST banner/get
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Get(string username, string subtext, int width)
        {
            var client = DevRantClient.Create(new HttpClient());

            // Convert username to userID
            var userId = await client.GetUserID(username);

            if (!userId.Success)
            {
                _logger.LogDebug($"User {username} does not exist.");

                return BadRequest("User does not exist!");
            }

            // Use the userID to retrive the meta information about the avatar
            var user = await client.GetUser(userId.UserId);
            var userProfile = user.Profile;

            var text = string.IsNullOrEmpty(subtext) ? userProfile.About : subtext;

            if (text.Length > _bannerOptions.MaxSubtextLength)
            {
                _logger.LogDebug("Subtext is too long");
                _logger.LogDebug(subtext);

                return BadRequest("Subtext too long");
            }

            string banner;

            try
            {
                banner = await Banner.GenerateAsync(_bannerOptions, userProfile, text, width, (int)(width / _bannerOptions.WidthToHeightRatio));
            }
            catch (AvatarNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                return StatusCode(500, ex.Message);
            }

            _logger.LogDebug($"Successfully generated banner for {username}");

            return PhysicalFile(banner, "image/png");
        }
    }
}
