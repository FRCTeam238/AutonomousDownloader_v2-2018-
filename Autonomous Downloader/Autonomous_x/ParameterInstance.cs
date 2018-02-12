using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous_Downloader.Autonomous_x
{
    public class ParameterInstance
    {
        private CommandTemplate mTemplate;
        private int mParameterIndex;

        public String Value
        {
            get;
            set;
        }

        public String Name
        {
            get
            {
                return mTemplate.ParameterNames[mParameterIndex];
            }
        }

        public ParameterInstance(CommandTemplate template, int parameterIndex)
        {
            mTemplate = template;
            mParameterIndex = parameterIndex;
            Value = "";
        }
    }
}
