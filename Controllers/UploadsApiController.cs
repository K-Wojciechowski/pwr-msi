#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pwr_msi.Models.Dto;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Authorize]
    [Route(template: "api/uploads/")]
    public class UploadsApiController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly S3Service _s3Service;
        private readonly ILogger<UploadsApiController> _logger;

        public UploadsApiController(MsiDbContext dbContext, S3Service s3Service, ILogger<UploadsApiController> logger) {
            _dbContext = dbContext;
            _s3Service = s3Service;
            _logger = logger;
        }

        [AdminAuthorize]
        [HttpPost]
        [Route("{rid}/logo/")]
        public async Task<ActionResult<ResultDto<string>>> UploadLogo([FromRoute] int rid, IFormFile file) {
            var restaurant = await _dbContext.Restaurants.FindAsync(rid);
            if (restaurant == null) return NotFound();
            string uploadedPath;
            try {
                uploadedPath = await _s3Service.HandleImageUpload(rid, file);
            } catch (Exception e) {
                _logger.LogError(e, "Logo upload for {Rid} failed", rid);
                return Problem("Upload failed.");
            }

            restaurant.Logo = FormatPath(uploadedPath);
            await _dbContext.SaveChangesAsync();
            return new ResultDto<string>(restaurant.Logo);
        }

        [AdminAuthorize]
        [HttpDelete]
        [Route("{rid}/logo/")]
        public async Task<IActionResult> DeleteLogo([FromRoute] int rid) {
            var restaurant = await _dbContext.Restaurants.FindAsync(rid);
            if (restaurant == null) return NotFound();
            restaurant.Logo = null;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }


        [ManageRestaurantAuthorize("rid")]
        [HttpPost]
        [Route("{rid}/menuitemphoto/")]
        public async Task<ActionResult<ResultDto<string>>> UploadMenuItemPhoto([FromRoute] int rid, IFormFile file) {
            try {
                var uploadedPath = await _s3Service.HandleImageUpload(rid, file);
                return new ResultDto<string>(FormatPath(uploadedPath));
            } catch (Exception e) {
                _logger.LogError(e, "Image upload for menu item in restaurant {Rid} failed", rid);
                return Problem("Upload failed.");
            }
        }

        [ManageRestaurantAuthorize("rid")]
        [HttpPost]
        [Route("{rid}/menuitem/{miid}/photo/")]
        public async Task<ActionResult<ResultDto<string>>> UploadMenuItemPhoto([FromRoute] int rid, [FromRoute] int miid, IFormFile file) {
            var menuItem = await _dbContext.MenuItems
                .Include(mi => mi.MenuCategory)
                .Where(mi => mi.MenuCategory.RestaurantId == rid && mi.MenuItemId == miid).FirstOrDefaultAsync();
            if (menuItem == null) return NotFound();
            string uploadedPath;
            try {
                uploadedPath = await _s3Service.HandleImageUpload(rid, file);
            } catch (Exception e) {
                _logger.LogError(e, "Image upload for menu item {Miid} failed", miid);
                return Problem("Upload failed.");
            }

            menuItem.Image = FormatPath(uploadedPath);
            await _dbContext.SaveChangesAsync();
            return new ResultDto<string>(menuItem.Image);
        }

        [ManageRestaurantAuthorize("rid")]
        [HttpDelete]
        [Route("{rid}/menuitem/{miid}/photo/")]
        public async Task<IActionResult> DeleteMenuItemPhoto([FromRoute] int rid, [FromRoute] int miid, IFormFile file) {
            var menuItem = await _dbContext.MenuItems
                .Include(mi => mi.MenuCategory)
                .Where(mi => mi.MenuCategory.RestaurantId == rid && mi.MenuItemId == miid).FirstOrDefaultAsync();
            if (menuItem == null) return NotFound();
            menuItem.Image = null;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        private static string FormatPath(string uploadedPath) {
            return "/uploads/" + uploadedPath;
        }
    }
}
