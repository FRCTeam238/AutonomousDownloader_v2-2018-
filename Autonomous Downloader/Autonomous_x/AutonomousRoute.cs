using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous_Downloader.Autonomous_x
{
    /// <summary>
    /// A list of commands to execute in the order specified by a user.
    /// </summary>
    /// 
    /// The autonomous route is an order sequence of events, steps for the robot to perform
    /// as part of a specific route.
    /// 
    public class AutonomousRoute
    {
        public String Name { get; set; }
        public ObservableCollection<Command> Commands = new ObservableCollection<Command>();

        public AutonomousRoute(String name)
        {
            Name = name;
        }
    }
}
