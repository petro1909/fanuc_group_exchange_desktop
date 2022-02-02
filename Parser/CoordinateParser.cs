using fanuc_group_exchange_desktop.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Parser
{
    class CoordinateParser : AbstractParser
    {
        public override BasicInstance Parse(string coordinateString)
        {
            Regex counterRegex = new Regex("J\\d+");
            string coordinateCounterString = counterRegex.Match(coordinateString).Value;
            int CoordinateNumber = int.Parse(coordinateCounterString.Substring(1));

            Regex coordinateNumberRegex = new Regex("\\-*\\d*\\.{1}\\d+");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            double CoordinatePosition = double.Parse(coordinateNumberRegex.Match(coordinateString).Value, formatter);

            Regex unitRegex = new Regex("mm|deg");
            string CoordinateUnit = unitRegex.Match(coordinateString).Value;

            return new Coordinate(CoordinateNumber, CoordinatePosition, CoordinateUnit);
        }
    }
}
