using fanuc_group_exchange_desktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Parser
{
    class GroupParser : AbstractParser
    {
        public override BasicInstance Parse(string groupString)
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
                CoordinateParser coordinateParser = new CoordinateParser();
                Coordinate coordinate = coordinateParser.Parse(coordinateString.Trim()) as Coordinate;
                coordinates.Add(coordinate);
            }


            return new RobotGroup(groupNumber, userFrame, userTool, configuration, coordinates);
        }



        //private RobotGroup ParseCartesianGroup(int groupNumber, string groupString)
        //{
        //    string groupFrames = groupString.Substring(groupString.IndexOf("UF"));
        //    groupFrames = groupFrames.Substring(0, groupFrames.IndexOf(","));
        //    Regex groupFramesRegex = new Regex("\\d");
        //    int UserFrame = int.Parse(groupFramesRegex.Match(groupFrames).Value);

        //    string groupTools = groupString.Substring(groupString.IndexOf("UT"));
        //    groupTools = groupTools.Substring(0, groupTools.IndexOf(","));
        //    Regex groupToolRegex = new Regex("\\d");
        //    int UserTool = int.Parse(groupToolRegex.Match(groupTools).Value);


        //}

        //private RobotNotFirstGroup ParseJointGroup(int groupNumber, string groupString)
        //{

        //    string groupFrames = groupString.Substring(groupString.IndexOf("UF"));
        //    groupFrames = groupFrames.Substring(0, groupFrames.IndexOf(","));
        //    Regex groupFramesRegex = new Regex("\\d");
        //    int UserFrame = int.Parse(groupFramesRegex.Match(groupFrames).Value);

        //    string groupTools = groupString.Substring(groupString.IndexOf("UT"));
        //    groupTools = groupTools.Substring(0, groupTools.IndexOf(","));
        //    Regex groupToolRegex = new Regex("\\d");
        //    int UserTool = int.Parse(groupToolRegex.Match(groupTools).Value);

        //    string coordinatesString = groupString.Substring(groupString.IndexOf("J"));

        //    List<string> coordinatesList = new List<string>(coordinatesString.Split(",	"));
        //    List<Coordinate> Coordinates = new List<Coordinate>();

        //    foreach (string coordinateString in coordinatesList)
        //    {
        //        CoordinateParser coordinateParser = new CoordinateParser();
        //        Coordinate coordinate = coordinateParser.Parse(coordinateString) as Coordinate;
        //        Coordinates.Add(coordinate);
        //    }

        //    return new RobotNotFirstGroup(groupNumber, UserFrame, UserTool, Coordinates);
        //}
    }
}
