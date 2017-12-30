using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Models
{
    public class MetaData
    {
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "max_barcode")]
        public long MaxBarcode { get; set; }
        
       
    }
}
