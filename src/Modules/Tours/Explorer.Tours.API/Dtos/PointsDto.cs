﻿namespace Explorer.Tours.API.Dtos
{
    public class PointsDto
    {
        public long Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public long TourId { get; set; }
    }
}
