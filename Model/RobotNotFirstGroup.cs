using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    class RobotNotFirstGroup : RobotGroup
    {
        private int _UserFrame;
        private int _UserTool;
        private List<Coordinate> _Coordinates;

        public int UserFrame
        {
            set
            {
                if (value < 0)
                {
                    Console.WriteLine("User Frame can't be less a zero");
                }
                else
                {
                    _UserFrame = value;
                }
            }
            get { return _UserFrame; }
        }

        public int UserTool
        {
            set
            {
                if (value < 0)
                {
                    Console.WriteLine("User Tool can't be less a zero");
                }
                else
                {
                    _UserTool = value;
                }
            }
            get { return _UserTool; }
        }

        public List<Coordinate> Coordinates
        {
            set
            {
                _Coordinates = value;
            }
            get
            {
                return _Coordinates;
            }
        }
        public RobotNotFirstGroup() { }

        public RobotNotFirstGroup(int Number)
        {
            this.Number = Number;
        }

        public RobotNotFirstGroup(int Number, int UserFrame, int UserTool, List<Coordinate> Coordinates)
        {
            this.Number = Number;
            this.UserFrame = UserFrame;
            this.UserTool = UserTool;
            this.Coordinates = Coordinates;
        }

        public override void Parse(string groupString)
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
                Coordinate coordinate = new Coordinate();
                coordinate.Parse(coordinateString);
                Coordinates.Add(coordinate);
            }
            this.UserFrame = UserFrame;
            this.UserTool = UserTool;
            this.Coordinates = Coordinates;
        }

        public override string ToString()
        {
            string coordinateString = "";

            for (int i = 0; i < _Coordinates.Count; i++)
            {
                coordinateString += Coordinates[i].ToString();
                if (_Coordinates[i].CoordinateNumber != Coordinates.Count)
                    coordinateString += ",";
            }
            return "\n" +
            "   GP" + this._Number + ":\n" +
            "\tUF : " + _UserFrame + ", UT : " + _UserTool + ",\n" + coordinateString;
        }
    }
}
