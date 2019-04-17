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
            int N = 20;
            MatrixGPSPoints matrixGPSPoints = new MatrixGPSPoints(N);
            GoogleDistanceMatrix googleDM = new GoogleDistanceMatrix(matrixGPSPoints);

            //googleDM.DrivingDistancebyLngLatHasOneOriginAndDestinationAdresses();
            //googleDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses();
            //googleDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted();


            BingDistanceMatrix bingDM = new BingDistanceMatrix(matrixGPSPoints);
            //bingDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses();
            bingDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted();

            Console.WriteLine("\n\n\nThe process has finished.\n\nPress any key...");
            Console.ReadKey();
        }


    }
}
