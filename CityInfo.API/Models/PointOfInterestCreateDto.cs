// Ignore Spelling: Dto

using System;

namespace CityInfo.API.Models;

public class PointOfInterestCreateDto
{

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
