using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
    }
}
