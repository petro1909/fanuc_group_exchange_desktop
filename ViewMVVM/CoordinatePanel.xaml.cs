using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using fanuc_group_exchange_desktop.ViewModel;

namespace fanuc_group_exchange_desktop.ViewMVVM
{
    /// <summary>
    /// Interaction logic for CoordinatePanel.xaml
    /// </summary>
    public partial class CoordinatePanel : UserControl
    {
        public CoordinatePanel()
        {
            InitializeComponent();
            DataContext = new CoordinateViewModel();
        }

        public void DeleteCoordinateBlock(object sender, RoutedEventArgs e)
        {
            ListBox coordinateBlockList = (ListBox)this.Parent;
            
            string coordinateNumber = this.CoordinateNumber.Text;

            coordinateBlockList.Items.Remove(this);
            
            Regex regex = new Regex("\\d+");
            int CoordinateNumber = int.Parse(regex.Match(coordinateNumber).Value);

            for(int i = CoordinateNumber - 1; i < coordinateBlockList.Items.Count; i++)
            {
                CoordinatePanel s = coordinateBlockList.Items[i] as CoordinatePanel;
                s.CoordinateNumber.Text = $"J{i + 1} =";
            }
        }
    }
}