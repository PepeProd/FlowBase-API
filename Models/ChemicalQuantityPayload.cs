using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Models
{
    public class ChemicalQuantityByPayload
    {
        [Required]
        [JsonProperty(PropertyName = "newChemical")]
        public Chemical NewChemical { get; set; }

        [Required]
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

    }
}