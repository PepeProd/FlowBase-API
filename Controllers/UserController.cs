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
        private readonly FlowbaseContext _context;

        public UserController(FlowbaseContext context)
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
                return NoContent();
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
            
            try {
                _context.Users.AddRange(Users);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Created("/Users", Users);
        }

        [Route("Users/ValidateUser/")]
        [HttpPost(Name = "ValidateUser")]
        public IActionResult ValidateUser([FromBody] User UserCredentials) {
            //not sure if i need this
            var users = _context.Users;
            if(_context.Users == null)
            {
                return NoContent();
            }

            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            if (_context.Users.Any(u => u.Username == UserCredentials.Username && u.Password == UserCredentials.Password))
            {
                var userToValidate = _context.Users.FirstOrDefault(u => u.Username == UserCredentials.Username && u.Password == UserCredentials.Password);
                UserCredentials.Email = userToValidate.Email;
                return Ok(UserCredentials);
            }
            else
                return Unauthorized();
        }

        // DELETE api/Users/5
        [HttpDelete("{id}", Name = "DeleteUsers")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = _context.Users.FirstOrDefault(u => u.Id == id);
            if (User == null)
            {
                return NoContent();
            }

            try {
                _context.Users.Remove(User);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Ok(User);
        }
    }
}