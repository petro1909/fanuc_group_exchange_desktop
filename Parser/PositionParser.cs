using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;

namespace fanuc_group_exchange_desktop.Parser
{
    class PositionParser
    {
        public RobotPosition ParsePosition(string positionString)
        {
            positionString = positionString.Trim();

            //get position number
            string positionNumberString = positionString.Substring(0, positionString.IndexOf("GP"));
            Regex positionNumberRegex = new Regex("\\d+");
            int positionNumber = int.Parse(positionNumberRegex.Match(positionNumberString).Value);

            //get position comment
            string positionComment = "";
            int index = positionNumberString.IndexOf('"');
            if (index != -1)
            {
                string StartOfComment = positionNumberString.Substring(index);
                positionComment = ":" + StartOfComment.Substring(0, StartOfComment.LastIndexOf(']'));
            }

            //get string with robot groups and parse to robot groups list
            SortedDictionary<int, RobotGroup> groupsDictionary = new SortedDictionary<int, RobotGroup>();

            string groups = positionString.Substring(positionString.IndexOf("GP"));
            groups = groups.Substring(0, groups.IndexOf("}"));

            List<string> strGroups = new(groups.Split("\n   "));


            for (int i = 0; i < strGroups.Count; i++) { 

                RobotGroup robotGroup = ParseGroup(strGroups[i]);
                groupsDictionary.Add(robotGroup.Number, robotGroup);
            }
            return new RobotPosition(positionNumber, positionComment, groupsDictionary);
        }

        private RobotGroup ParseGroup(string groupString)
        {
            Regex digit = new Regex("\\d+");
            int groupNumber = int.Parse(digit.Match(groupString).Value);

            string groupFrames = groupString[groupString.IndexOf("UF")..groupString.IndexOf(",")];
            int userFrame = int.Parse(digit.Match(groupFrames).Value);

            string groupTools = groupString[groupString.IndexOf("UT")..groupString.IndexOf(',', groupString.IndexOf("UT"))];
            int userTool = int.Parse(digit.Match(groupTools).Value);

            string configuration = "";
            int configIndex = groupString.IndexOf("CONFIG");
            if (configIndex != -1)
            {
                int lineIndex = groupString.IndexOf('\n', configIndex);
                configuration = groupString[configIndex..lineIndex];
            }

            string positionString = configIndex != -1 ? groupString[groupString.IndexOf("X")..] : groupString[groupString.IndexOf("J")..];

            List<string> coordinatesList = new List<string>(positionString.Split(','));

            List<Coordinate> coordinates = new List<Coordinate>();

            foreach (string coordinateString in coordinatesList)
            {
                Coordinate coordinate = ParseCoordinate(coordinateString.Trim());
                coordinates.Add(coordinate);
            }

            return new RobotGroup(groupNumber, userFrame, userTool, configuration, coordinates);
        }

        private Coordinate ParseCoordinate(string coordinateString)
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
