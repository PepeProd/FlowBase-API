using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using FlowBaseAPI.Dto;

namespace FlowBaseAPI.Models
{
    public class User
    {
        public User () {

        }

        public User(dtoUser newUser) {
            Username = newUser.Username;
            Email = newUser.Email;
            Notifications = newUser.Notifications;
            Password = newUser.Password;
            Frequency = newUser.Frequency;
        }
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
