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
        private MatrixGPSPoints _matrixGPSPoints;

        public GoogleDistanceMatrix(MatrixGPSPoints matrixGPSPoints)
        {
            GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyAx371kWMfkkUt3-lnMR_8JlNqC67mb33g"));

            this._matrixGPSPoints = matrixGPSPoints;
        }

        private List<Location> PopulateWayPoints(List<MatrixGPSPoints.GPSLatLng> gpsPoints)
        {
            List<Location> wayPoints = new List<Location>();

            foreach (var gpsPoint in gpsPoints)
            {
                wayPoints.Add(new LatLng(gpsPoint.Latitude, gpsPoint.Latitude));
            }

            return wayPoints;
        }

        /// <summary>
        /// Calculate distances and times to travel from one origin to many destinations
        /// </summary>
        public void DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses()
        {
            /*
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
                                                    }
            };
            */

            DistanceMatrixRequest request = new DistanceMatrixRequest
            {
                WaypointsOrigin = PopulateWayPoints(_matrixGPSPoints.OriginPoints),
                WaypointsDestination = PopulateWayPoints(_matrixGPSPoints.DestinationPoints),

                Mode = TravelMode.driving
            };

            var response = new DistanceMatrixService().GetResponse(request);

            PrintResponseMatrix(request, response);

            Console.ReadKey();
        }

        /// <summary>
        /// Calculate distances and times to travel from one origin to many destinations
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

            Console.ReadKey();
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

        private void PrintResponseMatrix(DistanceMatrixRequest request, DistanceMatrixResponse response)
        {
            int i_orig = 0, i_dest = 0;

            string[,] response_matrix = new string[request.WaypointsDestination.Count + 1, request.WaypointsOrigin.Count + 1];

            Console.WriteLine(response.Status);
            Console.WriteLine(response.ErrorMessage);
            Console.WriteLine("Rows returned: " + response.Rows.Length);

            foreach (var row in response.Rows)
            {
                //Print the origins
                response_matrix[i_dest, i_orig + 1] = request.WaypointsOrigin[i_orig].ToString();

                foreach (var element in row.Elements)
                {
                    //Print the destinies
                    response_matrix[i_dest + 1, 0] = request.WaypointsDestination[i_dest].ToString();

                    response_matrix[i_dest + 1, i_orig + 1] = "Dist: " + element.distance.Text + " / Dur: " + element.duration.Text;
                    ++i_dest;
                }
                ++i_orig;
                i_dest = 0;
                //Console.WriteLine("Distance: " + row.Elements.First().distance.Text + "\tDuration: " + row.Elements.First().duration.Text);
            }

            using (StreamWriter outfile = new StreamWriter(@"D:\GoogleDistanceMatrix_"+_matrixGPSPoints.QuantityOfOriginsAndDestinations + DateTime.Now.ToString("yyMMddHHmmss") + ".csv"))
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
    }
}
