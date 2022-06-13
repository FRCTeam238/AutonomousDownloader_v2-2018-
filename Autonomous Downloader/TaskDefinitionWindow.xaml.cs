using Autonomous_Downloader.Autonomous_x;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Text.RegularExpressions;

namespace Autonomous_Downloader
{
    /// <summary>
    /// Interaction logic for AutonomousWindow.xaml
    /// </summary>
    /// 
    /// This represents the main window for the autonomous downloader.
    /// 
    /// 
    public partial class TaskDefinitionWindow : Window
    {
        /// <summary>
        /// The displayed program version
        /// </summary>
        /// 
        public const String ProgramVersion = "2020 Beta";

        public const String commandDirectory = "\\src\\main\\java\\frc\\robot\\commands";
        public const String trajectoryDirectory = "\\src\\main\\deploy\\pathplanner\\generatedJSON";

		private static bool dirty = false;

        /// <summary>
        /// The main list of routes.
        /// </summary>
        /// 
        private RouteGroup mProgramModes = null;

        /// <summary>
        /// The name of the last file loaded or saved.
        /// </summary>
        /// 
        String mSaveFilename;

        /// <summary>
        /// Bindable property of the last filename saved or loaded
        /// </summary>
        /// 
        private String SaveFilename
        {
            get
            {
                return mSaveFilename;
            }
            set
            {
                mSaveFilename = value;
                SetWindowTitle(mSaveFilename);
            }
        }

        /// <summary>
        /// Current selected autonomous route.
        /// </summary>
        /// 
        private AutonomousRoute SelectedProgram
        {
            get
            {
                if (ProgramModeLB.SelectedItem != null)
                {
                    return (AutonomousRoute)ProgramModeLB.SelectedItem;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Current selected command.
        /// </summary>
        /// 
        private Command SelectedCommand
        {
            get
            {
                return ProgramPnl.SelectedCommand;
            }
        }

		public static bool Dirty { 
        get => dirty; 
        set => dirty = value; 
        }

		/// <summary>
		/// Constructor
		/// </summary>
		public TaskDefinitionWindow()
        {
            InitializeComponent();

            InitializeProgram();
        }

        /// <summary>
        /// Initialize the program.
        /// </summary>
        /// 
        /// A new empty route is created.
        /// The window title is set.
        /// 
        private void InitializeProgram()
        {
            RouteGroup programModes = new RouteGroup();
            programModes.AutonomousModes.Add(new AutonomousRoute("new"));
            SetWindowTitle("");
            AddNewRoute(programModes);
        }

        /// <summary>
        /// Set the window title using a standard format.
        /// </summary>
        /// 
        /// <param name="filename">The name of the routing file</param>
        /// 
        private void SetWindowTitle(String filename)
        {
            String title = String.Format("{0}  Autonomous Designer {1}", filename, ProgramVersion);
            Title = title;
        }

        /// <summary>
        /// Handle user request to load a file.
        /// </summary>
        /// 
        /// This function will prompt for a file to load and then perform the 
        /// loading.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LoadFolder(dialog.FileName);
            }
        }

        /// <summary>
        /// Handle user request to save a file
        /// </summary>
        /// 
        /// This function will save the data in memory to a file. 
        /// 
        /// If a filename hasn't been specified before (by a load or
        /// previous save) this function will prompt for the name.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            RouteGroup programList = mProgramModes;
            if (programList != null)
            {
                if (String.IsNullOrEmpty(SaveFilename))
                {
                    SaveAsButton_Click(sender, e);
                }
                else
                {
                    SaveFile(SaveFilename);
                    MessageBox.Show($"Saved to {SaveFilename}");
                }
            }
            Dirty = false;
        }

        /// <summary>
        /// Handle user request to save a file with prompt for filename
        /// </summary>
        /// 
        /// This function will save teh data in memory to a file.
        /// 
        /// This function always prompts for a filename.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Autonomous JSON File(*.txt)|*.txt|Autonomous JSON File (*.json)|*.json|All(*.*)|*.*";
            dlg.DefaultExt = ".txt";
            dlg.Title = "Save an Autonomous File";
            dlg.ShowDialog();

            if (!String.IsNullOrEmpty(dlg.FileName))
            {
                SaveFile(dlg.FileName);
                MessageBox.Show($"Saved to {SaveFilename}");
            }
        }

