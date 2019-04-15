﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DistanceMatrixTest.Model
{
    /*
     * Points Generated by: http://www.geomidpoint.com/random/
     */

    public class MatrixGPSPoints
    {
        private int _quantityOfOriginsAndDestinations;

        private List<GPSLatLng> _originPoints = new List<GPSLatLng>();
        private List<GPSLatLng> _destinationPoints = new List<GPSLatLng>();

        public List<GPSLatLng> OriginPoints { get => _originPoints; set => _originPoints = value; }
        public List<GPSLatLng> DestinationPoints { get => _destinationPoints; set => _destinationPoints = value; }

        public int QuantityOfOriginsAndDestinations { get => _quantityOfOriginsAndDestinations; set => _quantityOfOriginsAndDestinations = value; }

        public class GPSLatLng
        {
            private double _latitude;
            private double _longitude;

            public GPSLatLng(double latitude, double longitude)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
            }

            public GPSLatLng(string latitude, string longitude)
            {
                try
                {
                    this.Latitude = Convert.ToDouble(latitude);
                    this.Longitude = Convert.ToDouble(longitude);
                }
                catch (Exception e)
                {
                    this.Latitude = 0;
                    this.Longitude = 0;
                }
            }

            public double Latitude { get => _latitude; set => _latitude = value; }
            public double Longitude { get => _longitude; set => _longitude = value; }
        }

        public class DistDurData
        {
            private double _distance;
            private double _duration;

            public double Distance { get => _distance; set => _distance = value; }
            public double Duration { get => _duration; set => _duration = value; }
        }

        public class DistDurText
        {
            private string _distance;
            private string _duration;

            public string Distance { get => _distance; set => _distance = value; }
            public string Duration { get => _duration; set => _duration = value; }
        }

        public MatrixGPSPoints(int quantityOfOriginsAndDestinations)
        {
            QuantityOfOriginsAndDestinations = quantityOfOriginsAndDestinations;
            PopulateOriginsAndDestinations(quantityOfOriginsAndDestinations);
        }

        /// <summary>
        /// Populate two lists of N GPS points from an internal CSV file that contains 500 random GPS points.
        /// </summary>
        /// <param name="quantityOfOriginsAndDestinations"></param>
        public void PopulateOriginsAndDestinations(int quantityOfOriginsAndDestinations = 10)
        {
            if (quantityOfOriginsAndDestinations > 500)
                quantityOfOriginsAndDestinations = 500;

            int currentQuantity = 0;
            using (var reader = new StreamReader(@"D:\projects\dotNet\DistanceMatrixGPS\DistanceMatrixTest\Data\gps_500_points.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(",");


                    GPSLatLng gpsLatLng = new GPSLatLng(values[0], values[1]);

                    if (gpsLatLng.Latitude == 0) continue;

                    OriginPoints.Add(gpsLatLng);
                    DestinationPoints.Add(gpsLatLng);

                    ++currentQuantity;

                    if (currentQuantity >= quantityOfOriginsAndDestinations) break;
                }
            }
        }

    }
}
