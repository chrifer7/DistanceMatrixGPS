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
            MatrixGPSPoints matrixGPSPoints = new MatrixGPSPoints(5);
            GoogleDistanceMatrix googleDM = new GoogleDistanceMatrix(matrixGPSPoints);

            //googleDM.DrivingDistancebyLngLatHasOneOriginAndDestinationAdresses();
            googleDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdresses();

        }


    }
}
