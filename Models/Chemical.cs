using System;
using System.ComponentModel.DataAnnotations;

namespace FlowBaseAPI.Models
{
    public class Chemical
    {
        public int Id { get; set; }
        public long Barcode { get; set; }

        [Required]
        public string ChemicalName { get; set; }
        [Required]
        public string CommonName { get; set; }
        [Required]
        public string SiemensMaterialNumber { get; set; }
        [Required]
        public string VendorName { get; set; }
        [Required]
        public string LotNumber { get; set; }
        [Required]
        public DateTime ReceiveDate { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        public string ProjectCode{ get; set; }
        [Required]
        public string StorageTemperature{ get; set; }
        [Required]
        public string Location { get; set; }
    }
}
