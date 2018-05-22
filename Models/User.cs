using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [Required]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [Required]
        [JsonProperty(PropertyName = "notifications")]
        public bool Notifications { get; set; }
        [Required]
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [Required]
        [JsonProperty(PropertyName="frequency")]
        public string Frequency {get; set;}

    }
}
