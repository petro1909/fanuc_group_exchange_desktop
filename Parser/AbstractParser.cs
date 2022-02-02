using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;

namespace fanuc_group_exchange_desktop.Parser
{
    abstract class AbstractParser
    {
        public abstract BasicInstance Parse(string code);
    }
}
