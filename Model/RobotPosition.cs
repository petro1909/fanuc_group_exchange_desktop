using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class RobotPosition
    {
        private int _PositionNumber;
        private SortedDictionary<int, RobotGroup> _RobotGroupsList;

        public int PositionNumber {
            set {
                if (value <= 0)
                {
                    Console.WriteLine("Number of position can't be less a zero");
                }
                else 
                { 
                    this._PositionNumber = value; 
                }
            }
            get { return _PositionNumber; } 
        }

        public SortedDictionary<int, RobotGroup> RobotGroupsList
        {
            set 
            { 
                _RobotGroupsList = value; 
            }
            get 
            { 
                return _RobotGroupsList; 
            }

        }
 
        public RobotPosition(int positionNumber, SortedDictionary<int, RobotGroup> robotGroupsList)
        {
            PositionNumber = positionNumber;
            RobotGroupsList = robotGroupsList;
        }

        public static RobotPosition ParsePositionFromString(string positionString)
        {
            positionString = positionString.Trim();

            //get position number
            string positionNumberString = positionString.Substring(0, positionString.IndexOf("GP"));
            Regex positionNumberRegex = new Regex("\\d+");
            int positionNumber = int.Parse(positionNumberRegex.Match(positionNumberString).Value);

            //get string with robot groups and parse to robot groups list
            SortedDictionary<int, RobotGroup> groupsDictionary = new SortedDictionary<int, RobotGroup>();

            string groups = positionString.Substring(positionString.IndexOf("GP"));
            groups = groups.Substring(0, groups.IndexOf("}"));

            List<string> strGroups = new(groups.Split("\n   "));

            RobotGroup robotGroup;
            for (int i = 0; i < strGroups.Count; i++)
            {
                string groupString = strGroups[i];
                
                string strGroupNumber = groupString.Substring(groupString.IndexOf("GP"), 3);
                int groupNumber = int.Parse(strGroupNumber.Substring(2));

                if (groupNumber == 1)
                {
                    robotGroup = new RobotFirstGroup(groupNumber);
                } else
                {
                    robotGroup = new RobotNotFirstGroup(groupNumber);
                }
                robotGroup.ParseGroupInPositionFromString(groupString);
                groupsDictionary.Add(robotGroup.Number, robotGroup);
            }
            return new RobotPosition(positionNumber, groupsDictionary);
        }
        public override string ToString()
        {
            string robotGroups = "";
            foreach (KeyValuePair<int, RobotGroup> robotGroup in _RobotGroupsList)
            {
                robotGroups += "   " + robotGroup.Value.ToString();
            }
            return "\nP[" + _PositionNumber.ToString() + "]{\n" +  robotGroups + "\n};";
        }
    }
}