using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using AgriStreamIoT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Controllers
{
    public class IrrigationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IrrigationQueueService _queueService;

        public IrrigationController(
            ApplicationDbContext context,
            IrrigationQueueService queueService)
        {
            _context = context;
            _queueService = queueService;
        }

        // ============================================================
        // TRIGGER - GET
        // Displays registered irrigation controllers.
        // ============================================================

        public async Task<IActionResult> Trigger()
        {
            ViewBag.Devices = await _context.Devices
                .Where(d => d.DeviceType == "Irrigation Controller")
                .OrderBy(d => d.DeviceName)
                .ToListAsync();

            return View();
        }

        // ============================================================
        // TRIGGER - POST
        // Places an irrigation command in Azure Queue Storage.
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Trigger(
            int deviceId,
            string command)
        {
            var device =
                await _context.Devices.FindAsync(deviceId);

            if (device == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please select a valid irrigation controller.");

                ViewBag.Devices = await _context.Devices
                    .Where(d =>
                        d.DeviceType == "Irrigation Controller")
                    .OrderBy(d => d.DeviceName)
                    .ToListAsync();

                return View();
            }

            var irrigationCommand =
                new IrrigationCommand
                {
                    DeviceId = device.Id,
                    DeviceName = device.DeviceName,
                    FieldLocation = device.FieldLocation,
                    Command = command,
                    RequestedBy = "AgriStream MVC"
                };

            await _queueService.SendCommandAsync(
                irrigationCommand);

            TempData["Success"] =
                "Irrigation command successfully placed in Queue Storage.";

            return RedirectToAction(nameof(Trigger));
        }
    }
}