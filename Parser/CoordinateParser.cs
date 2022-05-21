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
            int equalIndex = coordinateString.IndexOf("=") + 1;
            string coordinateNameWithEqual = coordinateString[0..equalIndex];

            Regex coordinateNumberRegex = new Regex("\\-*\\d*\\.{1}\\d+");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            double CoordinatePosition = double.Parse(coordinateNumberRegex.Match(coordinateString).Value, formatter);

            Regex unitRegex = new Regex("mm|deg");
            string CoordinateUnit = unitRegex.Match(coordinateString).Value;

            return new Coordinate(coordinateNameWithEqual, CoordinatePosition, CoordinateUnit);
        }
    }
}
