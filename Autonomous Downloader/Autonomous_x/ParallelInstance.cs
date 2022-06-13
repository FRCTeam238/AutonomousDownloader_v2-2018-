using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous_Downloader.Autonomous_x
{
	public class ParallelInstance : ParameterInstance
	{
		private List<string> parallelTypes = new List<String>() 
			{ "None", "Deadline_Leader", "Deadline_Follower", "Parallel", "Race" };

		public ParallelInstance(CommandTemplate template, int index) : base(template, index)
		{
			Value = "None";
		}

		public List<string> ParallelTypes { get => parallelTypes; set => parallelTypes = value; }
	}
}
