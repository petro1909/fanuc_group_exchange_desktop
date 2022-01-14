﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;
using System.Text.RegularExpressions;
using System.Collections;

namespace fanuc_group_exchange_desktop
{
    public class GroupManipulator
    {
        public static List<string> GetUsedGroupsList(string header)
        {
            Regex groupStringRegex = new Regex("DEFAULT_GROUP\\t=\\s{1}((1|\\*),)+(1|\\*)\\;");
            string groupString = groupStringRegex.Match(header).Value;

            Regex groupElementRegex = new Regex("(\\*|1)");
            string groupElement = groupElementRegex.Match(groupString).Value;

            groupString = groupString.Substring(groupString.IndexOf(groupElement), groupString.IndexOf(";") - groupString.IndexOf(groupElement));
            List<string> usedGroups = new List<string>(groupString.Split(','));

            return usedGroups;
        }

        public static List<RobotPosition> GetPositionList(string positions)
        {
            positions = positions.Substring(0, positions.LastIndexOf(";"));
            try
            {
                List<string> positionStringList = new List<string>(positions.Split(";"));

                List<RobotPosition> positionList = new List<RobotPosition>();

            
                foreach (string positionString in positionStringList)
                {
                    positionList.Add(RobotPosition.ParsePositionFromString(positionString));
                }
                return positionList;
            } catch(NullReferenceException e)
            {
                return null;
            }
        }

        public static List<string> GetListOfUsedGroupsInHeader(List<string> originUsedGroupList, List<RobotGroup> addedGroupList)
        {
            foreach (RobotGroup robotGroup in addedGroupList)
            {
                int diff = robotGroup.Number - originUsedGroupList.Count;
                if (diff > 0)
                {
                    originUsedGroupList.AddRange(Enumerable.Repeat("*", diff));
                }
                originUsedGroupList[robotGroup.Number - 1] = "1";
            }
            return originUsedGroupList;
        }

        public static List<RobotPosition> GetListOfPositionsWithAddedGroups(List<RobotPosition> originUsedGroupList, List<RobotGroup> addedGroupList)
        {
            foreach (RobotPosition position in originUsedGroupList)
            {
                foreach (RobotGroup robotGroup in addedGroupList)
                {
                    position.RobotGroupsList.Add(robotGroup.Number, robotGroup);
                }
                
            }
            return originUsedGroupList;
        }
    }
}