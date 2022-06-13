using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Autonomous_Downloader.Autonomous_x
{
    public class ParameterInstance : DependencyObject
    {
        private CommandTemplate mTemplate;
        private int mParameterIndex;
        private string value;

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(ProgramPanel));

        public String Value
        {
            get => (string)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                this.value = value;
                TaskDefinitionWindow.dirty = true;
            }
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
