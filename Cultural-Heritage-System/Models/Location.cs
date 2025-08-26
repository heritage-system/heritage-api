using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class Location: BaseEntity<int>
    {
        [Column("province")]
        public string? Province { get; set; }

        [Column("district")]
        public string? District { get; set; }
        [Column("ward")]
        public string? Ward { get; set; }
        [Column("addressDetail")]
        public string? AddressDetail { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }

        [Column("longitude")]
        public decimal Longitude { get; set; }

        [JsonIgnore]
        public ICollection<HeritageLocation> HeritageLocations { get; set; } = new List<HeritageLocation>();

        [Column("province_unsigned")]
        public string? ProvinceUnsigned { get; set; }

        [Column("district_unsigned")]
        public string? DistrictUnsigned { get; set; }
        [Column("ward_unsigned")]
        public string? WardUnsigned { get; set; }
        [Column("address_detail_unsigned")]
        public string? AddressDetailUnsigned { get; set; }

        
        public void GenerateUnsignedFields()
        {
            ProvinceUnsigned = StringHelper.RemoveDiacritics(Province).ToLower();
            DistrictUnsigned = StringHelper.RemoveDiacritics(District).ToLower();
            WardUnsigned = StringHelper.RemoveDiacritics(Ward).ToLower();
            AddressDetailUnsigned = StringHelper.RemoveDiacritics(AddressDetail).ToLower();
        }
    }
}
