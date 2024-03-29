﻿using System;
using System.Collections.Generic;
using System.IO;
using BingMapsRESTToolkit;

namespace DistanceMatrixTest.Model
{
    class BingDistanceMatrix
    {
        static private string _ApiKey;

        private MatrixGPSPoints _matrixGPSPoints;

        public static int SizeOfSubMatrix = 10;

        public BingDistanceMatrix(MatrixGPSPoints matrixGPSPoints)
        {
            this._matrixGPSPoints = matrixGPSPoints;

            _ApiKey = System.Configuration.ConfigurationManager.AppSettings.Get("BingAPIKey");
            //Console.WriteLine("BingKey: {0}", _ApiKey);
        }

        static private Resource[] GetResourcesFromRequest(BaseRestRequest rest_request)
        {
            var r = ServiceManager.GetResponseAsync(rest_request).GetAwaiter().GetResult();

            if (!(r != null && r.ResourceSets != null &&
                r.ResourceSets.Length > 0 &&
                r.ResourceSets[0].Resources != null &&
                r.ResourceSets[0].Resources.Length > 0))

                throw new Exception("No results found.");

            return r.ResourceSets[0].Resources;
        }

        private List<SimpleWaypoint> PopulateWayPoints(List<MatrixGPSPoints.GPSLatLng> gpsPoints)
        {
            List<SimpleWaypoint> wayPoints = new List<SimpleWaypoint>();

            foreach (var gpsPoint in gpsPoints)
            {
                wayPoints.Add(new SimpleWaypoint(gpsPoint.Latitude, gpsPoint.Longitude));
            }

            return wayPoints;
        }

        private List<SimpleWaypoint> PopulateSegmentWayPoints(List<MatrixGPSPoints.GPSLatLng> gpsPoints, int fromPoint, int toPoint)
        {
            List<SimpleWaypoint> wayPoints = new List<SimpleWaypoint>();

            if (fromPoint < 0 || toPoint > gpsPoints.Count) return null;

            for (int i = fromPoint; i < toPoint; i++)
            {
                wayPoints.Add(new SimpleWaypoint(gpsPoints[i].Latitude, gpsPoints[i].Longitude));
            }

            Console.WriteLine("\nGPS Points to Bing (SimpleWaypoint)");
            gpsPoints.ForEach(item => Console.Write(item.Latitude + "," + item.Longitude + " / "));
            Console.WriteLine("\nWay Points from: {0} to: {1}", fromPoint, toPoint);
            wayPoints.ForEach(item => Console.Write(item.Coordinate.ToString() + " / "));

            return wayPoints;
        }

