using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Dto
{
    public class dtoChemFamily
    {
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "chemical_name")]
        public string ChemicalName { get; set; }

        [JsonProperty(PropertyName="threshold")]
        public int reorderThreshold {get; set;}

        [JsonProperty(PropertyName="reorder_quantity")]
        public int ReorderQuantity {get; set;}

        [JsonProperty(PropertyName="quantity")]
        public int Quantity {get; set;}

    }
}
