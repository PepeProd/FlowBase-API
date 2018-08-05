using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlowBaseAPI.DataLayer;
using FlowBaseAPI.Globals;
using FlowBaseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using FlowBaseAPI.util;
using FlowBaseAPI.Dto;

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

        [HttpPost("/Users/CreateUser/", Name = "CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] List<dtoUser> Users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userList = new List<User>();
            foreach(var user in Users) {
                if (_context.UserRegistration.Any(u => u.Email == user.Email)) {
                    var registrationCode = _context.UserRegistration.FirstOrDefault(u => u.Email == user.Email).RegistrationCode;
                    if (registrationCode == user.registrationCode) {
                        var newUser = new User(user);
                        userList.Add(newUser);
                    }
                    else {
                        return BadRequest();
                    }
                } else {
                    return BadRequest();
                }
            }
            try {
                _context.Users.AddRange(userList);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Created("/Users", Users);
        }

        [HttpPost("/Users/UpdateUserEmailFrequency", Name="UpdateUserEmailFrequency")]
        public IActionResult UpdateUserEmailFrequency([FromBody] User UserInfo) {
            if (_context.Users.Any(u => u.Username == UserInfo.Username)) {
                _context.Users.FirstOrDefault(u => u.Username == UserInfo.Username).Frequency = UserInfo.Frequency;
                _context.SaveChanges();
                return Ok(UserInfo);
            }

            return BadRequest("No user found");

        }

        [HttpPost("/Users/UpdateUserPassword", Name="UpdateUserPassword")]
        public IActionResult UpdateUserPassword([FromBody] User UserInfo) {
            if (_context.Users.Any(u => u.Username == UserInfo.Username)) {
                _context.Users.FirstOrDefault(u => u.Username == UserInfo.Username).Password = UserInfo.Password;
                _context.SaveChanges();
                return Ok(UserInfo);
            }

            return BadRequest("No user found");

        } 

        [HttpPost("/Users/ValidateUser/", Name = "ValidateUser")]
        public IActionResult ValidateUser([FromBody] User UserCredentials) {

            var users = _context.Users;
            if(_context.Users == null)
            {
                return NoContent();
            }

            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            if (_context.Users.Any(u => u.Username.ToLower() == UserCredentials.Username.ToLower() && u.Password == UserCredentials.Password))
            {
                var userToValidate = _context.Users.FirstOrDefault(u => u.Username.ToLower() == UserCredentials.Username.ToLower() && u.Password == UserCredentials.Password);
                UserCredentials.Email = userToValidate.Email;
                UserCredentials.Frequency = userToValidate.Frequency;
                return Ok(UserCredentials);
            }
            else
                return Unauthorized();
        }

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

        [HttpGet("/Users/GetAllWhitelistEmails", Name="GetAllWhitelistEmails")]
        public IActionResult GetAllWhitelistEmails() {

            var allWhiteListEmails = _context.UserRegistration.ToList().Select(u => new { u.Id, u.Email}).ToList();
            //.Select(u => { u.RegistrationCode = ""; return u;}).ToList();

            return Ok(allWhiteListEmails);
        }

        [HttpPost("/Users/NewWhitelist", Name="NewWhitelist")]
        public IActionResult NewWhitelist([FromBody] UserRegistration newEmail) {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            var newUserRegistrationWhitelist = new UserRegistration(newEmail.Email);
            _context.UserRegistration.Add(newUserRegistrationWhitelist);
            _context.SaveChanges();
            return Ok(newUserRegistrationWhitelist);
        }

        [HttpPost("/Users/SendRegistrationCode", Name="SendRegistrationCode")]
        public IActionResult SendRegistrationCode([FromBody] UserRegistration sendToEmail) {
            var registrationCode = _context.UserRegistration.FirstOrDefault(u => u.Email == sendToEmail.Email)?.RegistrationCode;
            if (registrationCode == null) {
                return BadRequest();
            }
            var emailMessageBody = "Your registration code is " + registrationCode;
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.googlemail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            var emailer = new NetworkCredentials();
            client.Credentials = new NetworkCredential(emailer.NetworkUserEmail, emailer.NetworkUserPassword);
            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(emailer.NetworkUserEmail);
            msg.To.Add(new MailAddress(sendToEmail.Email));
            msg.Subject = "FlowBase Registration Code";
            msg.Body = emailMessageBody;
            client.Send(msg);

            return Ok(registrationCode);
        }
    }
}