        private bool LoadFolder(String folderName)
        {
            bool retval = false;
            string paramsPattern = @"@AutonomousModeAnnotation\(parameterNames = {(.*)}\)";
            string commandPattern = @".*\\(.*).java";
            string trajectoryPattern = @".*\\(.*).wpilib";
            Regex paramReg = new Regex(paramsPattern, RegexOptions.IgnoreCase);
            Regex commandReg = new Regex(commandPattern, RegexOptions.IgnoreCase);
            Regex trajectoryReg = new Regex(trajectoryPattern, RegexOptions.IgnoreCase);

            ProgramPnl.ClearCommandSet();
            foreach (var file in
                Directory.EnumerateFiles(folderName + commandDirectory, "*.java"))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    String fileText = sr.ReadToEnd();
                    Match m = paramReg.Match(fileText);
                    if(m.Success)
                    {
                        string commandName = commandReg.Match(file).Groups[1].Value;
                        string paramsText = m.Groups[1].Value.Replace("\"", "").Trim();
                        CommandTemplate template;
                        if (paramsText.Length > 0)
                        {
                            string[] paramsArray = paramsText.Split(',');
                            template = new CommandTemplate(commandName, paramsArray);
                        }
                        else
                        {
                            template = new CommandTemplate(commandName);
                        }   
                        ProgramPnl.UpdateCommandSet(template);
                    }
                }
            }
            List<string> trajectories = new List<string>();
            foreach (var file in
                Directory.EnumerateFiles(folderName + trajectoryDirectory, "*.wpilib.json"))
            {
                string trajectoryName = trajectoryReg.Match(file).Groups[1].Value;
                trajectories.Add(trajectoryName);
            }
            CommandTemplate.Trajectories = trajectories;
            
            retval = LoadFile(folderName + "\\src\\main\\deploy\\amode238.txt");

            Dirty = false;
            return retval;
        }

        /// <summary>
        /// Load a file into memory.
        /// </summary>
        /// 
        /// This function will load a file into memory replacing 
        /// whatever is already loaded.
        /// 
        /// <param name="filename">The name and path of the file to load</param>
        /// <returns>Returns true if the file was successfully loaded,
        /// otherwise false is returned.</returns>
        /// 
        private bool LoadFile(String filename)
        {
            bool retval = false;
            try
            {
                Autonomous_x.RouteGroup programList = null;
                programList = Autonomous_x.RouteGroup.Load(filename);

                if (programList != null)
                {
                    SaveFilename = filename;
                    AddNewRoute(programList);
                }

                retval = true;
            }
            catch (Exception ex)
            {
                String msg = String.Format("Unable to load JSON file\n{0}", ex.Message);
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                retval = false;
            }

            return retval;
        }

        /// <summary>
        /// Create a new empty route and add it to the program list.
        /// </summary>
        /// 
        /// This function sets the program modes to be the provided
        /// program list.
        /// 
        /// <param name="programList">The new program list to use</param>
        /// 
        private void AddNewRoute(Autonomous_x.RouteGroup programList)
        {
            mProgramModes = programList;
            UpdateRouteList();
            ProgramModeLB.SelectedIndex = 0;
        }

        /// <summary>
        /// Updatea the program modes (routes) listbox with the 
        /// current program modes.
        /// </summary>
        /// 
        private void UpdateRouteList()
        {
            ProgramModeLB.ItemsSource = mProgramModes.AutonomousModes;
        }

        /// <summary>
        /// Save the current data to the specified file.
        /// </summary>
        /// 
        /// <param name="filename">The name and path of the file to save to</param>
        /// 
        private void SaveFile(String filename)
        {
            RouteGroup programList = mProgramModes;
            if (programList != null)
            {
                programList.Save(filename);
                SaveFilename = filename;
            }
        }

        /// <summary>
        /// User request to exit the program.
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            if(Dirty)
            {
                String msg = "Are you sure you want to quit without saving?";
                MessageBoxResult result = MessageBox.Show(msg, "Quit?", MessageBoxButton.YesNo);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    Close();
                } else
                {
                    return;
                }
            }

            Close();
        }

        /// <summary>
        /// The user has changed the selection of the auto route.
        /// </summary>
        /// 
        /// This function will update the route steps listbox to display
        /// those belonging to the newly selected route.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ProgramModeLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutonomousRoute selectedMode = SelectedProgram;

            // display the new program mode in the right side panel
            if (selectedMode != null)
            {
                ProgramPnl.ProgramNameLabel = selectedMode.Name;
                ProgramPnl.Commands = selectedMode.Commands;
            }
        }

        /// <summary>
        /// User requested to move current route down by one
        /// </summary>
        /// 
        /// This function moves the current selected route down one space in
        /// the route list.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((ProgramModeLB.SelectedIndex >= 0) && (ProgramModeLB.SelectedIndex < ProgramModeLB.Items.Count - 1))
            {
                int index = ProgramModeLB.SelectedIndex;
                AutonomousRoute item = mProgramModes.AutonomousModes[index];
                mProgramModes.AutonomousModes.RemoveAt(index);
                mProgramModes.AutonomousModes.Insert(index + 1, item);
                ProgramModeLB.SelectedIndex = index + 1;
            }
            Dirty = true;
        }

        /// <summary>
        /// User requested to move current route up by one
        /// </summary>
        /// 
        /// This function moves the current selected route up one space in
        /// the route list.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            // if the index is set and greater than the first element
            // move it up
            if (ProgramModeLB.SelectedIndex > 0)
            {
                int index = ProgramModeLB.SelectedIndex;
                AutonomousRoute item = mProgramModes.AutonomousModes[index];
                mProgramModes.AutonomousModes.RemoveAt(index);
                mProgramModes.AutonomousModes.Insert(index - 1, item);
                ProgramModeLB.SelectedIndex = index - 1;
            }
            Dirty = true;
        }

        /// <summary>
        /// Add a new route to the route list
        /// </summary>
        /// 
        /// Adds a new empty route to the route list. The insertion 
        /// occurs wherever the user has currently selected a route
        /// in the route list. If no route is currently selected 
        /// the new route will be appended.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AutonomousRoute mode = new AutonomousRoute("new");

            if ((ProgramModeLB.SelectedIndex >= 0) && (ProgramModeLB.SelectedIndex < ProgramModeLB.Items.Count))
            {
                mProgramModes.AutonomousModes.Insert(ProgramModeLB.SelectedIndex + 1, mode);
            }
            else
            {
                mProgramModes.AutonomousModes.Add(mode);
            }
            Dirty = true;
        }

        /// <summary>
        /// User has requested the currently selected route should be removed.
        /// </summary>
        /// 
        /// This function will remove and discard the current route from the
        /// route list.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramModeLB.SelectedIndex >= 0)
            {
                mProgramModes.AutonomousModes.RemoveAt(ProgramModeLB.SelectedIndex);
            }
            Dirty = true;
        }

        /// <summary>
        /// User request to rename the route.
        /// </summary>
        /// 
        /// When the user double clicks a route in the route list an
        /// editor is presented allowing them to enter a new name for
        /// the route.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ProgramModeTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBlock displayText = (TextBlock)sender;
                displayText.Visibility = System.Windows.Visibility.Collapsed;

                TextBox editBox = (TextBox)displayText.Tag;
                editBox.Visibility = System.Windows.Visibility.Visible;
                editBox.Text = SelectedProgram.Name;
            }
            Dirty = true;
        }

        /// <summary>
        /// The user has changed the name of the route in the route list
        /// </summary>
        /// 
        /// This function is called when the name of the route changes. This
        /// may be called before the user has completed the change.
        /// 
        /// This function will push the name back into the data store.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ProgramModeEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutonomousRoute mode = SelectedProgram;
            if (mode != null)
            {
                TextBox editBox = sender as TextBox;
                mode.Name = editBox.Text;
                ProgramPnl.ProgramNameLabel = mode.Name;
            }
            Dirty = true;
        }

        /// <summary>
        /// The player has pressed enter in the editor for a route
        /// being renamed.
        /// </summary>
        /// 
        /// This function disables the text editor and restores it to a 
        /// static label. 
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ProgramModeEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox box = (TextBox)sender;
                e.Handled = true;

                TextBlock block = (TextBlock)box.Tag;
                box.Visibility = System.Windows.Visibility.Collapsed;
                block.Visibility = System.Windows.Visibility.Visible;

                block.Text = box.Text;

