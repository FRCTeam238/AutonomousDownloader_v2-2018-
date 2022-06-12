using Autonomous_Downloader.Autonomous_x;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Autonomous_Downloader
{
    /// <summary>
    /// Interaction logic for PlayDefinitionWindow.xaml
    /// </summary>
    public partial class PlayDefinitionWindow : Window
    {
        private PlayGroup mPlays = null;

        // this value is set after the ListBox selection changed has been processed
        // prefer the use of the SelectedPlay property
        private AutonomousPlay mCurrentPlaySelected = null;

        private string mSaveFilename;

        private string SaveFilename
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

        private string L_LL
        {
            get
            {
                return SelectedPlay.L_LL[0];
            }

            set
            {
                SelectedPlay.L_LL[0] = value;
            }
        }

        private AutonomousPlay SelectedPlay
        {
            get
            {
                if (PlaysLB.SelectedItem != null)
                {
                    return (AutonomousPlay)PlaysLB.SelectedItem;
                }
                else
                {
                    return null;
                }
            }
        }

        public PlayDefinitionWindow()
        {
            InitializeComponent();
            
            PutPlayToControls(null);

            InitializeCommandsList();
            InitializeComboBoxes();

            PlayGroup playsList = new PlayGroup();
            AutonomousPlay newPlay = new AutonomousPlay("new");
            newPlay.L_LL[0] = "L_LL";
            newPlay.L_LR[0] = "L_LR";
            newPlay.L_RR[0] = "L_RR";
            newPlay.L_RL[0] = "L_RL";

            newPlay.C_LL[0] = "C_LL";
            newPlay.C_LR[0] = "C_LR";
            newPlay.C_RR[0] = "C_RR";
            newPlay.C_RL[0] = "C_RL";

            newPlay.R_LL[0] = "R_LL";
            newPlay.R_LR[0] = "R_LR";
            newPlay.R_RR[0] = "R_RR";
            newPlay.R_RL[0] = "R_RL";

            playsList.AutonomousPlays.Add(newPlay);
            AddNewRoute(playsList);
        }

        private void InitializeComboBoxes()
        {
            ObservableCollection<string> list;

            try
            {
                list = LoadTasksForDropDowns();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                list = new ObservableCollection<string>();
            }

            Play_L_LL_CB.ItemsSource = list;
            Play_C_LL_CB.ItemsSource = list;
            Play_R_LL_CB.ItemsSource = list;

            Play_L_LR_CB.ItemsSource = list;
            Play_C_LR_CB.ItemsSource = list;
            Play_R_LR_CB.ItemsSource = list;

            Play_L_RR_CB.ItemsSource = list;
            Play_C_RR_CB.ItemsSource = list;
            Play_R_RR_CB.ItemsSource = list;

            Play_L_RL_CB.ItemsSource = list;
            Play_C_RL_CB.ItemsSource = list;
            Play_R_RL_CB.ItemsSource = list;
        }

        private ObservableCollection<string> LoadTasksForDropDowns()
        {
            ObservableCollection<string> tasks = LoadTasksForDropDowns(".\\amode238.txt");
            return tasks;
        }

        private ObservableCollection<string> LoadTasksForDropDowns(string filepath)
        {
            RouteGroup tasks = RouteGroup.Load(filepath);

            ObservableCollection<string> taskNames = new ObservableCollection<string>();

            foreach (AutonomousRoute task in tasks.AutonomousModes)
            {
                taskNames.Add(task.Name);
            }

            return taskNames;
        }

        private void AddNewRoute(Autonomous_x.PlayGroup playsList)
        {
            mPlays = playsList;
            UpdatePlayList();
            //ProgramModeLB.SelectedIndex = 0;
        }

        private void UpdatePlayList()
        {
            PlaysLB.ItemsSource = mPlays.AutonomousPlays;
        }

        private void AddNewPlay(Autonomous_x.PlayGroup playsList)
        {
            mPlays = playsList;
            UpdatePlayList();
            PlaysLB.SelectedIndex = 0;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AutonomousPlay mode = new AutonomousPlay("new");

            if ((PlaysLB.SelectedIndex >= 0) && (PlaysLB.SelectedIndex < PlaysLB.Items.Count))
            {
                mPlays.AutonomousPlays.Insert(PlaysLB.SelectedIndex + 1, mode);
            }
            else
            {
                mPlays.AutonomousPlays.Add(mode);
            }

        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PlaysLB.SelectedIndex >= 0)
            {
                mPlays.AutonomousPlays.RemoveAt(PlaysLB.SelectedIndex);
            }
        }

        private void PlayTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBlock displayText = (TextBlock)sender;
                displayText.Visibility = System.Windows.Visibility.Collapsed;

                TextBox editBox = (TextBox)displayText.Tag;
                editBox.Visibility = System.Windows.Visibility.Visible;
                editBox.Text = SelectedPlay.Name;
            }
        }

        private void PlayEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutonomousPlay mode = SelectedPlay;
            if (mode != null)
            {
                TextBox editBox = sender as TextBox;
                mode.Name = editBox.Text;
                //TODO ProgramPnl.ProgramNameLabel = mode.Name;
            }
        }

        private void PlayEntry_KeyDown(object sender, KeyEventArgs e)
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
        }

        private void PlayEntry_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            e.Handled = true;

            TextBlock block = (TextBlock)box.Tag;
            box.Visibility = System.Windows.Visibility.Collapsed;
            block.Visibility = System.Windows.Visibility.Visible;

            block.Text = box.Text;
        }

        /// <summary>
        /// Called by the PlaysLB when the selection has been changed
        /// </summary>
        /// 
        /// When called this function will unload the previous selection and load the
        /// new one. Primarily this will be loading the new play's selection onto the
        /// dashboard.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaysLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutonomousPlay selectedPlay = SelectedPlay;

            // display the new program mode in the right side panel
            if (selectedPlay != null)
            {
                PutPlayToControls(selectedPlay);
                mCurrentPlaySelected = selectedPlay;
                //PlayGroupingGrid.DataContext = selectedMode;
                //TODO ProgramPnl.ProgramNameLabel = selectedMode.Name;
                //TODO ProgramPnl.Commands = selectedMode.Commands;
            }
        }

        private void PutPlayToControls(AutonomousPlay selectedPlay)
        {
            if (selectedPlay != null)
            {
                Play_L_LL_CB.IsEnabled = true;
                Play_L_LL_CB.Text = selectedPlay.L_LL[0];
                Play_C_LL_CB.IsEnabled = true;
                Play_C_LL_CB.Text = selectedPlay.C_LL[0];
                Play_R_LL_CB.IsEnabled = true;
                Play_R_LL_CB.Text = selectedPlay.R_LL[0];

                Play_L_LR_CB.IsEnabled = true;
                Play_L_LR_CB.Text = selectedPlay.L_LR[0];
                Play_C_LR_CB.IsEnabled = true;
                Play_C_LR_CB.Text = selectedPlay.C_LR[0];
                Play_R_LR_CB.IsEnabled = true;
                Play_R_LR_CB.Text = selectedPlay.R_LR[0];

                Play_L_RR_CB.IsEnabled = true;
                Play_L_RR_CB.Text = selectedPlay.L_RR[0];
                Play_C_RR_CB.IsEnabled = true;
                Play_C_RR_CB.Text = selectedPlay.C_RR[0];
                Play_R_RR_CB.IsEnabled = true;
                Play_R_RR_CB.Text = selectedPlay.R_RR[0];

                Play_L_RL_CB.IsEnabled = true;
                Play_L_RL_CB.Text = selectedPlay.L_RL[0];
                Play_C_RL_CB.IsEnabled = true;
                Play_C_RL_CB.Text = selectedPlay.C_RL[0];
                Play_R_RL_CB.IsEnabled = true;
                Play_R_RL_CB.Text = selectedPlay.R_RL[0];
            }
            else
            {
                Play_L_LL_CB.IsEnabled = false;
                Play_C_LL_CB.IsEnabled = false;
                Play_R_LL_CB.IsEnabled = false;

                Play_L_LR_CB.IsEnabled = false;
                Play_C_LR_CB.IsEnabled = false;
                Play_R_LR_CB.IsEnabled = false;

                Play_L_RR_CB.IsEnabled = false;
                Play_C_RR_CB.IsEnabled = false;
                Play_R_RR_CB.IsEnabled = false;

                Play_L_RL_CB.IsEnabled = false;
                Play_C_RL_CB.IsEnabled = false;
                Play_R_RL_CB.IsEnabled = false;
            }
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            // if the index is set and greater than the first element
            // move it up
            if (PlaysLB.SelectedIndex > 0)
            {
                int index = PlaysLB.SelectedIndex;
                AutonomousPlay item = mPlays.AutonomousPlays[index];
                mPlays.AutonomousPlays.RemoveAt(index);
                mPlays.AutonomousPlays.Insert(index - 1, item);
                PlaysLB.SelectedIndex = index - 1;
            }

        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((PlaysLB.SelectedIndex >= 0) && (PlaysLB.SelectedIndex < PlaysLB.Items.Count - 1))
            {
                int index = PlaysLB.SelectedIndex;
                AutonomousPlay item = mPlays.AutonomousPlays[index];
                mPlays.AutonomousPlays.RemoveAt(index);
                mPlays.AutonomousPlays.Insert(index + 1, item);
                PlaysLB.SelectedIndex = index + 1;
            }
        }

        private void TasksBtn_Click(object sender, RoutedEventArgs e)
        {
            Window ww = new TaskDefinitionWindow();
            ww.ShowDialog();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Autonomous JSON File(*.txt)|*.txt|Autonomous JSON File (*.json)|*.json|All(*.*)|*.*";
            dlg.DefaultExt = ".txt";
            dlg.Title = "Open an Autonomous File";
            dlg.ShowDialog();

            if (!String.IsNullOrEmpty(dlg.FileName))
            {
                LoadFile(dlg.FileName);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            PlayGroup playList = mPlays;
            if (playList != null)
            {
                if (String.IsNullOrEmpty(SaveFilename))
                {
                    SaveAsButton_Click(sender, e);
                }
                else
                {
                    SaveFile(SaveFilename);
                }
            }
        }

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
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Play_N_CB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == Play_L_LL_CB)
            {
                SelectedPlay.L_LL[0] = Play_L_LL_CB.Text;
            }
            else if (sender == Play_C_LL_CB)
            {
                SelectedPlay.C_LL[0] = Play_C_LL_CB.Text;
            }
            else if (sender == Play_R_LL_CB)
            {
                SelectedPlay.R_LL[0] = Play_R_LL_CB.Text;
            }
            else if (sender == Play_L_LR_CB)
            {
                SelectedPlay.L_LR[0] = Play_L_LR_CB.Text;
            }
            else if (sender == Play_C_LR_CB)
            {
                SelectedPlay.C_LR[0] = Play_C_LR_CB.Text;
            }
            else if (sender == Play_R_LR_CB)
            {
                SelectedPlay.R_LR[0] = Play_R_LR_CB.Text;
            }
            else if (sender == Play_L_RR_CB)
            {
                SelectedPlay.L_RR[0] = Play_L_RR_CB.Text;
            }
            else if (sender == Play_C_RR_CB)
            {
                SelectedPlay.C_RR[0] = Play_C_RR_CB.Text;
            }
            else if (sender == Play_R_RR_CB)
            {
                SelectedPlay.R_RR[0] = Play_R_RR_CB.Text;
            }
            else if (sender == Play_L_RL_CB)
            {
                SelectedPlay.L_RL[0] = Play_L_RL_CB.Text;
            }
            else if (sender == Play_C_RL_CB)
            {
                SelectedPlay.C_RL[0] = Play_C_RL_CB.Text;
            }
            else if (sender == Play_R_RL_CB)
            {
                SelectedPlay.R_RL[0] = Play_R_RL_CB.Text;
            }
        }

        private void Play_N_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == Play_L_LL_CB)
            {
                string text = (string)Play_L_LL_CB.SelectedValue;
                SelectedPlay.L_LL[0] = text;
            }
            else if (sender == Play_C_LL_CB)
            {
                string text = (string)Play_C_LL_CB.SelectedValue;
                SelectedPlay.C_LL[0] = text;
            }
            else if (sender == Play_R_LL_CB)
            {
                string text = (string)Play_R_LL_CB.SelectedValue;
                SelectedPlay.R_LL[0] = text;
            }
            else if (sender == Play_L_LR_CB)
            {
                string text = (string)Play_L_LR_CB.SelectedValue;
                SelectedPlay.L_LR[0] = text;
            }
            else if (sender == Play_C_LR_CB)
            {
                string text = (string)Play_C_LR_CB.SelectedValue;
                SelectedPlay.C_LR[0] = text;
            }
            else if (sender == Play_R_LR_CB)
            {
                string text = (string)Play_R_LR_CB.SelectedValue;
                SelectedPlay.R_LR[0] = text;
            }
            else if (sender == Play_L_RR_CB)
            {
                string text = (string)Play_L_RR_CB.SelectedValue;
                SelectedPlay.L_RR[0] = text;
            }
            else if (sender == Play_C_RR_CB)
            {
                string text = (string)Play_C_RR_CB.SelectedValue;
                SelectedPlay.C_RR[0] = text;
            }
            else if (sender == Play_R_RR_CB)
            {
                string text = (string)Play_R_RR_CB.SelectedValue;
                SelectedPlay.R_RR[0] = text;
            }
            else if (sender == Play_L_RL_CB)
            {
                string text = (string)Play_L_RL_CB.SelectedValue;
                SelectedPlay.L_RL[0] = text;
            }
            else if (sender == Play_C_RL_CB)
            {
                string text = (string)Play_C_RL_CB.SelectedValue;
                SelectedPlay.C_RL[0] = text;
            }
            else if (sender == Play_R_RL_CB)
            {
                string text = (string)Play_R_RL_CB.SelectedValue;
                SelectedPlay.R_RL[0] = text;
            }
        }

        /// <summary>
        /// Save the current data to the specified file.
        /// </summary>
        /// 
        /// <param name="filename">The name and path of the file to save to</param>
        /// 
        private void SaveFile(String filename)
        {
            PlayGroup playList = mPlays;
            if (playList != null)
            {
                playList.Save(filename);
                SaveFilename = filename;
            }
        }

        private bool LoadFile(String filename)
        {
            bool retval = false;
            try
            {
                Autonomous_x.PlayGroup playList = null;
                playList = Autonomous_x.PlayGroup.Load(filename);

                if (playList != null)
                {
                    SaveFilename = filename;
                    AddNewPlay(playList);
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

        private void SetWindowTitle(String filename)
        {
            String title = String.Format("{0}  Autonomous Downloader {1}", filename, TaskDefinitionWindow.ProgramVersion);
            Title = title;
        }

        /// <summary>
        /// The current set of commands being shown in the commands panel.
        /// </summary>
        private List<CommandTemplate> CommandSet = null;

        private bool LoadCommandSet(String filepath)
        {
            bool retval = false;

            try
            {
                CommandSet = CommandTemplate.LoadCommandSet(filepath);
                retval = true;
            }
            catch (Exception ex)
            {
                //don't show this error when xaml editor is trying to render or debugging
                if (!DesignerProperties.GetIsInDesignMode(this)){
                    String msg = String.Format("Unable to load file {0}\n", filepath);
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return retval;
        }

        private void InitializeCommandsList()
        {
            if (!LoadCommandSet("commands.json"))
            {
            }

            CommandTemplate.CommandSet = CommandSet;
        }

        private void SaveCommandSet(String filepath)
        {
            CommandTemplate.SaveCommandSet(CommandSet, filepath);
        }
    }
}
