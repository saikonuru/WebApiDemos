using System;

namespace CityInfo.API.Models;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int NumberOfPointOfInterest => PointOfInterest.Count();
    public IEnumerable<PointOfInterestDto> PointOfInterest { get; set; } = [];
}
