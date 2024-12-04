using System;
using SistemaVIP.Core.Interfaces;

namespace SistemaVIP.Infrastructure.Services
{
    public interface ILocationService
    {
        double CalculateDistance(double startLat, double startLng, double endLat, double endLng);
        bool IsWithinSameArea(double startLat, double startLng, double endLat, double endLng, double maxDistanceKm = 0.5);
    }

    public class LocationService : ILocationService
    {
        private const double EarthRadiusKm = 6371.0; // Radio de la Tierra en kilómetros

        public double CalculateDistance(double startLat, double startLng, double endLat, double endLng)
        {
            // Convertir grados a radianes
            var dLat = ToRad(endLat - startLat);
            var dLon = ToRad(endLng - startLng);
            var lat1 = ToRad(startLat);
            var lat2 = ToRad(endLat);

            // Fórmula del haversine
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c; // Distancia en kilómetros
        }

        public bool IsWithinSameArea(double startLat, double startLng, double endLat, double endLng, double maxDistanceKm = 0.5)
        {
            var distance = CalculateDistance(startLat, startLng, endLat, endLng);
            return distance <= maxDistanceKm;
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}