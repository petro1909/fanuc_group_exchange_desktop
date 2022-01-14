using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public abstract class RobotGroup
    {

        protected internal int _Number;
       

        public int Number {
            set{
                if (value <= 0) 
                { 
                    Console.WriteLine("Group number can't be less or be a zero"); 
                }
                else
                {
                    _Number = value;
                } 
            }
            get { return _Number; }   
        }

        public abstract void ParseGroupInPositionFromString(string groupString);
    }
}
