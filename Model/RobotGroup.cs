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

        public List<Coordinate> MainAxes { set; get; }
        public bool IsCartesian { set; get; }
        public List<Coordinate> ExtendedAxes { set; get; }

        public RobotGroup(int UserFrame, int UserTool)
        {
            this.UserFrame = UserFrame;
            this.UserTool = UserTool;
        }

        public RobotGroup(int Number, int UserFrame, int UserTool, string Config, List<Coordinate> axes)
        {
            this.Number = Number;
            this.UserFrame = UserFrame;
            this.UserTool = UserTool;
            this.Config = Config;
            this.MainAxes = new List<Coordinate>();
            this.ExtendedAxes = new List<Coordinate>();
            
            IsCartesian = axes[0].CoordinateName == "X";
            for (int i = 0; i < axes.Count; i++)
            {
                if (axes[i].CoordinateName == "E")
                {
                    ExtendedAxes.Add(axes[i]);
                }
                else
                {
                    MainAxes.Add(axes[i]);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder coordinatesString = new StringBuilder();

            for (int i = 0; i < MainAxes.Count; i++)
            {
                coordinatesString.Append($"{MainAxes[i]},");
                if ((i + 1) % 3 == 0 && (i + 1) != MainAxes.Count) coordinatesString.Append("\n");
            }
            if (ExtendedAxes.Count == 0)
            {
                coordinatesString.Remove(coordinatesString.Length - 1, 1);
            }
            if (MainAxes.Count != 0)
            {
                coordinatesString.Append("\r\n");
            }
            for (int i = 0; i < ExtendedAxes.Count; i++)
            {
                coordinatesString.Append($"{ExtendedAxes[i]},");
                if ((i + 1) % 3 == 0 && (i + 1) != ExtendedAxes.Count) coordinatesString.Append("\n");
            }
            coordinatesString.Remove(coordinatesString.Length - 1, 1);
            return $"\n   GP{Number}:\n\tUF : {_UserFrame}, UT : {_UserTool}, \t{Config}\n{coordinatesString}";
        }
    }
}
