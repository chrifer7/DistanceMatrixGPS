using System;
using System.Collections.Generic;
using System.Linq;
using DistanceMatrixTest.Model;


namespace DistanceMatrixTest
{   
    class Program
    {
        static void Main(string[] args)
        {
            //Create a Matrix of N x N points and calculate all the matrix distances and times
            //By now only acepts N = 10x
            int N = 10;
            MatrixGPSPoints matrixGPSPoints = new MatrixGPSPoints(N);

            Console.WriteLine("/******************************************************/");
            Console.WriteLine("/****************** Google Maps API *******************/");
            Console.WriteLine("/******************************************************/");

            GoogleDistanceMatrix googleDM = new GoogleDistanceMatrix(matrixGPSPoints);

            //googleDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses();
            var watchGoogle = System.Diagnostics.Stopwatch.StartNew();
            googleDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted();
            watchGoogle.Stop();
            var elapsedMsGoogle = watchGoogle.ElapsedMilliseconds;

            Console.WriteLine("\n\n\n\n");

            Console.WriteLine("/******************************************************/");
            Console.WriteLine("/****************** Bing Maps API *******************/");
            Console.WriteLine("/******************************************************/");

            BingDistanceMatrix bingDM = new BingDistanceMatrix(matrixGPSPoints);

            //bingDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses();
            var watchBing = System.Diagnostics.Stopwatch.StartNew();
            bingDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted();
            watchBing.Stop();
            var elapsedMsBing = watchBing.ElapsedMilliseconds;

            Console.WriteLine("\n\nGoogle Execution Time: {0}", elapsedMsGoogle);
            Console.WriteLine("Google estimated cuota of elements consumed: {0}", googleDM.QuotaConsumptionEstimation());
            Console.WriteLine("Google estimated price for the execution: ${0} \n" +
                "(considering a monthly consumption between 100K and 500K elements)", googleDM.PriceForTransactionEstimation());

            Console.WriteLine("\n\nBing Execution Time: {0}", elapsedMsBing);
            Console.WriteLine("Bing estimated cuota of elements consumed: {0}", bingDM.QuotaConsumptionEstimation());
            Console.WriteLine("Bing estimated price for the execution: ${0} \n" +
                "(considering a monthly consumption between 250K and 500K transactions and an internal website implementation)", bingDM.PriceForTransactionEstimation());

            Console.WriteLine("\n\n\nThe process has finished.\n\nPress any key...");
            Console.ReadKey();
        }


    }
}
