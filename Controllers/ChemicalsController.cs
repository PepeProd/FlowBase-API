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
    [Route("chemicals")]
    public class ChemicalsController : Controller
    {
        private readonly FlowbaseContext _context;

        public ChemicalsController(FlowbaseContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        // GET api/chemicals
        [HttpGet(Name = "GetAllChemicals")]
        public IActionResult GetAllChemicals()
        {
            var chemicals = _context.Chemicals;
            if (chemicals == null)
            {
                return NoContent();
            }
            return new ObjectResult(chemicals);
        }

        // GET api/chemicals/5
        [HttpGet("{id}", Name = "GetChemical")]
        public IActionResult GetChemical(int id)
        {
            var chemical = _context.Chemicals.FirstOrDefault(u => u.Id == id);
            if (chemical == null)
            {
                return NoContent();
            }
            return new ObjectResult(chemical);
        }

        // POST api/chemicals
        [HttpPost(Name = "CreateChemicals")]
        public async Task<IActionResult> CreateChemicals([FromBody] List<Chemical> chemicals)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //TODO: Store this on DB itself
            //find highest barcode
            var highestBarcode = _context.Chemicals.Any() ? _context.Chemicals.Max(u => u.Barcode) : Numbers.FirstBarcode;

            //set barcode
            foreach (var chemical in chemicals)
            {
                chemical.Barcode = ++highestBarcode;
            }

            _context.Chemicals.AddRange(chemicals);
            await _context.SaveChangesAsync();

            return Created("/chemicals", chemicals);
        }

        // PUT api/chemicals/5
        //[HttpPut("{id}", Name = "UpdateChemical")]
        //public async Task<IActionResult> UpdateChemical(int id, [FromBody] Chemical chemical)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    chemical.Id = id;
        //    _context.Chemicals.Update(chemical);
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}

        //change to post and add developer delete from database
        // DELETE api/chemicals/5
        [HttpDelete("{barcode}", Name = "DeleteChemical")]
        public async Task<IActionResult> DeleteChemical(string barcode)
        {
            long barcodeAsLong = 0;
            var isInt64 = Int64.TryParse(barcode, out barcodeAsLong);
            if (isInt64) {
                var chemical = _context.Chemicals.FirstOrDefault(u => u.Barcode == barcodeAsLong);
                if (chemical == null)
                {
                    return NoContent();
                }

                _context.Chemicals.Remove(chemical);

                _context.DisposedChemicals.Add(chemical);
                await _context.SaveChangesAsync();

                return Ok(chemical);
            } else {
                return BadRequest();
            }
        }
    }
}
