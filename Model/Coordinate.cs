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
        private double _CoordinatePosition;
        private string _CoordinateUnit;


        public double CoordinatePosition
        {
            set { _CoordinatePosition = value; }
            get { return _CoordinatePosition; }
        }

        public string CoordinateUnit
        {
            set { _CoordinateUnit = value; }
            get { return _CoordinateUnit; }
        }

        public Coordinate() { }

        public Coordinate(int Number)
        {
            this.Number = Number; 
        }

        public Coordinate(int CoordinateNumber, double CoordinatePosition, string CoordinateUnit)
        {
            this.Number = CoordinateNumber;
            this.CoordinatePosition = CoordinatePosition;
            this.CoordinateUnit = CoordinateUnit;
        }

        public override string ToString()
        {
            string coordinateNumberString = "J" + Number.ToString();

            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            string coordinatePositionString1 = String.Format(formatter, "{0:0.000}", _CoordinatePosition);
            string coordinatePositionString = "          " + coordinatePositionString1;
            coordinatePositionString = coordinatePositionString.Substring(coordinatePositionString1.Length);

            string coordinateUnitString = "    " + _CoordinateUnit.ToString();
            coordinateUnitString = coordinateUnitString.Substring(_CoordinateUnit.Length);

            return "\t" + coordinateNumberString + "=" + coordinatePositionString + coordinateUnitString;
        }
    }
}