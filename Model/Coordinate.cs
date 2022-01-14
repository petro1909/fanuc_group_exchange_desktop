using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class Coordinate
    {
        private int _CoordinateNumber;
        private double _CoordinatePosition;
        private string _CoordinateUnit;


        public int CoordinateNumber
        {
            set { 
                if(value<=0)
                {
                    Console.WriteLine("Number of Coordinate can't be less or be a zero");
                } else
                {
                    _CoordinateNumber = value;
                }    
            }
            get { return _CoordinateNumber; }
        }

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

        public Coordinate(int CoordinateNumber, double CoordinatePosition, string CoordinateUnit)
        {
            this.CoordinateNumber = CoordinateNumber;
            this.CoordinatePosition = CoordinatePosition;
            this.CoordinateUnit = CoordinateUnit;
        }


        public void ParseCoordinateInGroupFromString(string coordinateString)
        {
            Regex counterRegex = new Regex("J\\d+");
            string coordinateCounterString = counterRegex.Match(coordinateString).Value;
            int CoordinateNumber = int.Parse(coordinateCounterString.Substring(1));

            Regex coordinateNumberRegex = new Regex("\\-*\\d*\\.{1}\\d+");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            double CoordinatePosition = double.Parse(coordinateNumberRegex.Match(coordinateString).Value, formatter);

            Regex unitRegex = new Regex("mm|deg");
            string CoordinateUnit = unitRegex.Match(coordinateString).Value;

            this.CoordinateNumber = CoordinateNumber;
            this.CoordinatePosition = CoordinatePosition;
            this.CoordinateUnit = CoordinateUnit;
        }

        public override string ToString()
        {
            string coordinateNumberString = "J" + _CoordinateNumber.ToString();

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