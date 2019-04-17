using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using static Google.Maps.DistanceMatrix.DistanceMatrixResponse;

namespace DistanceMatrixTest.Model
{
    class GoogleDistanceMatrix
    {
        static private string _ApiKey = "AIzaSyAx371kWMfkkUt3-lnMR_8JlNqC67mb33g";
        private MatrixGPSPoints _matrixGPSPoints;

        public GoogleDistanceMatrix(MatrixGPSPoints matrixGPSPoints)
        {
            GoogleSigned.AssignAllServices(new GoogleSigned(_ApiKey));

            this._matrixGPSPoints = matrixGPSPoints;
        }

        private List<Location> PopulateWayPoints(List<MatrixGPSPoints.GPSLatLng> gpsPoints)
        {
            List<Location> wayPoints = new List<Location>();

            foreach (var gpsPoint in gpsPoints)
            {
                wayPoints.Add(new LatLng(gpsPoint.Latitude, gpsPoint.Longitude));
            }

            return wayPoints;
        }

        private List<Location> PopulateSegmentWayPoints(List<MatrixGPSPoints.GPSLatLng> gpsPoints, int fromPoint, int toPoint)
        {
            List<Location> wayPoints = new List<Location>();

            if (fromPoint < 0 || toPoint > gpsPoints.Count) return null;

            for (int i = fromPoint; i < toPoint; i++)
            {
                wayPoints.Add(new LatLng(gpsPoints[i].Latitude, gpsPoints[i].Longitude));
            }

            Console.WriteLine("\nGPS Points to Google (Location)");
            gpsPoints.ForEach(item => Console.Write(item.Latitude + "," + item.Longitude + " / "));
            Console.WriteLine("\nWay Points from: {0} to: {1}", fromPoint, toPoint);
            wayPoints.ForEach(item => Console.Write(item + " / "));

            return wayPoints;
        }

        public void DistantMatrixTest()
        {
            DistanceMatrixRequest request = new DistanceMatrixRequest()
            {
                WaypointsOrigin = new List<Location> {
                                                        new LatLng(47.58162076M,7.18289976M),
                                                        new LatLng(46.63146309M,4.23322802M),
                                                        new LatLng(47.60999809M,2.47722561M),
                                                        new LatLng(48.63594493M,11.13342617M),
                                                        new LatLng(45.00055829M,8.43115231M),
                                                        new LatLng(46.1774775M,4.34958805M),
                                                        new LatLng(50.22820419M,2.78969141M),
                                                        new LatLng(46.47207431M,11.16166495M),
                                                        new LatLng(48.40098444M,5.27661128M),
                                                        new LatLng(45.91952505M,4.23231255M)
                                                    },
                WaypointsDestination = new List<Location> {
                                                        new LatLng(47.58162076M,7.18289976M),
                                                        new LatLng(46.63146309M,4.23322802M),
                                                        new LatLng(47.60999809M,2.47722561M),
                                                        new LatLng(48.63594493M,11.13342617M),
                                                        new LatLng(45.00055829M,8.43115231M),
                                                        new LatLng(46.1774775M,4.34958805M),
                                                        new LatLng(50.22820419M,2.78969141M),
                                                        new LatLng(46.47207431M,11.16166495M),
                                                        new LatLng(48.40098444M,5.27661128M),
                                                        new LatLng(45.91952505M,4.23231255M)
                                                    },
                Mode = TravelMode.driving
            };

            var response = new DistanceMatrixService().GetResponse(request);

            SimplePrintResponseMatrix(request, response);

            
        }

        /// <summary>
        /// Google: Calculates the distances and times to travel from many origins to many destinations
        /// </summary>
        public void DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses()
        {
            DistanceMatrixRequest request = new DistanceMatrixRequest
            {
                WaypointsOrigin = PopulateWayPoints(_matrixGPSPoints.OriginPoints),
                WaypointsDestination = PopulateWayPoints(_matrixGPSPoints.DestinationPoints),

                Mode = TravelMode.driving
            };

            var response = new DistanceMatrixService().GetResponse(request);

            SimplePrintResponseMatrix(request, response);
            
        }

        /// <summary>
        /// Google: Calculates the distances and times to travel from many origins to many destinations using many requests to avoid the limitation of 10 x 10 
        /// </summary>
        public void DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted()
        {
            int splits = (_matrixGPSPoints.QuantityOfOriginsAndDestinations + 9)/ 10; //Equivalent to Math.Ceiling
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
                        WaypointsOrigin = PopulateSegmentWayPoints(_matrixGPSPoints.OriginPoints, fromOriginPoint, toOriginPoint),
                        WaypointsDestination = PopulateSegmentWayPoints(_matrixGPSPoints.DestinationPoints, fromDestinationPoint, toDestinationPoint),

                        Mode = TravelMode.driving
                    };

                    var response = new DistanceMatrixService().GetResponse(request);

