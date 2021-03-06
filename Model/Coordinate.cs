using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class Coordinate : BasicInstance
    {
        public string CoordinateName { set; get; }
        public double CoordinatePosition { set; get; }
        public string CoordinateUnit { set; get; }

        public Coordinate() { }

        public Coordinate(string CoordinateName, int number)
        {
            this.CoordinateName = CoordinateName;
            this.Number = number;
        }

        public Coordinate(string CoordinateName, int number, double CoordinatePosition, string CoordinateUnit) : this(CoordinateName, number)
        {
            this.CoordinatePosition = CoordinatePosition;
            this.CoordinateUnit = CoordinateUnit;
        }

        public override string ToString()
        {
            string numberStr = Number == 0 ? " " : Number.ToString(); 

            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            string coordinatePositionString = string.Format(formatter, "{0:0.000}", CoordinatePosition);

            int whitespacesCount = 10 - coordinatePositionString.Length;
            string whitespaceString = new(' ', whitespacesCount);


            string coordinateUnitString;
            if (CoordinateUnit.Equals("mm")) coordinateUnitString = $" {CoordinateUnit}";
            else coordinateUnitString = $"{CoordinateUnit}";

            return $"\t{CoordinateName}{numberStr}={whitespaceString}{coordinatePositionString} {coordinateUnitString}";
        }
    }
}