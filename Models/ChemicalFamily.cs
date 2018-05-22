using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Models
{
    public class ChemicalFamily
    {
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "chemical_name")]
        public string ChemicalName { get; set; }

        [Required]
        [JsonProperty(PropertyName="threshold")]
        public int reorderThreshold {get; set;}

        [Required]
        [JsonProperty(PropertyName="reorder_quantity")]
        public int ReorderQuantity {get; set;}

        [Required]
        [JsonProperty(PropertyName="quantity")]
        public int Quantity {get; set;}

    }
}
