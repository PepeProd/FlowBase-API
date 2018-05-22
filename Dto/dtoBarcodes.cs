using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlowBaseAPI.Dto
{
    public class dtoBarcodes
    {
        [Required]
        [JsonProperty(PropertyName = "barcodes")]
        public List<dtoBarcode> Barcodes { get; set; }

    }
}
