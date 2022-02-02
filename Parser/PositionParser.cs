using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;

namespace fanuc_group_exchange_desktop.Parser
{
    class PositionParser : AbstractParser
    {
        public override BasicInstance Parse(string positionString)
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

                GroupParser groupParser = new GroupParser();
                RobotGroup robotGroup = groupParser.Parse(strGroups[i]) as RobotGroup; 

                groupsDictionary.Add(robotGroup.Number, robotGroup);
            }
            return new RobotPosition(positionNumber, positionComment, groupsDictionary);
        }
    }
}
