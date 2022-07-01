using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class RobotProgram
    {
        private string _Name;
        private string _HeaderSection;
        private string _MainSection;
        private string _PositionSection;

        private List<bool> _UsedGroupsList;
        private List<RobotPosition> _RobotPositions;

        public RobotProgram(string name, string headerSection, string mainSection, string positionSection, List<RobotPosition> positionList,  List<bool> usedGroups)
        {
            Name = name;
            HeaderSection = headerSection;
            MainSection = mainSection;
            PositionSection = positionSection;
            RobotPositions = positionList;
            UsedGroupsList = usedGroups;
        }

        public string Name
        {
            set
            {
                this._Name = value;
            }
            get
            {
                return _Name;
            }
        }

        public string HeaderSection
        {
            set
            {
                this._HeaderSection = value;
            }
            get
            {
                return _HeaderSection;
            }
        }

        public string MainSection
        {
            set
            {
                this._MainSection = value;
            }
            get
            {
                return _MainSection;
            }
        }

        public string PositionSection
        {
            set
            {
                this._PositionSection = value;
            }
            get
            {
                return _PositionSection;
            }
        }

        public List<bool> UsedGroupsList 
        {
            set 
            {
                this._UsedGroupsList = value;
            }
            get
            {
                return _UsedGroupsList;
            }
        }

        public List<RobotPosition> RobotPositions
        {
            set
            {
                this._RobotPositions = value;
            }
            get
            {
                return _RobotPositions;
            }
        }

        public string EndOfFile
        {
            get
            {
                return "\nEND";
            }
        }



    }
}
