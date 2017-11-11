using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlowBaseAPI.Models
{
    public class Chemical
    {
        public int Id { get; set; }
        public long Barcode { get; set; }

        [Required]
        [JsonProperty(PropertyName = "chemical_name")]
        public string ChemicalName { get; set; }
        [Required]
        [JsonProperty(PropertyName = "common_name")]
        public string CommonName { get; set; }
        [Required]
        [JsonProperty(PropertyName = "siemens_material_number")]
        public string SiemensMaterialNumber { get; set; }
        [Required]
        [JsonProperty(PropertyName = "vendor_name")]
        public string VendorName { get; set; }
        [Required]
        [JsonProperty(PropertyName = "lot_number")]
        public string LotNumber { get; set; }
        [Required]
        [JsonProperty(PropertyName = "receive_date")]
        public DateTime ReceiveDate { get; set; }
        [Required]
        [JsonProperty(PropertyName = "expiration_date")]
        public DateTime ExpirationDate { get; set; }
        [Required]
        [JsonProperty(PropertyName = "project_code")]
        public string ProjectCode{ get; set; }
        [Required]
        [JsonProperty(PropertyName = "storage_temperature")]
        public string StorageTemperature{ get; set; }
        [Required]
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
    }
}
