using System;
using System.Collections.Generic;
using System.Linq;
using DistanceMatrixTest.Model;

using System.Configuration;


namespace DistanceMatrixTest
{   
    class Program
    {
        public static bool CalculatesGoogle = false;
        public static bool CalculatesBing = true;
        public static int N = 10;

        static void Main(string[] args)
        {
            {//Temporaly write the keys here
                //var keys = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
                //foreach (var k in keys) Console.WriteLine("key: {0} \t value: {1}", k, ConfigurationManager.AppSettings[k]);
                //AddUpdateAppSettings("BingAPIKey", "AnxzBGPbW-0XE-v8FbLhGCbVPlch-7AyT9b_Fn9LnfXekr8hRL_2lblal9_FNEaK");
                //AddUpdateAppSettings("GoogleAPIKey", "AIzaSyAx371kWMfkkUt3-lnMR_8JlNqC67mb33g");
            }
            System.Diagnostics.Stopwatch watchGoogle;
            long elapsedMsGoogle = 0;
            System.Diagnostics.Stopwatch watchBing;
            long elapsedMsBing = 0;

            //Create a Matrix of N x N points and calculate all the matrix distances and times
            //By now only acepts N = 10x
            //N = 10;
            MatrixGPSPoints matrixGPSPoints = new MatrixGPSPoints(N);
            
            GoogleDistanceMatrix googleDM = new GoogleDistanceMatrix(matrixGPSPoints);
            BingDistanceMatrix bingDM = new BingDistanceMatrix(matrixGPSPoints);
            
            if (CalculatesGoogle)
            {
                { 
                    Console.WriteLine("\n\n\n\n");

                    Console.WriteLine("******************************************************");
                    Console.WriteLine("****************** Google Maps API *******************");
                    Console.WriteLine("******************************************************");
                }
                watchGoogle = System.Diagnostics.Stopwatch.StartNew();
                googleDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted();
                watchGoogle.Stop();
                elapsedMsGoogle = watchGoogle.ElapsedMilliseconds;
            }
            
            if (CalculatesBing)
            {
                { 
                Console.WriteLine("\n\n\n\n");
                
                Console.WriteLine("******************************************************");
                Console.WriteLine("******************* Bing Maps API ********************");
                Console.WriteLine("******************************************************");
                }
                watchBing = System.Diagnostics.Stopwatch.StartNew();
                bingDM.DrivingDistancebyLngLatHasManyOriginsAndManyDestinationsAdressesSplitted();
                watchBing.Stop();
                elapsedMsBing = watchBing.ElapsedMilliseconds;
            }
                        
            Console.WriteLine("********************** SUMARY ************************");
            Console.WriteLine("Total OriginPoints: {0}", N);
            Console.WriteLine("Total DestinationPoints: {0}", N);


            if (CalculatesGoogle)
            {
                Console.WriteLine("\n\nGoogle Execution Time: {0}", elapsedMsGoogle);
                Console.WriteLine("Google estimated quota of elements consumed: {0}", googleDM.QuotaConsumptionEstimation());
                Console.WriteLine("Google estimated price for the execution: ${0} \n" +
                    "(considering a monthly consumption between 100K and 500K elements)", googleDM.PriceForTransactionEstimation());
            }

            if (CalculatesBing)
            {
                Console.WriteLine("\n\nBing Execution Time: {0}", elapsedMsBing);
                Console.WriteLine("Bing theoretical estimated quota of requests consumed: {0}", bingDM.QuotaConsumptionEstimation());
                Console.WriteLine("Bing estimated price for the execution: ${0} \n" +
                    "(considering a monthly consumption between 250K and 500K transactions and an internal website implementation)", bingDM.PriceForTransactionEstimation());
            }

            Console.WriteLine("\n\n\nThe process has finished.\n\nPress any key...");
            Console.ReadKey();
        }
        
        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);                    
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }


}

