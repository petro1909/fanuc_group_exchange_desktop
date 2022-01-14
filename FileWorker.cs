using fanuc_group_exchange_desktop.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop
{
    class FileWorker
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
            string fileStrings = File.ReadAllText(filePath);

            getFanucLSFileName(fileStrings);
            getFanucLSFileHeader(fileStrings);
            getFanucLSFileMain(fileStrings);
            getFanucLSFilePositions(fileStrings);

            return fileStrings;
        }

        public void WriteToFile(string path, string code)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine(code);
            }
        }
        
        //name
        public string GetFileName(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            FileName = file.Name;
            return FileName;
        }

        public void getFanucLSFileName(string code)
        {
            string fileName = code.Substring(0, code.IndexOf("/ATTR"));
            FileName = fileName.Substring(7);
        }

        public string setFanucLSFileNameLine()
        {
            return "/PROG  " + _FileName.ToUpper();
        }


        //header
        public void getFanucLSFileHeader(string code)
        {
            string header = code.Substring(0, code.IndexOf("/MN"));
            Header = header.Substring(header.IndexOf("/ATTR"));
            UsedGroupsList = GroupManipulator.GetUsedGroupsList(_Header);
        }


        public string setFanucLSFileUsedGroupsLine()
        {
            string usedGroupString = "";
            for (int i = 0; i < UsedGroupsList.Count; i++)
            {
                usedGroupString += UsedGroupsList[i];
                if (i != UsedGroupsList.Count - 1) usedGroupString += ",";
            }
            return "DEFAULT_GROUP\t= " + usedGroupString + ";\n";
        }


        public string setFanucLSFileHeaderBlock()
        {
            string headerBegin = _Header.Substring(0, _Header.IndexOf("DEFAULT_GROUP"));
            string headerEnd = _Header.Substring(_Header.IndexOf("CONTROL_CODE"));
            string groups = setFanucLSFileUsedGroupsLine();

            return headerBegin + groups + headerEnd;
        }

        //main
        private void getFanucLSFileMain(string code)
        {
            string mainWithPositions = code.Substring(code.IndexOf("/MN"));
            Main = mainWithPositions.Substring(0, mainWithPositions.IndexOf("/POS"));
            
        }

        private string setFanucLSFileMainBlock()
        {
            return _Main;
        }


        //positions
        private void getFanucLSFilePositions(string fileCode)
        {
            string positions = fileCode.Substring(fileCode.IndexOf("/POS")+4);
            Positions = positions.Substring(0, positions.LastIndexOf(";")+1);
        }


        public void setFanucLSFilePositions(List<RobotGroup> groupList)
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

        public string setFanucLSFilePositionsBlock()
        {
            return "/POS" + _Positions;
        }

        public string combineFileParts()
        {
            try
            {
                return setFanucLSFileNameLine() + setFanucLSFileHeaderBlock() + setFanucLSFileMainBlock() + setFanucLSFilePositionsBlock() + _EndOfFile;
            } catch(Exception e)
            {
                return e.Message + "";
            }
        }
    }
}