#if false
                // int selectedIndex = ProgramParametersLB.SelectedIndex;
                int selectedIndex = ProgramModeLB.SelectedIndex;

                try
                {
                    // Parameters[selectedIndex] = box.Text;
                    String tt = box.Text;
                }
                catch (Exception /*ex*/)
                {
                    /* //TODO do something with the error */
                }
#endif
            }
            Dirty = true;
        }

        /// <summary>
        /// The keyboard has lost focus away from an edited route
        /// name.
        /// </summary>
        /// 
        /// When the keyboard loses focus the value in the text editor
        /// is pushed back to the label, the text editor is disabled and
        /// the label is made visible.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ProgramModeEntry_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            e.Handled = true;

            TextBlock block = (TextBlock)box.Tag;
            box.Visibility = System.Windows.Visibility.Collapsed;
            block.Visibility = System.Windows.Visibility.Visible;

            block.Text = box.Text;

            Dirty = true;
        }

        /// <summary>
        /// Called when the window is first opened.
        /// </summary>
        /// 
        /// This function attempts to size and position the window
        /// appropriately on the desktop when it first opens.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TaskDefinitionWindow win = (TaskDefinitionWindow)sender;
            double width = win.Width;
            double height = win.Height;

            double desktopWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double desktopHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            if (width > desktopWidth)
            {
                win.Width = desktopWidth * 0.80;
                win.Left = 20.0;
            }

            if (height > desktopHeight)
            {
                win.Height = desktopHeight * 0.90;
                win.Top = 20.0;
            }
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadWindow dlg = new DownloadWindow();
            dlg.Owner = this;
            dlg.Show();
        }

        private void CloneBtn_Click(object sender, RoutedEventArgs e)
        {
            //get the old one
            //make a copy into a new variable
            //put it in the list

            if (ProgramModeLB.SelectedIndex >= 0)
            {
                var toBeCloned = mProgramModes.AutonomousModes[ProgramModeLB.SelectedIndex];
                var cloned = Utility.Clone(toBeCloned);
                cloned.Name = cloned.Name + " Cloned";
                mProgramModes.AutonomousModes.Insert(ProgramModeLB.SelectedIndex + 1, cloned);
            }
            Dirty = true;
        }
    }
}
