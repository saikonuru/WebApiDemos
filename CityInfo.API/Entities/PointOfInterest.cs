using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace CityInfo.API.Entities
{
    public class PointOfInterest(string name)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = name;


        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }
        public int CityId { get; set; }
    }
}
