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

        // GET api/locations
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

        // POST api/locations
        [HttpPost(Name = "CreateLocation")]
        public async Task<IActionResult> CreateLocation([FromBody] List<Location> locations)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Locations.AddRange(locations);
            await _context.SaveChangesAsync();

            return Created("/locations", locations);
        }

        // DELETE api/locations/5
        [HttpDelete("{id}", Name = "DeleteLocation")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = _context.Locations.FirstOrDefault(u => u.Id == id);
            if (location == null)
            {
                return NoContent();
            }

            _context.Remove(location);
            await _context.SaveChangesAsync();

            return Ok(location);
        }
    }
}
