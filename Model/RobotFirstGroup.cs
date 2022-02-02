using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public class RobotFirstGroup : RobotGroup
    {
        private string _CoordinatesString = "";

        public string CoordinatesString
        {
            set
            {
                _CoordinatesString = value;
            }
            get
            {
                return _CoordinatesString;
            }
        }
        public RobotFirstGroup()
        {
        }

        public RobotFirstGroup(int Number)
        {
            this.Number = Number;
        }

        public RobotFirstGroup(int Number, string CoordinatesString)
        {
            this.Number = Number;
            this.CoordinatesString = CoordinatesString;
        }

        public override string ToString()
        {
            return _CoordinatesString.Substring(0, _CoordinatesString.Length-1);
        }
    }
}
