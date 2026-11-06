using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using AgriStreamIoT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Controllers
{
    public class TelemetryController : Controller
    {
        private readonly TelemetryTableService _telemetryService;
        private readonly ApplicationDbContext _context;

        public TelemetryController(
            TelemetryTableService telemetryService,
            ApplicationDbContext context)
        {
            _telemetryService = telemetryService;
            _context = context;
        }

        // ============================================================
        // INDEX
        // Displays all sensor telemetry stored in Table Storage.
        // ============================================================

        public async Task<IActionResult> Index()
        {
            var telemetry =
                await _telemetryService.GetTelemetryAsync();

            return View(telemetry);
        }

        // ============================================================
        // CREATE - GET
        // ============================================================

        public async Task<IActionResult> Create()
        {
            ViewBag.Devices = await _context.Devices
                .OrderBy(d => d.DeviceName)
                .ToListAsync();

            return View();
        }

        // ============================================================
        // CREATE - POST
        // Simulates an IoT field device producing telemetry.
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SensorTelemetry telemetry)
        {
            var device =
                await _context.Devices.FindAsync(
                    telemetry.DeviceId);

            if (device == null)
            {
                ModelState.AddModelError(
                    "DeviceId",
                    "Please select a valid registered device.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Devices = await _context.Devices
                    .OrderBy(d => d.DeviceName)
                    .ToListAsync();

                return View(telemetry);
            }

            telemetry.DeviceName = device!.DeviceName;
            telemetry.FieldLocation = device.FieldLocation;

            await _telemetryService.AddTelemetryAsync(
                telemetry);

            TempData["Success"] =
                "Sensor telemetry recorded successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
