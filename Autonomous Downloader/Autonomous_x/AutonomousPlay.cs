using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous_Downloader.Autonomous_x
{
    public class AutonomousPlay
    {
        public String Name { get; set; }

        private List<string> mL_LL_str = new List<string> { "" };
        private List<string> mC_LL_str = new List<string> { "" };
        private List<string> mR_LL_str = new List<string> { "" };

        private List<string> mL_LR_str = new List<string> { "" };
        private List<string> mC_LR_str = new List<string> { "" };
        private List<string> mR_LR_str = new List<string> { "" };

        private List<string> mL_RR_str = new List<string> { "" };
        private List<string> mC_RR_str = new List<string> { "" };
        private List<string> mR_RR_str = new List<string> { "" };

        private List<string> mL_RL_str = new List<string> { "" };
        private List<string> mC_RL_str = new List<string> { "" };
        private List<string> mR_RL_str = new List<string> { "" };

        public List<string> L_LL
        {
            get
            {
                return mL_LL_str;
            }
            set
            {
                mL_LL_str = value;
            }
        }

        public List<string> C_LL
        {
            get
            {
                return mC_LL_str;
            }
            set
            {
                mC_LL_str = value;
            }
        }

        public List<string> R_LL
        {
            get
            {
                return mR_LL_str;
            }
            set
            {

                mR_LL_str = value;
            }
        }


        public List<string> L_LR
        {
            get
            {
                return mL_LR_str;
            }
            set
            {
                mL_LR_str = value;
            }
        }

        public List<string> C_LR
        {
            get
            {
                return mC_LR_str;
            }
            set
            {
                mC_LR_str = value;
            }
        }

        public List<string> R_LR
        {
            get
            {
                return mR_LR_str;
            }
            set
            {
                mR_LR_str = value;
            }
        }

        public List<string> L_RR
        {
            get
            {
                return mL_RR_str;
            }
            set
            {
                mL_RR_str = value;
            }
        }

        public List<string> C_RR
        {
            get
            {
                return mC_RR_str;
            }
            set
            {
                mC_RR_str = value;
            }
        }

        public List<string> R_RR
        {
            get
            {
                return mR_RR_str;
            }
            set
            {
                mR_RR_str = value;
            }
        }

        public List<string> L_RL
        {
            get
            {
                return mL_RL_str;
            }
            set
            {
                mL_RL_str = value;
            }
        }

        public List<string> C_RL
        {
            get
            {
                return mC_RL_str;
            }
            set
            {
                mC_RL_str = value;
            }
        }

        public List<string> R_RL
        {
            get
            {
                return mR_RL_str;
            }
            set
            {
                mR_RL_str = value;
            }
        }
        
        public AutonomousPlay(string name)
        {
            Name = name;
        }
    }
}
