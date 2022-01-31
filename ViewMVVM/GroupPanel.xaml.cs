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

namespace fanuc_group_exchange_desktop.ViewMVVM
{
    /// <summary>
    /// Interaction logic for GroupPanel.xaml
    /// </summary>
    public partial class GroupPanel : UserControl
    {
        public static ListBox listBox;
        public StackPanel coordinates;

        public GroupPanel()
        {

            InitializeComponent();
            //DataContext = new GroupPanel();
            //GroupsCoordinates.Items.Add(new CoordinatePanel());
        }


        public void AddCoordinateBlockFromButton(object sender, RoutedEventArgs e)
        {
            int coordinateNumber = GroupsCoordinates.Items.Count;

            CoordinatePanel coordinatePanel = new CoordinatePanel();

            coordinatePanel.CoordinateNumber.Text = "J" + (coordinateNumber + 1).ToString() + " =";
            GroupsCoordinates.Items.Add(coordinatePanel);
        }

        
        private void GroupNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DeleteGroupsBlock(object sender, RoutedEventArgs e)
        {
            ListBox groupBlockList = (ListBox)this.Parent;
            groupBlockList.Items.Remove(this);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            ListBox l = this.Parent as ListBox;
            l.Background = Brushes.Black;
        }
    }
}
