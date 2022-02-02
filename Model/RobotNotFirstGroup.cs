using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class RobotNotFirstGroup : RobotGroup
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
        public RobotNotFirstGroup() {
            Coordinates = new List<Coordinate>();
        }

        public RobotNotFirstGroup(int Number)
        {
            this.Number = Number;
        }

        public RobotNotFirstGroup(int UserFrame, int UserTool)
        {
            this.UserFrame = UserFrame;
            this.UserTool = UserTool;
        }

        public RobotNotFirstGroup(int Number, int UserFrame, int UserTool, List<Coordinate> Coordinates)
        {
            this.Number = Number;
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
                if (_Coordinates[i].Number != Coordinates.Count)
                    coordinateString += ",";
            }
            return "\n" +
            "   GP" + this.Number + ":\n" +
            "\tUF : " + _UserFrame + ", UT : " + _UserTool + ",\n" + coordinateString;
        }
    }
}
