using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlowBaseAPI.DataLayer;
using FlowBaseAPI.Globals;
using FlowBaseAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlowBaseAPI.Controllers
{
    [Route("TempZones")]
    public class TempZoneController : Controller
    {
        private readonly FlowbaseContext _context;

        public TempZoneController(FlowbaseContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        [HttpGet(Name = "GetAllTempZones")]
        public IActionResult GetAllTempZones()
        {
            var TempZones = _context.TempZones;
            if (TempZones == null)
            {
                return NoContent();
            }
            return new ObjectResult(TempZones);
        }

        [HttpPost(Name = "CreateTempZone")]
        public async Task<IActionResult> CreateTempZone([FromBody] List<TempZone> TempZones)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try {
                _context.TempZones.AddRange(TempZones);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Created("/TempZones", TempZones);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTempZone([FromBody] TempZone tempZoneName)
        {
            var TempZone = _context.TempZones.FirstOrDefault(u => u.StorageTemperature == tempZoneName.StorageTemperature);
            if (TempZone == null)
            {
                return NoContent();
            }

            try {
                if (_context.Chemicals.Any(x => x.StorageTemperature.ToLower() == tempZoneName.StorageTemperature.ToLower())) {
                    return BadRequest(_context.Chemicals.Where(x => x.StorageTemperature.ToLower() == tempZoneName.StorageTemperature.ToLower()).ToList());
                }
                _context.TempZones.Remove(TempZone);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Ok(TempZone);
        }
    }
}