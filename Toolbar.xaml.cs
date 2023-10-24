using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Painter
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {




        public Toolbar()
        {
            InitializeComponent();
            Start();
        }

        void Start(bool loadSettings = true)
        {

            List<string> cols = new List<string>() { "Black" , "LightSlateGray" , "DarkRed","Red","DarkOrange","Yellow",
                "Green","DeepSkyBlue","RoyalBlue","MediumOrchid","White","LightGray","RosyBrown","Pink","Orange","Beige",
                "YellowGreen","PaleTurquoise","SteelBlue","Lavender"};
            lbColors.ItemsSource = cols;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            // remove toolbar overflow
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void paletteImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {




        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "isf files (*.isf)|*.isf";
            sfd.ShowDialog();
            ((DrawingWin)System.Windows.Application.Current.MainWindow).Save(sfd.FileName);
        }



        private void OpenFile_Click(object sender, RoutedEventArgs e) 
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "isf files (*.isf)|*.isf";
            ofd.ShowDialog();

            ((DrawingWin)System.Windows.Application.Current.MainWindow).LoadFile(ofd.FileName); 
        }

        private void rbMode_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton obj = sender as RadioButton;

            Console.WriteLine("{0}, {1}", obj.Name, obj.IsChecked);

        }

        private void rbShape_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton obj = sender as RadioButton;

            Console.WriteLine("{0}, {1}", obj.Name, obj.IsChecked);
        }

        private void bRedo_Click(object sender, RoutedEventArgs e)
        {
            ((DrawingWin)System.Windows.Application.Current.MainWindow).Redo();

        }

        private void bUndo_Click(object sender, RoutedEventArgs e)
        {
            ((DrawingWin)System.Windows.Application.Current.MainWindow).Undo();

        }
    }
}
