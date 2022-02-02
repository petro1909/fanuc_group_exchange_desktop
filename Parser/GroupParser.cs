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
            string strGroupNumber = groupString.Substring(groupString.IndexOf("GP"), groupString.IndexOf(":") - groupString.IndexOf("GP"));
            int groupNumber = int.Parse(strGroupNumber.Substring(2));

            RobotGroup robotGroup;
            if(groupNumber == 1)
            {
                robotGroup = ParseFirstGroup(groupString);
            } else
            {
                robotGroup = ParseNotFirstGroup(groupNumber, groupString);
            }
            return robotGroup;
        }

        private RobotFirstGroup ParseFirstGroup(string groupString)
        {
            return new RobotFirstGroup(1, groupString);
        }

        private RobotNotFirstGroup ParseNotFirstGroup(int groupNumber, string groupString)
        {

            string groupFrames = groupString.Substring(groupString.IndexOf("UF"));
            groupFrames = groupFrames.Substring(0, groupFrames.IndexOf(","));
            Regex groupFramesRegex = new Regex("\\d");
            int UserFrame = int.Parse(groupFramesRegex.Match(groupFrames).Value);

            string groupTools = groupString.Substring(groupString.IndexOf("UT"));
            groupTools = groupTools.Substring(0, groupTools.IndexOf(","));
            Regex groupToolRegex = new Regex("\\d");
            int UserTool = int.Parse(groupToolRegex.Match(groupTools).Value);

            string coordinatesString = groupString.Substring(groupString.IndexOf("J"));

            List<string> coordinatesList = new List<string>(coordinatesString.Split(",	"));
            List<Coordinate> Coordinates = new List<Coordinate>();

            foreach (string coordinateString in coordinatesList)
            {
                CoordinateParser coordinateParser = new CoordinateParser();
                Coordinate coordinate = coordinateParser.Parse(coordinateString) as Coordinate;
                Coordinates.Add(coordinate);
            }

            return new RobotNotFirstGroup(groupNumber, UserFrame, UserTool, Coordinates);
        }
    }
}
