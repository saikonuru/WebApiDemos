using System;
using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }

    //public static CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        Cities =
        [
            new() {Id =1,Name="Hyderabad",Description ="Historic City;Bhagaya Nagar", PointOfInterest = [
                new() {
                    Id=1,
                    Name ="Golkonda",
                    Description = "Golkonda khilla"
                },
                new() {
                    Id=2,
                    Name ="Charminar",
                    Description = "Charminar khilla"
                }
            ] },
            new() {Id =2,Name="Visakhapatnam",Description ="Vizag;Beach City", PointOfInterest = [
                new() {
                    Id=3,
                    Name ="Beach",
                    Description = "Sand Beach"
                },
                new() {
                    Id=4,
                    Name ="Simhachalam",
                    Description = "Simhachalam temple"
                }
            ] }
        ];
    }
}
