using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class City(string name)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = name;
        [MaxLength(255)]
        public string? Description { get; set; }

        public ICollection<PointOfInterest> PointOfInterests { get; set; } = [];
    }
}
