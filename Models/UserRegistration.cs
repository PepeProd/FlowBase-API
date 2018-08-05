using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Linq;

namespace FlowBaseAPI.Models
{
    public class UserRegistration
    {
        public UserRegistration() {

        }
        public UserRegistration(string newEmail) {
            this.Email = newEmail;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 7)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            this.RegistrationCode = code;
        }

        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "email")]
        public string Email {get; set;}

        public string RegistrationCode { get; set; }
    }
}