using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous_Downloader.Autonomous_x
{
    public class TrajectoryInstance : ParameterInstance
    {
        private List<string> trajectoryNames = new List<String>();

        public TrajectoryInstance (CommandTemplate template, int parameterIndex, List<string> trajectories) : base (template, parameterIndex)
        {
            trajectoryNames = trajectories;
        }

        public List<string> TrajectoryNames { get => trajectoryNames; set => trajectoryNames = value; }
    }
}
