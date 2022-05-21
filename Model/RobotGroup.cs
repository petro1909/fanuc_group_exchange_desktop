using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class RobotGroup : BasicInstance
    {
        private int _UserFrame;
        private int _UserTool;

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

        public string Config { set; get; }

        public List<Coordinate> Coordinates { set; get; }

        public RobotGroup(int UserFrame, int UserTool)
        {
            this.UserFrame = UserFrame;
            this.UserTool = UserTool;
        }

        public RobotGroup(int Number, int UserFrame, int UserTool, string Config,  List<Coordinate> Coordinates)
        {
            this.Number = Number;
            this.UserFrame = UserFrame;
            this.UserTool = UserTool;
            this.Config = Config;
            this.Coordinates = Coordinates;
        }

        public override string ToString()
        {
            StringBuilder coordinatesString = new StringBuilder();

            for (int i = 0; i < Coordinates.Count; i++)
            {
                if (i > 0 && i % 3 != 0 && Coordinates[i].CoordinateName == "E1=")
                {
                    coordinatesString.Append("\n");
                }
                coordinatesString.Append(Coordinates[i]);

                if ((i + 1) != Coordinates.Count)
                {
                    coordinatesString.Append(",");
                    if ((i + 1) % 3 == 0) coordinatesString.Append("\n");
                }
            }

            return $"\n   GP{Number}:\n\tUF : {_UserFrame}, UT : {_UserTool}, \t{Config}\n{coordinatesString}";
        }
    }
}
