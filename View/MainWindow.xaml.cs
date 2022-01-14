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
            
            fileWorker.ReadFromFile(filePath);
            FileCode.Text = fileWorker.combineFileParts();

            ShowUsedProgramGroups();
        }

        private void ShowUsedProgramGroups()
        {
            UsedGroups.Text = "";
            foreach (string group in fileWorker.UsedGroupsList)
            {
              UsedGroups.Text += group;
            }
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
            AddGroupsToPositions();
            ShowUsedProgramGroups();
            FileCode.Text = fileWorker.combineFileParts();
        }


        public void AddGroupsToPositions()
        {
                List<RobotGroup> robotGroups = GetGroupsList();
                fileWorker.setFanucLSFilePositions(robotGroups);
        }

        public List<RobotGroup> GetGroupsList()
        {

            List<RobotGroup> robotGroups = new List<RobotGroup>();

            int groupsCount = Groups.Items.Count;
            
            if(groupsCount == 0)
            {
                return robotGroups;
            }

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
            
        }


        private void AddGroupPanel(object sender, RoutedEventArgs e)
        {
            Groups.Items.Add(new GroupPanel());
        }

    }
}