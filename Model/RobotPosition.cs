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

        
        public override string ToString()
        {
            string robotGroups = "";
            foreach (KeyValuePair<int, RobotGroup> robotGroup in _RobotGroupsList)
            {
                robotGroups += "   " + robotGroup.Value.ToString();
            }
            return "\r\nP[" + Number.ToString() + _PositionComment +  "]{" +  robotGroups + "\n};";
        }
    }
}