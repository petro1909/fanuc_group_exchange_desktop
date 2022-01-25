using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fanuc_group_exchange_desktop.Model
{
    public abstract class BasicInstance : Parser
    {
        public int _Number;


        public int Number
        {
            set
            {
                if (value <= 0)
                {
                    Console.WriteLine("Number can't be less a zero");
                }
                else
                {
                    this._Number = value;
                }
            }
            get { return _Number; }
        }

        public abstract void Parse(string str);


    }
}
