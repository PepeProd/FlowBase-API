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
    [Route("Users")]
    public class UserController : Controller
    {
        private readonly ChemicalContext _context;

        public ChemicalsController(ChemicalContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        // GET api/Users
        [HttpGet(Name = "GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var Users = _context.Users;
            if (Users == null)
            {
                return NotFound();
            }
            return new ObjectResult(Users);
        }

        // POST api/Users
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] List<User> Users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Users.AddRange(Users);
            await _context.SaveChangesAsync();

            return Created("/Users", Users);
        }

        // DELETE api/Users/5
        [HttpDelete("{id}", Name = "DeleteUsers")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = _context.Users.FirstOrDefault(u => u.Id == id);
            if (User == null)
            {
                return NotFound();
            }

            _context.Users.Remove(User);
            await _context.Users.SaveChangesAsync();

            return Ok(User);
        }
    }
}