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
        private readonly ChemicalContext _context;

        public ChemicalsController(ChemicalContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        // GET api/TempZones
        [HttpGet(Name = "GetAllTempZones")]
        public IActionResult GetAllTempZones()
        {
            var TempZones = _context.TempZones;
            if (TempZones == null)
            {
                return NotFound();
            }
            return new ObjectResult(TempZones);
        }

        // POST api/TempZone
        [HttpPost(Name = "CreateTempZone")]
        public async Task<IActionResult> CreateTempZone([FromBody] List<TempZone> TempZones)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.TempZones.AddRange(TempZones);
            await _context.SaveChangesAsync();

            return Created("/TempZones", TempZones);
        }

        // DELETE api/TempZones/5
        [HttpDelete("{id}", Name = "DeleteTempZones")]
        public async Task<IActionResult> DeleteTempZone(int id)
        {
            var TempZone = _context.TempZones.FirstOrDefault(u => u.Id == id);
            if (TempZone == null)
            {
                return NotFound();
            }

            _context.TempZones.Remove(TempZone);
            await _context.TempZones.SaveChangesAsync();

            return Ok(TempZone);
        }
    }
}