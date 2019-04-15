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
            int N = 4;
            MatrixGPSPoints matrixGPSPoints = new MatrixGPSPoints(N);
            GoogleDistanceMatrix googleDM = new GoogleDistanceMatrix(matrixGPSPoints);

            //googleDM.DrivingDistancebyLngLatHasOneOriginAndDestinationAdresses();
            googleDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses();

        }


    }
}