                    AddToResponseMatrix(request, response, fromOriginPoint, fromDestinationPoint);
                }                    
            }

            _matrixGPSPoints.PrintDistDurTextMatrix("Google");

            
        }

        /// <summary>
        /// Google: Calculates the distances and times to travel from one origin to many destinations
        /// </summary>
        private void DrivingDistancebyLngLatHasOneOriginAndDestinationAdresses()
        {
            DistanceMatrixRequest request = new DistanceMatrixRequest()
            {
                WaypointsOrigin = new List<Location> { new LatLng(49.171020M, 5.969358M) },
                WaypointsDestination = new List<Location> { new LatLng(54.555393M, 23.362615M) }
            };

            var response = new DistanceMatrixService().GetResponse(request);

            //Console.WriteLine(response.Rows.First().Elements.First().distance.Text);
            //Console.WriteLine(response.Rows.First().Elements.First().duration.Text);
            PrintResponseList(request, response);

            
        }

        private void PrintResponseList(DistanceMatrixRequest request, DistanceMatrixResponse response)
        {
            int i_orig = 0, i_dest = 0;
            foreach (var row in response.Rows)
            {
                Console.WriteLine("===>>> Origin: " + request.WaypointsOrigin[i_orig]);
                foreach (var element in row.Elements)
                {
                    Console.WriteLine(">>> Destination: " + request.WaypointsDestination[i_dest]);
                    Console.WriteLine("Distance: " + element.distance.Text + "\tDuration: " + element.duration.Text);
                    ++i_dest;
                }
                ++i_orig;
                i_dest = 0;
                //Console.WriteLine("Distance: " + row.Elements.First().distance.Text + "\tDuration: " + row.Elements.First().duration.Text);
            }
        }

        private void SimplePrintResponseMatrix(DistanceMatrixRequest request, DistanceMatrixResponse response)
        {
            int i_orig = 0, i_dest = 0;

            string[,] response_matrix = new string[request.WaypointsDestination.Count + 1, request.WaypointsOrigin.Count + 1];

            Console.WriteLine(response.Status);
            Console.WriteLine(response.ErrorMessage);
            Console.WriteLine("Rows returned: " + response.Rows.Length);

            foreach (var row in response.Rows)
            {
                //Print the origins
                response_matrix[i_orig + 1, 0] = request.WaypointsOrigin[i_orig].ToString();

                foreach (var element in row.Elements)
                {
                    //Print the destinies
                    response_matrix[0, i_dest + 1] = request.WaypointsDestination[i_dest].ToString();

                    if (element.distance  == null) continue;

                    response_matrix[i_orig + 1, i_dest + 1] = "Dist: " + element.distance.Text + " / Dur: " + element.duration.Text;
                    ++i_dest;
                }
                ++i_orig;
                i_dest = 0;
                //Console.WriteLine("Distance: " + row.Elements.First().distance.Text + "\tDuration: " + row.Elements.First().duration.Text);
            }

            using (StreamWriter outfile = new StreamWriter(@"D:\output\GoogleDistanceMatrix_N-"+_matrixGPSPoints.QuantityOfOriginsAndDestinations + "_" + DateTime.Now.ToString("yyMMddHHmmss.fff") + ".csv"))
            {
                for (int row = 0; row < request.WaypointsDestination.Count + 1; row++)
                {
                    string content = "";
                    for (int col = 0; col < request.WaypointsOrigin.Count + 1; col++)
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

        private void AddToResponseMatrix(DistanceMatrixRequest request, DistanceMatrixResponse response, int fromOrigPos = 0, int fromDestPos = 0)
        {
            //Maybe I don't need the size of the matrix, It comes defined in the response temporal avoid this paramenters(int toOrigPos = 10, , int toDestPos = 10)
            int i_orig = fromOrigPos, i_dest = fromDestPos;

            //string[,] response_matrix = new string[request.WaypointsDestination.Count + 1, request.WaypointsOrigin.Count + 1];

            Console.WriteLine(response.Status);
            Console.WriteLine(response.ErrorMessage);
            Console.WriteLine("Rows returned: " + response.Rows.Length);

            foreach (var row in response.Rows)
            {

                foreach (var element in row.Elements)
                {
                    if (element.distance == null) continue;

                    //Console.WriteLine("DistDurValueMatrix==> i_dest:{0}, i_orig:{1}", i_orig, i_dest);

                    _matrixGPSPoints.DistDurValueMatrix[i_orig, i_dest] = new MatrixGPSPoints.DistDurValue(element.distance.Value, element.duration.Value);

                    _matrixGPSPoints.DistDurTextMatrix[i_orig, i_dest] = new MatrixGPSPoints.DistDurText(element.distance.Text, element.duration.Text);

                    ++i_dest;
                }
                ++i_orig;
                i_dest = fromDestPos;
            }
        }
    }
}
