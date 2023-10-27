using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Painter.Views
{
    /// <summary>
    /// Interaction logic for ColorSelector.xaml
    /// </summary>
    public partial class ColorSelector : UserControl
    {
        public ColorSelector()
        {
            InitializeComponent();
            tbLineColor.IsChecked = true;
            tbShapeColor.IsChecked = false;

            List<string> cols = new List<string>() { 
                "Black" , 
                "LightSlateGray" , 
                "DarkRed",
                "Red",
                "DarkOrange",
                "Yellow",
                "Green",
                "DeepSkyBlue",
                "RoyalBlue",
                "MediumOrchid",
                "White",
                "LightGray",
                "RosyBrown",
                "Pink",
                "Orange",
                "Beige",
                "YellowGreen",
                "PaleTurquoise",
                "SteelBlue",
                "Lavender"};
            lbColors.ItemsSource = cols;
        }

        private void tbShapeColor_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton obj = sender as ToggleButton;
            //Console.WriteLine(obj.IsChecked);

            tbLineColor.IsChecked = !obj.IsChecked;
        }

        private void tbLineColor_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton obj = sender as ToggleButton;
            //Console.WriteLine(obj.IsChecked);

            tbShapeColor.IsChecked = !obj.IsChecked;
        }
    }
}
