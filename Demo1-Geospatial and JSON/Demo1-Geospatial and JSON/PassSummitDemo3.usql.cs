using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Interfaces.Streaming;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Demo3_Geospatial_and_JSON
{
    public class Functions
    {
        public static string ToLatLon(int Zone, string polygon)
        {
            string[] coordinates = polygon.Split(',');

            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = Zone;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;

            string latlon = "";

            foreach (string i in coordinates)
            {
                string[] point = i.Split(' ');
                double utmX = double.Parse(point[0]);
                double utmY = double.Parse(point[1]);

                var x = utmX - 500000;
                var y = isNorthHemisphere ? utmY : utmY - 10000000;

                var s = ((zone * 6.0) - 183.0);
                var lat = y / (c_sa * 0.9996);
                var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
                var a = x / v;
                var a1 = Math.Sin(2 * lat);
                var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
                var j2 = lat + (a1 / 2.0);
                var j4 = ((3 * j2) + a2) / 4.0;
                var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
                var alfa = (3.0 / 4.0) * e2cuadrada;
                var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
                var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
                var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
                var b = (y - bm) / v;
                var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
                var eps = a * (1 - (epsi / 3.0));
                var nab = (b * (1 - epsi)) + lat;
                var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
                var delt = Math.Atan(senoheps / (Math.Cos(nab)));
                var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

                double longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
                double latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;

                latlon = latlon + longitude.ToString() + " " + latitude.ToString() + ",";
            }

            latlon = latlon.Substring(0, latlon.Length - 1);

            return latlon;
        }
    }
}
