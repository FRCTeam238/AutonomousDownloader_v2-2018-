using Autonomous_Downloader.Autonomous_x;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Autonomous_Downloader
{
    /// <summary>
    /// Interaction logic for ProgramPanel.xaml
    /// </summary>
    /// 
    /// Creates a consolidated sub-panel within the AutonomousWindow that is 
    /// responsible for allowing the user to edit a single route.
    /// 
    public partial class ProgramPanel : UserControl
    {

        /// <summary>
        /// The current set of commands being shown in the commands panel.
        /// </summary>
        private CommandTemplate[] CommandSet = null;

        /// <summary>
        /// The route name to present to the user.
        /// </summary>
        public String ProgramNameLabel
        {
            get { return (String)ProgramNameLbl.Content; }
            set { ProgramNameLbl.Content = value; }
        }

        /// <summary>
        /// Ordered list of commands representing the route being edited
        /// </summary>
        public ObservableCollection<Autonomous_x.Command> Commands
        {
            get
            {
                return (ObservableCollection<Autonomous_x.Command>)ProgramCommandsLB.ItemsSource;
            }
            set
            {
                ProgramCommandsLB.ItemsSource = value;
                if (!ProgramCommandsLB.Items.IsEmpty)
                {
                    ProgramCommandsLB.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Parameters associated with the current selected route step.
        /// </summary>
        public ObservableCollection<ParameterInstance> Parameters
        {
            get
            {
                return (ObservableCollection<ParameterInstance>)ProgramParametersLB.ItemsSource;
            }

            set
            {
                ProgramParametersLB.ItemsSource = value;
            }
        }

        /// <summary>
        /// The current selected command in the commands list.
        /// </summary>
        public Command SelectedCommand
        {
            get
            {
                if (ProgramCommandsLB.SelectedItem != null)
                {
                    return (Command)ProgramCommandsLB.SelectedItem;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Load the command set from its JSON file.
        /// </summary>
        /// 
        /// The command set is the list of CommandTemplate objects available for the user to
        /// add to a route.
        /// 
        /// <param name="filepath">The filename</param>
        /// <returns>Returns true if the file was loaded successfully, otherwise false is returned.</returns>
        private bool LoadCommandSet(String filepath)
        {
            bool retval = false;

            try
            {
                CommandSet = CommandTemplate.LoadCommandSet(filepath);
                retval = true;
            }
            catch(Exception ex)
            {
                String msg = String.Format("Unable to load file {0}\n", filepath);
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return retval;
        }

        /// <summary>
        /// Save commands to a file.
        /// </summary>
        /// 
        /// This function is primarily used to create a new command set file.
        /// 
        /// <param name="filepath">The name of the file to save to</param>
        /// 
        private void SaveCommandSet(String filepath)
        {
            CommandTemplate.SaveCommandSet(CommandSet, filepath);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgramPanel()
        {
            InitializeComponent();

            InitializeCommandsList();
        }

        /// <summary>
        /// Load commands into the display.
        /// </summary>
        /// 
        /// Load the commands.json file. If the file cannot be loaded a default
        /// file is created and saved to the current working directory and then
        /// also attached to the commands list.
        /// 
        private void InitializeCommandsList()
        {
            if (!LoadCommandSet("commands.json"))
            {
                CommandSet = new CommandTemplate[]
                {
                    new CommandTemplate("CollectorIn"),
                    new CommandTemplate("CollectorOut"),

                    new CommandTemplate("DriveForward", new String[] { "Target", "Speed" } ),
                    new CommandTemplate("DriveBackwards", new String[] { "Target", "Speed" }),
                    new CommandTemplate("TurnLeft", new String[] { "Target", "Speed", "Other" }),
                    new CommandTemplate("TurnRight", new String[] { "Target", "Speed", "Other" }),

                    new CommandTemplate("Finished")
                };
                SaveCommandSet("commands.json");
            }

            CommandTemplate.CommandSet = CommandSet;
            CommandTemplateLB.ItemsSource = CommandSet;
            CommandTemplateLB.SelectedIndex = 0;
        }

        /// <summary>
        /// An different route step has been selected.
        /// </summary>
        /// 
        /// This function will update the parameters display the parameters from the newly
        /// selected route item.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgramCommandsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Command selectedCommand = SelectedCommand;
            if (selectedCommand != null)
            {
                //C ProgramParametersLB.ItemsSource = selectedCommand.Parameters;
                ProgramParametersLB.ItemsSource = selectedCommand.ParameterInstances;
            }
            else
            {
                ProgramParametersLB.ItemsSource = null;
            }
        }

        /// <summary>
        /// Add a new step to the route.
        /// </summary>
        /// 
        /// This function will insert a new command into the route based
        /// on the location in the route that is selected and the command
        /// selected in the command set.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CommandTemplateLB.SelectedItem != null)
            {
                CommandTemplate item = (CommandTemplate)CommandTemplateLB.SelectedItem;
                if (item != null)
                {
                    Command command = item.CreateCommandInstance();
                    if (ProgramCommandsLB.SelectedItem != null)
                    {
                        int index = ProgramCommandsLB.SelectedIndex + 1;
                        Commands.Insert(index, command);
                        ProgramCommandsLB.SelectedIndex = index;
                    }
                    else
                    {
                        Commands.Add(command);
                        ProgramCommandsLB.SelectedIndex = ProgramCommandsLB.Items.Count - 1;
                    }   
                }
            }
        }

        /// <summary>
        /// Remove the current selected route step.
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramCommandsLB.SelectedIndex >= 0)
            {
                int index = ProgramCommandsLB.SelectedIndex;
                Commands.RemoveAt(index);
                if (index < ProgramCommandsLB.Items.Count)
                {
                    ProgramCommandsLB.SelectedIndex = index;
                }
                else if (ProgramCommandsLB.Items.Count > 0)
                {
                    ProgramCommandsLB.SelectedIndex = ProgramCommandsLB.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Move the current selected route step down by one.
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((ProgramCommandsLB.SelectedIndex >= 0) && (ProgramCommandsLB.SelectedIndex < ProgramCommandsLB.Items.Count - 1))
            {
                int index = ProgramCommandsLB.SelectedIndex;
                Command item = Commands[index];
                Commands.RemoveAt(index);
                Commands.Insert(index + 1, item);
                ProgramCommandsLB.SelectedIndex = index + 1;
            }
        }

        /// <summary>
        /// Move the current selected route step up by one.
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            // if the index is set and greater than the first element
            // move it up
            if (ProgramCommandsLB.SelectedIndex > 0)
            {
                int index = ProgramCommandsLB.SelectedIndex;
                Command item = Commands[index];
                Commands.RemoveAt(index);
                Commands.Insert(index - 1, item);
                ProgramCommandsLB.SelectedIndex = index - 1;
            }
        }

        /// <summary>
        /// A keyboard key has been issued in a parameter input field.
        /// </summary>
        /// 
        /// If the key is returned the value in the field is stored back
        /// to the parameters data store, the input is disabled, and
        /// the label is shown with the new value.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ParameterEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox box = (TextBox)sender;
                e.Handled = true;

                TextBlock block = (TextBlock)box.Tag;
                box.Visibility = System.Windows.Visibility.Collapsed;
                block.Visibility = System.Windows.Visibility.Visible;

                block.Text = box.Text;

                int selectedIndex = ProgramParametersLB.SelectedIndex;

                try
                {
                    //C Parameters[selectedIndex] = Convert.ToInt32(box.Text);
                    Parameters[selectedIndex].Value = box.Text;
                }
                catch (Exception /*ex*/)
                {
                    /* //TODO do something with the error */
                }
                RefreshCommandList();
            }
        }

        /// <summary>
        /// A click has been issued in a parameter display row.
        /// </summary>
        /// 
        /// This function will switch the parameter display to an editable
        /// input. The field
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParameterTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBlock block = (TextBlock)sender;
                Panel owner = (Panel)block.Parent;
                TextBox box = (TextBox)block.Tag;

                block.Visibility = System.Windows.Visibility.Collapsed;
                box.Visibility = System.Windows.Visibility.Visible;
                box.Text = block.Text;
                box.CaretIndex = box.Text.Length;
                Keyboard.Focus(box);
            }
        }

        /// <summary>
        /// Add a new command to the current route.
        /// </summary>
        /// 
        /// This function will create a new command from the selected command
        /// and add it to the current route after the current selection.
        /// 
        /// After the command is inserted selection switches to the new command.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void CommandTemplateLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CommandTemplateLB.SelectedItem != null)
            {
                CommandTemplate item = (CommandTemplate)CommandTemplateLB.SelectedItem;
                if (item != null)
                {
                    Command command = item.CreateCommandInstance();
                    if (ProgramCommandsLB.SelectedItem != null)
                    {
                        int index = ProgramCommandsLB.SelectedIndex + 1;
                        Commands.Insert(index, command);
                        ProgramCommandsLB.SelectedIndex = index;
                    }
                    else
                    {
                        Commands.Add(command);
                        ProgramCommandsLB.SelectedIndex = ProgramCommandsLB.Items.Count - 1;
                    }   
                }
            }
        }

        /// <summary>
        /// Force the contents of the a parameter back into the program parameters storage.
        /// </summary>
        /// 
        /// This function fires as the user is pressing a new key in a parameter entry field.
        /// It will force the resulting value back into the storage record for the parameters.
        /// 
        /// //TODO consider another attempt to see if the text editor can be bound directly
        ///   to the parameter in the backing store. That way WPF can take care of maintaining
        ///   the value in the store.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ParameterEntry_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int selectedIndex = ProgramParametersLB.SelectedIndex;
            if (selectedIndex >= 0)
            {
                TextBox box = (TextBox)sender;

                try
                {
                    ParameterInstance pi = (ParameterInstance)Parameters[selectedIndex];
                    Parameters[selectedIndex].Value = box.Text;
                }
                catch (Exception /*ex*/)
                {
                    /* //TODO do something with the error */
                }
                RefreshCommandList();
            }
        }

        /// <summary>
        /// Reload the commands list to ensure that it is up to date.
        /// </summary>
        /// 
        private void RefreshCommandList()
        {
            ProgramCommandsLB.Items.Refresh();
        }
    }
}
