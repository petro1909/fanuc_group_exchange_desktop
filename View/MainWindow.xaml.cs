using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.IO;
using fanuc_group_exchange_desktop.Model;
using System.Globalization;

namespace fanuc_group_exchange_desktop.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = "";
        

        FileWorker fileWorker;
        GroupManipulator groupManipulator;

        public MainWindow()
        {
            fileWorker = new FileWorker();
            groupManipulator = new GroupManipulator();
            
            InitializeComponent();
            Groups.Items.Add(new GroupPanel());

            FileCode.Text = null;
        }

        private void GetFileCode(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.HasValue) return;

            filePath = dialog.FileName;
            UsedGroups.Children.Clear();

            fileWorker.ReadFromFile(filePath);
            FileCode.Text = fileWorker.combineFileParts();

            ShowUsedProgramGroups();
        }

        private void ShowUsedProgramGroups()
        {
            List<string> groupList = fileWorker.UsedGroupsList;

            for (int i = 0; i < groupList.Count; i++)
            {
                if(groupList[i] == "1")
                {
                    StackPanel stackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    TextBlock groupNumber = new TextBlock()
                    {
                        Height = 25,
                        Text = "Group" + (i + 1).ToString(),
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    Button delete = new Button
                    {
                        Tag = (i + 1).ToString(),
                        Height = 25,
                        Content = "delete",
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    delete.Click += DeleteGroup;
                    stackPanel.Children.Add(groupNumber);
                    stackPanel.Children.Add(delete);
                    UsedGroups.Children.Add(stackPanel);
                }   
            }
        }

        public void DeleteGroup(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int groupNumber = int.Parse(button.Tag.ToString());
            fileWorker.deleteGroup(groupNumber);
            FileCode.Text = fileWorker.combineFileParts();
            StackPanel stack = button.Parent as StackPanel;
            UsedGroups.Children.Remove(stack);


        }


        public void SaveFile(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileCode.Text)) return;
            
            string file = fileWorker.combineFileParts();
            fileWorker.WriteToFile(filePath, file);
        }

        public void SaveFileAs(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileCode.Text)) return;
            
            
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Файлы программ (*.LS)|*.ls";
            if (dialog.ShowDialog() == DialogResult.HasValue) return;

            string fileName = dialog.SafeFileName;
            fileWorker.FileName = fileName.Substring(0, fileName.LastIndexOf("."));

            string file = fileWorker.combineFileParts();
            fileWorker.WriteToFile(dialog.FileName, file);
        }


        public void SaveGroup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileCode.Text)) return;
            if(AddGroupsToPositions()==default) return;
                ShowUsedProgramGroups();
                FileCode.Text = fileWorker.combineFileParts();
            
        }


        public int AddGroupsToPositions()
        {
            if (string.IsNullOrEmpty(FileCode.Text)) return default;
            List<RobotGroup> robotGroups = GetGroupsList();
            if (robotGroups != null) 
            {
                fileWorker.setFanucLSFilePositions(robotGroups);
                return 1;
            }
            return default;
        }

        public List<RobotGroup> GetGroupsList()
        {
            List<RobotGroup> robotGroups = new List<RobotGroup>();
            int groupsCount = Groups.Items.Count;
            if(groupsCount == 0) return default;

            try
            {
                for (int i = 0; i < groupsCount; i++)
                {
                    GroupPanel groupPanel = (GroupPanel)Groups.Items[i];

                    //get group elements
                    string groupNumberStr = groupPanel.GroupNumber.Text;
                    ListBox groupCoordinates = groupPanel.GroupsCoordinates;


                    int groupNumber = int.Parse(groupNumberStr);
                    int groupUserFrame = int.Parse(groupPanel.UserFrame.Text);
                    int groupUserTool = int.Parse(groupPanel.UserTool.Text);

                    List<Coordinate> coordinates = new List<Coordinate>();

                    ItemCollection positionsStackPanel = groupCoordinates.Items;

                    for (int j = 0; j < positionsStackPanel.Count; j++)
                    {
                        //get coorinate elements
                        CoordinatePanel coordinatePanel = (CoordinatePanel)positionsStackPanel[j];

                        int coordinateNumber = j + 1;

                        string coordinatePositionString = coordinatePanel.GroupPosition.Text;

                        IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                        double coordinatePosition = double.Parse(coordinatePositionString, formatter);

                        string unit = coordinatePanel.Units.Text;

                        coordinates.Add(new Coordinate(coordinateNumber, coordinatePosition, unit));
                    }

                    RobotGroup robotGroup = new RobotNotFirstGroup(groupNumber, groupUserFrame, groupUserTool, coordinates);
                    robotGroups.Add(robotGroup);
                }
                return robotGroups;
            } catch(Exception e)
            {
                return default;
            }
            
        }


        private void AddGroupPanel(object sender, RoutedEventArgs e)
        {
            Groups.Items.Add(new GroupPanel());
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}