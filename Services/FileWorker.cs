﻿using fanuc_group_exchange_desktop.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;

namespace fanuc_group_exchange_desktop.Services
{
    public class FileWorker
    {
        private string _FileName;
        private List<string> _UsedGroupsList;
        private string _Header;
        private string _Main;
        private string _Positions;
        private string _EndOfFile = "\r\n/END";


        public string FileName
        {
            set { _FileName = value; }
            get { return _FileName; }
        }

        public string Header
        {
            set { _Header = value; }
            get { return _Header; }
        }

        public List<string> UsedGroupsList
        {
            set { _UsedGroupsList = value; }
            get { return _UsedGroupsList; }
        }

        public string Main
        {
            set { _Main = value; }
            get { return _Main; }
        }

        public string Positions
        {
            set { _Positions = value; }
            get { return _Positions; }
        }

        public string ReadFromFile(string filePath)
        {
            StringBuilder builder = new StringBuilder(File.ReadAllText(filePath));
            string fileStrings = File.ReadAllText(filePath);

            GetFanucLSFileName(fileStrings);
            GetFanucLSFileHeader(fileStrings);
            GetFanucLSFileMain(fileStrings);
            GetFanucLSFilePositions(fileStrings);

            return fileStrings;
        }

        public void WriteToFile(string path, string code)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine(code);
            }
        }
        
        //Fanuc program name
        public string GetFileName(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            FileName = file.Name;
            return FileName;
        }

        private void GetFanucLSFileName(string code)
        {
            string fileName = code.Substring(0, code.IndexOf("/ATTR"));
            FileName = fileName.Substring(7);
        }

        private string SetFanucLSFileNameLine()
        {
            return "/PROG  " + _FileName.ToUpper();
        }


        //header
        private void GetFanucLSFileHeader(string code)
        {
            string header = code.Substring(0, code.IndexOf("/MN"));
            Header = header.Substring(header.IndexOf("/ATTR"));
            UsedGroupsList = GroupManipulator.GetUsedGroupsList(_Header);
        }


        private string SetFanucLSFileUsedGroupsLine()
        {
            string usedGroupString = "";
            for (int i = 0; i < UsedGroupsList.Count; i++)
            {
                usedGroupString += UsedGroupsList[i];
                if (i != UsedGroupsList.Count - 1) usedGroupString += ",";
            }
            return "DEFAULT_GROUP\t= " + usedGroupString + ";\n";
        }

        private string SetFanucLSFileHeaderBlock()
        {
            string headerBegin = _Header.Substring(0, _Header.IndexOf("DEFAULT_GROUP"));
            string headerEnd = _Header.Substring(_Header.IndexOf("CONTROL_CODE"));
            string groups = SetFanucLSFileUsedGroupsLine();

            return headerBegin + groups + headerEnd;
        }

        //main
        private void GetFanucLSFileMain(string code)
        {
            string mainWithPositions = code.Substring(code.IndexOf("/MN"));
            Main = mainWithPositions.Substring(0, mainWithPositions.IndexOf("/POS"));
            
        }

        private string SetFanucLSFileMainBlock()
        {
            return _Main;
        }


        //positions
        private void GetFanucLSFilePositions(string fileCode)
        {
            string positions = fileCode.Substring(fileCode.IndexOf("/POS")+4);
            Positions = positions.Substring(0, positions.LastIndexOf(";")+1);
        }


        public void SetFanucLSFilePositions(List<RobotGroup> groupList)
        {

            UsedGroupsList = GroupManipulator.GetListOfUsedGroupsInHeader(_UsedGroupsList, groupList);

            List<RobotPosition> originalPositions = GroupManipulator.GetPositionList(_Positions);
            List<RobotPosition> positionList = GroupManipulator.GetListOfPositionsWithAddedGroups(originalPositions, groupList);
            _Positions = "";
            foreach (RobotPosition position in positionList)
            {
               _Positions += position.ToString();
            }
        }

        public void DeleteGroup(int groupNumber)
        {
            GroupManipulator.DeleteGroupInHeader(_UsedGroupsList, groupNumber);
            
            List<RobotPosition> positions = GroupManipulator.GetPositionList(_Positions);
            GroupManipulator.deleteGroupInPosition(positions, groupNumber);
            _Positions = "";
            foreach (RobotPosition position in positions)
            {
                _Positions += position.ToString();
            }
        }



        private string SetFanucLSFilePositionsBlock()
        {
            return "/POS" + _Positions;
        }

        public StringBuilder CombineFileParts()
        {
            try
            {

                return new StringBuilder(SetFanucLSFileNameLine() + SetFanucLSFileHeaderBlock() + SetFanucLSFileMainBlock() + SetFanucLSFilePositionsBlock() + _EndOfFile);
            }
            catch (Exception e)
            {
                return new StringBuilder(e.Message + "");
            }
        }
    }
}