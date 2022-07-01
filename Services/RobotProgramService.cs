using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fanuc_group_exchange_desktop.Parser;
using fanuc_group_exchange_desktop.Model;

namespace fanuc_group_exchange_desktop.Services
{
    public class RobotProgramService
    {
        public RobotProgram GetFanucLSProgram(string fileText)
        {
            string headerSection = GetFanucLSFileHeaderSection(fileText);
            string name = GetFanucLSFileName(headerSection);

            string groupString = GetFanucLSFileGroupsString(headerSection);
            List<bool> usedGroupsList = RobotGroupsService.GetGroupsList(groupString);

            string mainSection = GetFanucLSFileMainSection(fileText);
            string positionsSection = GetFanucLSFilePositionSection(fileText);
            List<RobotPosition> positionList = GetFanucLSFilePositionList(positionsSection);

            return new RobotProgram(name, headerSection, mainSection, positionsSection, positionList, usedGroupsList);
        }

        private string GetFanucLSFileHeaderSection(string fileText)
        {
            string header = fileText[..fileText.IndexOf("/MN")];
            return header;
        }

        private string GetFanucLSFileName(string header)
        {
            int nameBeginIndex = header.IndexOf(" ");
            int nameEndIndex = header.IndexOf("\r\n");
            string fileName = header[nameBeginIndex..nameEndIndex].Trim();
            return fileName;
        }

        private string GetFanucLSFileGroupsString(string header)
        {
            int groupsLineStartIndex = header.IndexOf("DEFAULT_GROUP");
            int groupsLineEndIndex = header.IndexOf(";\r\n", groupsLineStartIndex);
            int groupsListStartIndex = header.IndexOfAny(new char[] { '1', '*' }, groupsLineStartIndex);
            
            string groups = header[groupsListStartIndex..groupsLineEndIndex];
            return groups;
        }

        private string GetFanucLSFileMainSection(string code)
        {
            int mainSectionStartIndex = code.IndexOf("/MN");
            int mainSectionEndIndex = code.IndexOf("/POS");
            
            string main = code[mainSectionStartIndex..mainSectionEndIndex];
            return main;
        }

        private string GetFanucLSFilePositionSection(string code)
        {
            int positionSectionStartIndex = code.IndexOf("/POS");
            int positionSectionEndIndex = code.LastIndexOf(";") + 1;
            
            string positions = code[positionSectionStartIndex.. positionSectionEndIndex];
            return positions;
        }

        private List<RobotPosition> GetFanucLSFilePositionList(string positionSection)
        {
            positionSection = positionSection[positionSection.IndexOf("P[")..];
            try
            {
                List<string> positionStringList = new List<string>(positionSection.Split(";\r\n"));

                List<RobotPosition> positionList = new List<RobotPosition>();
                PositionParser positionParser = new PositionParser();
                foreach (string positionString in positionStringList)
                {
                    RobotPosition robotPosition = positionParser.ParsePosition(positionString);
                    positionList.Add(robotPosition);
                }
                return positionList;
            }
            catch (NullReferenceException e)
            {
                return null;
            }
        }

        public void SetFanucLSFileName(RobotProgram program, string name)
        {
            //if (Validator.ValidateRpogramName(name))
            //{
                program.Name = name.ToUpper();
            //}
        }

        private string SetFanucLSFileNameLine(RobotProgram program)
        {
            string nameLine = $"/PROG  {program.Name}";
            return nameLine;
        }

        private string SetFanucLSFileUsedGroupsLine(RobotProgram program)
        {
            int groupListCount = program.UsedGroupsList.Count;
            List<string> groupListString = new List<string>();
            program.UsedGroupsList.ForEach(e =>
            {
                if (e)
                {
                    groupListString.Add("1");
                }
                else
                {
                    groupListString.Add("*");
                }
            });
            string groupsString = string.Join(',', groupListString);
            return $"DEFAULT_GROUP\t= {groupsString};";
        }

        private void SetFanucLSFileHeaderSection(RobotProgram program)
        {
            string header = program.HeaderSection;

            int groupsLineStartIndex = header.IndexOf("DEFAULT_GROUP");
            int groupsLineEndIndex = header.IndexOf("\r\n", groupsLineStartIndex);
            string groupLine = header[groupsLineStartIndex..groupsLineEndIndex];
            header = header.Replace(groupLine, SetFanucLSFileUsedGroupsLine(program));

            int nameEndIndex = header.IndexOf("\r\n");
            string nameLine = header[..nameEndIndex];
            header = header.Replace(nameLine, SetFanucLSFileNameLine(program));

            program.HeaderSection = header.ToString();
        }

        public void SetFanucLSFilePositionsSection(RobotProgram program)
        {
            StringBuilder positionStr = new StringBuilder("/POS");
            foreach (RobotPosition position in program.RobotPositions)
            {
                positionStr.Append(position.ToString());
            }
            program.PositionSection = positionStr.ToString();
        }

        public void DeleteGroup(RobotProgram program, int groupNumber)
        {
            RobotGroupsService.DeleteGroup(program, groupNumber);
        }

        public void SetGroup(RobotProgram program, RobotGroup group)
        {
            RobotGroupsService.SetGroup(program, group);
        }

        public void AddGroups(RobotProgram program, List<RobotGroup> groups)
        {
            foreach(RobotGroup group in groups)
            {
                SetGroup(program, group);
            }
        }

        public string CombineFileParts(RobotProgram program)
        {
            SetFanucLSFileHeaderSection(program);
            SetFanucLSFilePositionsSection(program);
            
            StringBuilder fileText = new StringBuilder();
            fileText.Append(program.HeaderSection)
                    .Append(program.MainSection)
                    .Append(program.PositionSection)
                    .Append(program.EndOfFile);
            
            return fileText.ToString();
        }
    }
}
