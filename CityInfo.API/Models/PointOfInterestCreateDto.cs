// Ignore Spelling: Dto

using System;
using System.ComponentModel.DataAnnotations;
namespace CityInfo.API.Models;

public class PointOfInterestCreateDto
{
    [Required(ErrorMessage = "Please provide a valid name")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
}
