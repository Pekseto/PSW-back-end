﻿using System.Text.Json.Serialization;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.TourExecutions
{
    public class Position: ValueObject
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastActivity { get; set; }

        [JsonConstructor]
        public Position(double lat, double lng, DateTime lastActivity)
        {
            Validate(lat, lng, lastActivity);
            Latitude = lat;
            Longitude = lng;
            LastActivity = lastActivity;
        }


        public bool IsChanged(Position currentPosition)
        {
            return (Latitude != currentPosition.Latitude) || (Longitude != currentPosition.Longitude);
        }
        
        private static void Validate(double lat, double lng, DateTime lastActivity)
        {
            if (!(lat is > -90.0 and < 90.0))
                throw new ArgumentException("Exeception! Latitude is out of range!");
            if (!(lng is > -180.0 and < 180.0))
                throw new ArgumentException("Exeception! Longitude is out of range!");
            if (lastActivity < DateTime.Now)
                throw new ArgumentException("Exception! Time not acceptable!");
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
            yield return LastActivity;
        }
    }
}
