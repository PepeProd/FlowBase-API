using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlowBaseAPI.Dto
{
    public class dtoBarcode
    {
        [Required]
        [JsonProperty(PropertyName = "barcode")]
        public string Barcode { get; set; }

    }
}
