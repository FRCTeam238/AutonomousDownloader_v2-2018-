using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous_Downloader.Autonomous_x
{
    /// <summary>
    /// This is a command currently being part of the route.
    /// </summary>
    /// 
    /// Commands are found in the route. They are rooted to a template which 
    /// defines the name of the command and its parameter set.
    /// 
    /// The actual value of each command parameter is stored by this object.
    /// 
    public class Command
    {
        private CommandTemplate mTemplate = null;

        [Newtonsoft.Json.JsonIgnore]
        public String NameExtended
        {
            get
            {
                String result = Name;

                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    if (String.IsNullOrEmpty(Parameters[parameterIndex]))
                    {
                        result += String.Format(" {0}", mTemplate.GetParameterName(parameterIndex));
                    }
                    else
                    {
                        result += String.Format(" '{0}'", Parameters[parameterIndex]);
                    }
                }

                    return result;
            }
        }

        public String Name 
        { 
            get
            {
                return mTemplate.CommandName;
            }
            /* set; */
        }

        public ObservableCollection<String> Parameters
        {
            get
            {
                ObservableCollection<String> valueList = new ObservableCollection<String>();
                foreach (ParameterInstance inst in ParameterInstances)
                {
                    valueList.Add(inst.Value);
                }

                return valueList;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public ObservableCollection<ParameterInstance> ParameterInstances { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        public Command(String name, String[] parameters)
        {
            CommandTemplate baseTemplate = CommandTemplate.FindCommandByName(name);
            mTemplate = baseTemplate;
            LoadCommandFromTemplate(parameters, baseTemplate);
        }

        public Command(CommandTemplate baseTemplate)
        {
            mTemplate = baseTemplate;
            LoadCommandFromTemplate(null, baseTemplate);
        }

        private void LoadCommandFromTemplate(String[] parameters, CommandTemplate baseTemplate)
        {
            ParameterInstances = new ObservableCollection<ParameterInstance>();
            int numberOfParameters = baseTemplate.NumberOfParameters;
            for (int index = 0; index < numberOfParameters; index++)
            {
                ParameterInstance inst = baseTemplate.CreateParameterInstance(index);

                if ((parameters != null) && (index < parameters.Length))
                {
                    inst.Value = parameters[index];
                }

                ParameterInstances.Add(inst);
            }
        }
    }
}
