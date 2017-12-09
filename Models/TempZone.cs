using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Models
{
    public class TempZone
    {
        public int Id { get; set; }
        [Required]
        [JsonProperty(PropertyName = "storage_temperature")]
        public string StorageTemperature{ get; set; }
    }
}
