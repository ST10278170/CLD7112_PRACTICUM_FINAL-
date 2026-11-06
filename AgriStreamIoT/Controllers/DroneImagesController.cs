using AgriStreamIoT.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriStreamIoT.Controllers
{
    public class DroneImagesController : Controller
    {
        private readonly DroneBlobStorageService _blobStorageService;

        public DroneImagesController(
            DroneBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            IFormFile? droneImage)
        {
            if (droneImage == null ||
                droneImage.Length == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please select a drone image.");

                return View();
            }

            try
            {
                string imageUrl =
                    await _blobStorageService
                        .UploadDroneImageAsync(droneImage);

                ViewBag.ImageUrl = imageUrl;

                ViewBag.Success =
                    "Drone image uploaded successfully.";

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Upload failed: {ex.Message}");

                return View();
            }
        }
    }
}