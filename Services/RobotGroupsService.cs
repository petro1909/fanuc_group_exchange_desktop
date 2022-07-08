using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;
using System.Text.RegularExpressions;
using System.Collections;
using fanuc_group_exchange_desktop.Parser;

namespace fanuc_group_exchange_desktop.Services
{
    public class RobotGroupsService
    {
        public static List<bool> GetGroupsList(string groupString)
        {
            string[] usedGroupsStrArray = groupString.Split(',');
            List<bool> usedGroupsBoolList = new List<bool>(usedGroupsStrArray.Length);
            
            foreach(string usedGroup in usedGroupsStrArray)
            {
                usedGroupsBoolList.Add(usedGroup == "1");
            }

            return usedGroupsBoolList;
        }

        public static void DeleteGroup(RobotProgram program, int number)
        {
            //Delete in header section
            program.UsedGroupsList[number - 1] = false;
            //Delete in position section
            for(int i = 0; i < program.RobotPositions.Count; i++)
            {
                program.RobotPositions[i].RobotGroupsList.Remove(number);
            }
        }

        public static void SetGroup(RobotProgram program, RobotGroup group)
        {
            //Add in header section
            if (group.Number <= program.UsedGroupsList.Count)
            {
                program.UsedGroupsList[group.Number - 1] = true;
            }
            else
            {
                bool[] extended = new bool[group.Number - program.UsedGroupsList.Count];
                extended[^1] = true;
                program.UsedGroupsList.AddRange(extended);
            }

            //Add in position section
            foreach (RobotPosition position in program.RobotPositions)
            {
                if (position.RobotGroupsList.ContainsKey(group.Number))
                {
                    position.RobotGroupsList[group.Number] = group;
                }
                else
                {
                    position.RobotGroupsList.Add(group.Number, group);
                }
            }
        }
    }
}