        /// <summary>
        ///  Location Recognition Test
        ///  
        /// https://docs.microsoft.com/en-us/bingmaps/rest-services/routes/calculate-a-distance-matrix
        /// </summary>
        public void DistantMatrixTest()
        {
            Console.WriteLine("Running Distance Matrix Test");

            var request = new DistanceMatrixRequest()
            {
                BingMapsKey = _ApiKey,

                Origins = new List<SimpleWaypoint>
                {
                    new SimpleWaypoint(47.58162076,7.18289976),
                    new SimpleWaypoint(46.63146309,4.23322802),
                    new SimpleWaypoint(47.60999809,2.47722561),
                    new SimpleWaypoint(48.63594493,11.13342617),
                    new SimpleWaypoint(45.00055829,8.43115231),
                    new SimpleWaypoint(46.1774775,4.34958805),
                    new SimpleWaypoint(50.22820419,2.78969141),
                    new SimpleWaypoint(46.47207431,11.16166495),
                    new SimpleWaypoint(48.40098444,5.27661128),
                    new SimpleWaypoint(45.91952505,4.23231255)
                },
                Destinations = new List<SimpleWaypoint>
                {
                    new SimpleWaypoint(47.58162076,7.18289976),
                    new SimpleWaypoint(46.63146309,4.23322802),
                    new SimpleWaypoint(47.60999809,2.47722561),
                    new SimpleWaypoint(48.63594493,11.13342617),
                    new SimpleWaypoint(45.00055829,8.43115231),
                    new SimpleWaypoint(46.1774775,4.34958805),
                    new SimpleWaypoint(50.22820419,2.78969141),
                    new SimpleWaypoint(46.47207431,11.16166495),
                    new SimpleWaypoint(48.40098444,5.27661128),
                    new SimpleWaypoint(45.91952505,4.23231255)
                },

                TravelMode = TravelModeType.Driving, //TravelModeType.Truck
                DistanceUnits = DistanceUnitType.Kilometers,
                TimeUnits = TimeUnitType.Minute,
                Resolution = 1
            };

            //var response = request.Execute();

            var resources = GetResourcesFromRequest(request);

            var r = (resources[0] as DistanceMatrix);

            foreach (var cell in r.Results)
            {
                Console.WriteLine("OriginIndex: {0}, DestinationIndex: {1}, TravelDistance: {2}, TravelDuration: {3}", 
                                    cell.OriginIndex, cell.DestinationIndex, cell.TravelDistance, cell.TravelDuration);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Bing: Calculates the distances and times to travel from many origins to many destinations
        /// </summary>
        public void DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses()
        {
            DistanceMatrixRequest request = new DistanceMatrixRequest
            {
                BingMapsKey = _ApiKey,

                Origins = PopulateWayPoints(_matrixGPSPoints.OriginPoints),
                Destinations = PopulateWayPoints(_matrixGPSPoints.DestinationPoints),

                TravelMode = TravelModeType.Driving, //TravelModeType.Truck
                DistanceUnits = DistanceUnitType.Kilometers,
                TimeUnits = TimeUnitType.Minute,
                Resolution = 1
            };

            var resources = GetResourcesFromRequest(request);

            //PrintResponseList(resources);
            SimplePrintResponseMatrix(resources);

            Console.ReadLine();
        }

        /// <summary>
        /// Bing: Calculates the distances and times to travel from many origins to many destinations using many requests to avoid the limitation of (50 x 50). But uses a size of 10 x 10.
        /// </summary>
        public void DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted()
        {
            int splits = (_matrixGPSPoints.QuantityOfOriginsAndDestinations + 9) / SizeOfSubMatrix; //Equivalent to Math.Ceiling
            Console.WriteLine("Number of splits {0}", splits);

            for (int i = 0; i < splits; i++)
            {

                int fromOriginPoint = i * SizeOfSubMatrix;
                int toOriginPoint;//(_matrixGPSPoints.QuantityOfOriginsAndDestinations < fromOriginPoint + SizeOfSubMatrix ? _matrixGPSPoints.QuantityOfOriginsAndDestinations : fromOriginPoint + SizeOfSubMatrix);

                if ((i == splits - 1) && (_matrixGPSPoints.QuantityOfOriginsAndDestinations % SizeOfSubMatrix != 0)) //if it is the last segment                
                    toOriginPoint = _matrixGPSPoints.QuantityOfOriginsAndDestinations - fromOriginPoint;                
                else
                    toOriginPoint = fromOriginPoint + SizeOfSubMatrix;

                Console.WriteLine("Origin From: {0} To: {1}", fromOriginPoint, toOriginPoint);

                for (int j = 0; j < splits; j++)
                {
                    Console.WriteLine("Request {0} - {1}", i, j);

                    int fromDestinationPoint = j * SizeOfSubMatrix;
                    int toDestinationPoint = fromDestinationPoint + SizeOfSubMatrix;

                    DistanceMatrixRequest request = new DistanceMatrixRequest
                    {
                        BingMapsKey = _ApiKey,

                        Origins = PopulateSegmentWayPoints(_matrixGPSPoints.OriginPoints, fromOriginPoint, toOriginPoint),
                        Destinations = PopulateSegmentWayPoints(_matrixGPSPoints.DestinationPoints, fromDestinationPoint, toDestinationPoint),

                        TravelMode = TravelModeType.Driving, //TravelModeType.Truck
                        DistanceUnits = DistanceUnitType.Kilometers,
                        TimeUnits = TimeUnitType.Minute,
                        Resolution = 1
                    };

                    var resources = GetResourcesFromRequest(request);

                    //SimplePrintResponseMatrix(resources);
                    AddToResponseMatrix(resources, fromOriginPoint, fromDestinationPoint);
                }
            }

            _matrixGPSPoints.PrintDistDurTextMatrix("Bing");


        }

        private void PrintResponseList(Resource[] resources)
        {
            var response = (resources[0] as DistanceMatrix);

            foreach (var cell in response.Results)
            {
                Console.WriteLine("==>> Origin: {0}, {1} to Destination: {2}, {3} >> Distance: {4} Duration: {5} ", 
                    _matrixGPSPoints.OriginPoints[cell.OriginIndex].Latitude, _matrixGPSPoints.OriginPoints[cell.OriginIndex].Longitude,
                    _matrixGPSPoints.DestinationPoints[cell.DestinationIndex].Latitude, _matrixGPSPoints.DestinationPoints[cell.DestinationIndex].Longitude,
                    cell.TravelDistance, cell.TravelDuration);
            }
        }

        private void SimplePrintResponseMatrix(Resource[] resources)
        {
            var response = (resources[0] as DistanceMatrix);

            string[,] response_matrix = new string[_matrixGPSPoints.QuantityOfOriginsAndDestinations + 1, _matrixGPSPoints.QuantityOfOriginsAndDestinations + 1];

            //Console.WriteLine(response.Status);
            Console.WriteLine(response.ErrorMessage);
            Console.WriteLine("Rows returned: " + response.Results.Length);

            foreach (var cell in response.Results)
            {
                //Print the origins
                response_matrix[cell.OriginIndex + 1, 0] = _matrixGPSPoints.OriginPoints[cell.OriginIndex].Latitude + ", " + _matrixGPSPoints.OriginPoints[cell.OriginIndex].Longitude;

                //Print the destinies
                response_matrix[0, cell.DestinationIndex + 1] = _matrixGPSPoints.DestinationPoints[cell.DestinationIndex].Latitude + ", " + _matrixGPSPoints.DestinationPoints[cell.DestinationIndex].Longitude;

                response_matrix[cell.OriginIndex + 1, cell.DestinationIndex + 1] = "Dist: " + cell.TravelDistance + " / Dur: " + TimeSpan.FromMinutes(cell.TravelDuration).ToString(@"hh\:mm");

            }

            using (StreamWriter outfile = new StreamWriter(MatrixGPSPoints.OutputPath + "BingDistanceMatrix_N-" + _matrixGPSPoints.QuantityOfOriginsAndDestinations + "_" + DateTime.Now.ToString("yyMMddHHmmss.fff") + ".csv"))
            {
                for (int row = 0; row < _matrixGPSPoints.OriginPoints.Count + 1; row++)
                {
                    string content = "";
                    for (int col = 0; col < _matrixGPSPoints.DestinationPoints.Count + 1; col++)
                    {
                        content += response_matrix[row, col] + ";"; //For CSV print
                        Console.Write(String.Format("{0}\t", response_matrix[row, col])); //For console print
                    }
                    //trying to write data to csv
                    outfile.WriteLine(content);
                    Console.WriteLine();
                }


            }
        }

        private void AddToResponseMatrix(Resource[] resources, int fromOrigPos = 0, int fromDestPos = 0)
        {
            var response = (resources[0] as DistanceMatrix);

            //int i_orig = fromOrigPos, i_dest = fromDestPos;

            //string[,] response_matrix = new string[request.WaypointsDestination.Count + 1, request.WaypointsOrigin.Count + 1];

            //Console.WriteLine(response.Status);
            Console.WriteLine(response.ErrorMessage);
            Console.WriteLine("Rows returned: " + response.Results.Length);

            foreach (var cell in response.Results)
            {
                if (cell.HasError) continue;

                _matrixGPSPoints.DistDurValueMatrix[fromOrigPos + cell.OriginIndex, fromDestPos + cell.DestinationIndex] = new MatrixGPSPoints.DistDurValue(cell.TravelDistance, cell.TravelDuration);

                _matrixGPSPoints.DistDurTextMatrix[fromOrigPos + cell.OriginIndex, fromDestPos + cell.DestinationIndex] = new MatrixGPSPoints.DistDurText(cell.TravelDistance.ToString(), TimeSpan.FromMinutes(cell.TravelDuration).ToString(@"hh\:mm"));

            }

        }

        /// <summary>
        /// Bing: Estimates the quota consumption
        /// </summary>
        public double QuotaConsumptionEstimation()
        {
            //-0.0003357142857

            var couta = _matrixGPSPoints.QuantityOfOriginsAndDestinations * _matrixGPSPoints.QuantityOfOriginsAndDestinations;

            return couta;
        }

        /// <summary>
        /// Bing: Estimate the price considering a monthly consumption between 250K and 500K transactions and an internal website implementation. Windows Development App offers Less than 50,000 cumulative billable transactions with any 24-hour period
        /// </summary>
        public double PriceForTransactionEstimation()
        {
            //https://www.microsoft.com/en-us/maps/licensing/options
            //http://www.noussintelligence.com/using-the-bing-maps-distance-matrix-api-for-solving-the-travelling-salesman-problem-the-origin-of-routing-problems/

            var price = QuotaConsumptionEstimation() * (2154.35 / 250000.0);

            return price;
        }


    }

}
