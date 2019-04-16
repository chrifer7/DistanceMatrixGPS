﻿using System;
using System.Collections.Generic;
using System.Text;
using BingMapsRESTToolkit;

namespace DistanceMatrixTest.Model
{
    class BingDistanceMatrix
    {
        static private string _ApiKey = "AnxzBGPbW-0XE-v8FbLhGCbVPlch-7AyT9b_Fn9LnfXekr8hRL_2lblal9_FNEaK";//System.Configuration.ConfigurationManager.AppSettings.Get("BingMapsKey");

        static private void PrintTZResource(TimeZoneResponse tz)
        {

            Console.WriteLine($"Name: {tz.GenericName}");
            Console.WriteLine($"Windows ID: {tz.WindowsTimeZoneId}");
            Console.WriteLine($"IANA ID: {tz.IANATimeZoneId}");
            Console.WriteLine($"UTC offset: {tz.UtcOffset}");
            Console.WriteLine($"Abbrev: {tz.Abbreviation}");

            if (tz.ConvertedTime != null)
            {
                var ctz = tz.ConvertedTime;
                Console.WriteLine($"Local Time: {ctz.LocalTime}");
                Console.WriteLine($"TZ Abbr: {ctz.TimeZoneDisplayAbbr} ");
                Console.WriteLine($"TZ Name: {ctz.TimeZoneDisplayName}");
                Console.WriteLine($"UTC offset: {ctz.UtcOffsetWithDst }");
            }

            if (tz.DstRule != null)
            {
                var dst = tz.DstRule;
                Console.WriteLine("Start: {0} - {1} - {2} - {3}", dst.DstStartTime, dst.DstStartMonth, dst.DstStartDateRule, dst.DstAdjust1);
                Console.WriteLine("End: {0} - {1} - {2} - {3}", dst.DstEndTime, dst.DstEndMonth, dst.DstEndDateRule, dst.DstAdjust2);
            }
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

        /*
         * 
         *  Pending review.
         * 
        static public void AutoSuggestTest()
        {
            Console.WriteLine("Running Autosuggest Test");
            CoordWithRadius ul = new CoordWithRadius() { Latitude = 47.668697, Longitude = -122.376373, Radius = 5 };
        
            var request = new AutosuggestRequest()
            {
                BingMapsKey = _ApiKey,
                Query = "El Bur",
                UserLoc = ul
            };
            Console.WriteLine(request.GetRequestUrl());
            var resources = GetResourcesFromRequest(request);
            var entities = (resources[0] as AutosuggestResource);
            foreach (var entity in entities.Value)
                Console.Write($"Entity of type {entity.Type} returned.");
            Console.ReadLine();
        }
        */

        /// <summary>
        ///  Convert Time Zone Test
        ///  https://msdn.microsoft.com/en-us/library/mt829733.aspx
        ///  
        ///  NOTE: The `ConvertTimeZoneRequest` requires a Datetime and a TimeZone ID
        /// </summary>
        static public void ConvertTimeZoneTest()
        {
            Console.WriteLine("Running Convert TZ Test");
            var dt = DateTimeHelper.GetDateTimeFromUTCString("2018-05-15T13:14:15Z");
            var request = new ConvertTimeZoneRequest(dt, "Cape Verde Standard Time") { BingMapsKey = _ApiKey };
            Resource[] resources = GetResourcesFromRequest(request);
            var tz = (resources[0] as RESTTimeZone);
            PrintTZResource(tz.TimeZone);
            Console.ReadLine();

        }


        /// <summary>
        /// List Time Zone Test
        /// 
        /// https://msdn.microsoft.com/en-us/library/mt829734.aspx
        /// </summary>
        static public void ListTimeZoneTest()
        {
            Console.WriteLine("Running List TZ Test");
            var list_request = new ListTimeZonesRequest(true)
            {
                BingMapsKey = _ApiKey,
                TimeZoneStandard = "Windows"
            };
            Console.WriteLine(list_request.GetRequestUrl());

            var resources = GetResourcesFromRequest(list_request);
            Console.WriteLine("Printing first three TZ resources:\n");
            for (int i = 0; i < 3; i++)
                PrintTZResource((resources[i] as RESTTimeZone).TimeZone);

            Console.WriteLine("Running Get TZ By ID Test");
            var get_tz_request = new ListTimeZonesRequest(false)
            {
                IncludeDstRules = true,
                BingMapsKey = _ApiKey,
                DestinationTZID = "Cape Verde Standard Time"
            };
            Console.WriteLine(get_tz_request.GetRequestUrl());

            var tz_resources = GetResourcesFromRequest(get_tz_request);
            var tz = (tz_resources[0] as RESTTimeZone);
            PrintTZResource(tz.TimeZone);

            Console.ReadLine();

        }

        /// <summary>
        /// Find Time Zone test
        /// 
        ///  https://msdn.microsoft.com/en-us/library/mt829732.aspx
        /// </summary>
        static public void FindTimeZoneTest()
        {
            Console.WriteLine("Running Find Time Zone Test: By Query");
            var dt = DateTime.Now;
            var query_tz_request = new FindTimeZoneRequest("Seattle, USA", dt) { BingMapsKey = _ApiKey };
            var query_resources = GetResourcesFromRequest(query_tz_request);
            Console.WriteLine(query_tz_request.GetRequestUrl());

            var r_query = (query_resources[0] as RESTTimeZone);

            if (r_query.TimeZoneAtLocation.Length > 0)
            {
                var qtz = (r_query.TimeZoneAtLocation[0] as TimeZoneAtLocationResource);
                Console.WriteLine($"Place Name: {qtz.PlaceName}");
                PrintTZResource(qtz.TimeZone[0] as TimeZoneResponse);
            }
            else
            {
                Console.WriteLine("No Time Zone Query response.");
            }


            Console.WriteLine("\nRunning Find Time Zone Test: By Point");
            Coordinate cpoint = new Coordinate(47.668915, -122.375789);
            var point_tz_request = new FindTimeZoneRequest(cpoint) { BingMapsKey = _ApiKey, IncludeDstRules = true };

            var point_resources = GetResourcesFromRequest(point_tz_request);
            var r_point = (point_resources[0] as RESTTimeZone);
            var tz = (r_point.TimeZone as TimeZoneResponse);

            Console.WriteLine($"Time Zone: {r_point.TimeZone}");
            PrintTZResource(tz);
            Console.ReadLine();
        }

        /// <summary>
        ///  Location Recognition Test
        ///  
        /// https://msdn.microsoft.com/en-US/library/mt847173.aspx
        /// </summary>
        static public void LocationRecogTest()
        {
            Console.WriteLine("Running Location Recognition Test");

            Coordinate cpoint = new Coordinate(47.668915, -122.375789);

            Console.WriteLine("coord: {0}", cpoint.ToString());

            var request = new LocationRecogRequest() { BingMapsKey = _ApiKey, CenterPoint = cpoint };

            var resources = GetResourcesFromRequest(request);

            var r = (resources[0] as LocationRecog);

            if (r.AddressOfLocation.Length > 0)
                Console.WriteLine($"Address:\n{r.AddressOfLocation.ToString()}");

            if (r.BusinessAtLocation != null)
            {
                foreach (LocalBusiness business in r.BusinessAtLocation)
                {
                    Console.WriteLine($"Business:\n{business.BusinessInfo.EntityName}");
                }
            }

            if (r.NaturalPOIAtLocation != null)
            {
                foreach (NaturalPOIAtLocationEntity poi in r.NaturalPOIAtLocation)
                {
                    Console.WriteLine($"POI:\n{poi.EntityName}");
                }
            }

            Console.ReadLine();
        }

        /// <summary>
        ///  Geocode Test
        ///  
        ///  
        /// </summary>
        static public void GeoCodeTest()
        {
            Console.WriteLine("Running Geocode Test");
            var request = new GeocodeRequest()
            {
                BingMapsKey = _ApiKey,
                Query = "Seattle"
            };

            var resources = GetResourcesFromRequest(request);

            foreach (var resource in resources)
            {
                Console.WriteLine((resource as Location).Name);
            }

            Console.ReadLine();
        }


        /// <summary>
        ///  Location Recognition Test
        ///  
        /// https://docs.microsoft.com/en-us/bingmaps/rest-services/routes/calculate-a-distance-matrix
        /// </summary>
        static public void DistantMatrixTest()
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
    }

}
