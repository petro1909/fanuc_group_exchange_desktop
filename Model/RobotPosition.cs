using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class RobotPosition : BasicInstance
    {
        private string _PositionComment;
        private SortedDictionary<int, RobotGroup> _RobotGroupsList;


        public string PositionComment
        {
            set { this._PositionComment = value; }
            get { return _PositionComment; }
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
 
        public RobotPosition()
        {

        }

        public RobotPosition(int positionNumber, string positionComment, SortedDictionary<int, RobotGroup> robotGroupsList)
        {
            Number = positionNumber;
            RobotGroupsList = robotGroupsList;
            PositionComment = positionComment;
        }

        public override void Parse(string positionString)
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
                robotGroup.Parse(groupString);
                groupsDictionary.Add(robotGroup.Number, robotGroup);
            }
            this._Number = positionNumber;
            this._PositionComment = positionComment;
            this._RobotGroupsList = groupsDictionary;
        }
        public override string ToString()
        {
            string robotGroups = "";
            foreach (KeyValuePair<int, RobotGroup> robotGroup in _RobotGroupsList)
            {
                robotGroups += "   " + robotGroup.Value.ToString();
            }
            return "\nP[" + _Number.ToString() + _PositionComment +  "]{\n" +  robotGroups + "\n};";
        }
    }
}