﻿namespace crud_cache_vehicle.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }
        public required string Plate { get; set; }
    }
}