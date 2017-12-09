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
    [Route("disposedChemicals")]
    public class DisposedChemicalsController : Controller
    {
        private readonly ChemicalContext _context;

        public ChemicalsController(ChemicalContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        // GET api/disposedChemicals
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

    }
}
