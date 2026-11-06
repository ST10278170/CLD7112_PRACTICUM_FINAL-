using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Controllers
{
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // INDEX
        // Displays all registered AgriStream IoT devices.
        // ============================================================

        public async Task<IActionResult> Index()
        {
            var devices = await _context.Devices
                .OrderBy(d => d.DeviceName)
                .ToListAsync();

            return View(devices);
        }

        // ============================================================
        // DETAILS
        // ============================================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // ============================================================
        // CREATE - GET
        // ============================================================

        public IActionResult Create()
        {
            return View();
        }

        // ============================================================
        // CREATE - POST
        // Registers a device in the SQL master registry.
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("DeviceName,DeviceType,FieldLocation,OperationalStatus")]
            Device device)
        {
            if (!ModelState.IsValid)
            {
                return View(device);
            }

            device.LastUpdated = DateTime.UtcNow;

            _context.Devices.Add(device);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Device registered successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // EDIT - GET
        // ============================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // ============================================================
        // EDIT - POST
        // Updates the device master registry.
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,DeviceName,DeviceType,FieldLocation,OperationalStatus")]
            Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(device);
            }

            var existingDevice =
                await _context.Devices.FindAsync(id);

            if (existingDevice == null)
            {
                return NotFound();
            }

            try
            {
                existingDevice.DeviceName = device.DeviceName;
                existingDevice.DeviceType = device.DeviceType;
                existingDevice.FieldLocation = device.FieldLocation;
                existingDevice.OperationalStatus =
                    device.OperationalStatus;

                existingDevice.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Device updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(device.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // DELETE - GET
        // ============================================================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // ============================================================
        // DELETE - POST
        // ============================================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Device deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // DEVICE EXISTS
        // ============================================================

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(
                d => d.Id == id);
        }
    }
}