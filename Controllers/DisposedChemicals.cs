using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlowBaseAPI.DataLayer;
using FlowBaseAPI.Globals;
using FlowBaseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowBaseAPI.Controllers
{
    [Route("chemicals/disposed")]
    public class DisposedChemicalsController : Controller
    {
        private readonly FlowbaseContext _context;

        public DisposedChemicalsController(FlowbaseContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        [HttpGet(Name = "GetAllDisposedChemicals")]
        public IActionResult GetAllDisposedChemicals()
        {
            var disposedChemicals = _context.DisposedChemicals;
            if (disposedChemicals == null)
            {
                return NotFound();
            }
            return new ObjectResult(disposedChemicals);
        }

         [HttpGet("top/{mostRecent}")]
         public IActionResult GetMostRecentDisposedChemical(int mostRecent)
        {
            IQueryable disposedChemicals;
            if (_context.DisposedChemicals == null)
            {
                return NotFound();
            }

            if (_context.DisposedChemicals.Any()){
                disposedChemicals = _context.DisposedChemicals.OrderByDescending(x => x.DisposalDate).Take(mostRecent);
                return new ObjectResult(disposedChemicals);
            }

            return BadRequest();            
        }
        
        [HttpGet("dates")]
         public IActionResult GetDisposedByDateRange([FromQuery] string toDate, [FromQuery] string fromDate)
        {
            IQueryable disposedChemicals;
            if (_context.DisposedChemicals == null)
            {
                return NotFound();
            }
            DateTime _toDate;
            DateTime _fromDate;

            var toDateConversion = DateTime.TryParse(toDate, out _toDate);
            var fromDateConversion = DateTime.TryParse(fromDate, out _fromDate);

            if (toDateConversion && fromDateConversion)
            {
                var endOfDayToDate = _toDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                if (_context.DisposedChemicals.Any()){
                    disposedChemicals = _context.DisposedChemicals.Where(x => x.DisposalDate >= _fromDate && x.DisposalDate <= endOfDayToDate);
                    return new ObjectResult(disposedChemicals);
                }
            }

            return BadRequest();            
        }



    }
}
