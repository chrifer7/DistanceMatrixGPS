using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BingMapsRESTToolkit;

namespace DistanceMatrixTest.Model
{
    class BingDistanceMatrix
    {
        static private string _ApiKey = "AnxzBGPbW-0XE-v8FbLhGCbVPlch-7AyT9b_Fn9LnfXekr8hRL_2lblal9_FNEaK";//System.Configuration.ConfigurationManager.AppSettings.Get("BingMapsKey");

        private MatrixGPSPoints _matrixGPSPoints;

        public BingDistanceMatrix(MatrixGPSPoints matrixGPSPoints)
        {
            this._matrixGPSPoints = matrixGPSPoints;
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
        /// Bing: Calculates the distances and times to travel from many origins to many destinations using many requests to avoid the limitation of 10 x 10 
        /// </summary>
        public void DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted()
        {
            int splits = (_matrixGPSPoints.QuantityOfOriginsAndDestinations + 9) / 10; //Equivalent to Math.Ceiling
            Console.WriteLine("Number of splits {0}", splits);

            for (int i = 0; i < splits; i++)
            {

                int fromOriginPoint = i * 10;
                int toOriginPoint = fromOriginPoint + 10;//(_matrixGPSPoints.QuantityOfOriginsAndDestinations < fromOriginPoint + 10 ? _matrixGPSPoints.QuantityOfOriginsAndDestinations : fromOriginPoint + 10);

                Console.WriteLine("Origin From: {0} To: {1}", fromOriginPoint, toOriginPoint);

                for (int j = 0; j < splits; j++)
                {
                    Console.WriteLine("Request {0} - {1}", i, j);

                    int fromDestinationPoint = j * 10;
                    int toDestinationPoint = fromDestinationPoint + 10;

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

            using (StreamWriter outfile = new StreamWriter(@"D:\output\BingDistanceMatrix_N-" + _matrixGPSPoints.QuantityOfOriginsAndDestinations + "_" + DateTime.Now.ToString("yyMMddHHmmss.fff") + ".csv"))
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


    }

}
