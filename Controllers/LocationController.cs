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
    [Route("Locations")]
    public class LocationController : Controller
    {
        private readonly FlowbaseContext _context;

        public LocationController(FlowbaseContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        [HttpGet(Name = "GetAllLocations")]
        public IActionResult GetAllLocations()
        {
            var locations = _context.Locations;
            if (locations == null)
            {
                return NoContent();
            }
            return new ObjectResult(locations);
        }

        [HttpPost(Name = "CreateLocation")]
        public async Task<IActionResult> CreateLocation([FromBody] List<Location> locations)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try {
                _context.Locations.AddRange(locations);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Created("/locations", locations);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteLocation([FromBody] Location postedLocation)
        {
            var location = _context.Locations.FirstOrDefault(u => u.Name.ToLower() == postedLocation.Name.ToLower());
            if (location == null)
            {
                return NoContent();
            }

            try {
                if (_context.Chemicals.Any(x => x.Location.ToLower() == postedLocation.Name.ToLower())) {
                    return BadRequest(_context.Chemicals.Where(x => x.Location.ToLower() == postedLocation.Name.ToLower()).ToList());
                }
                _context.Remove(location);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Ok(location);
        }
    }
}
