using Painter.ViewModels;
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
    /// Interaction logic for StrokeSelector.xaml
    /// </summary>
    public partial class StrokeSelector : UserControl
    {

        StrokeSelectorViewModel strokeModel = new StrokeSelectorViewModel();
        DrawingWinViewModel dwModel = DrawingWinViewModel.Instance;
        public StrokeSelector()
        {
            InitializeComponent();

            DataContext = new
            {
                skModel = strokeModel,
                dwModel = dwModel,
            };
        }

        //private void tbShapeBorderSize_Checked(object sender, RoutedEventArgs e)
        //{

        //    strokeModel.setupToggleButtonStatus(false);


        //}

        //private void tbStrokeSize_Checked(object sender, RoutedEventArgs e)
        //{

        //    strokeModel.setupToggleButtonStatus(true);
        //}

        private void toggleStatusChanged(object sender, RoutedEventArgs e)
        {
            ToggleButton obj = sender as ToggleButton;

            if (obj.Name == "tbStrokeSize")
            {
                tbStrokeSize.IsChecked = obj.IsChecked;
                tbShapeBorderSize.IsChecked = !obj.IsChecked;
                
            }
            else if (obj.Name == "tbShapeBorderSize")
            {
                tbShapeBorderSize.IsChecked = obj.IsChecked;
                tbStrokeSize.IsChecked = !obj.IsChecked;

            }
            bool Onstroke = tbStrokeSize.IsChecked ?? false;
            

            strokeModel.setupToggleButtonStatus(Onstroke);

            if (Onstroke)
            {
                ValueSlider.Value = dwModel.DrawingAttributesInkCanvas.Height;
            }
            else {
                ValueSlider.Value = dwModel.ShapeStrokeThickness;
            }
        }



        private void ValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine((sender as Slider).Value);

            Console.WriteLine("OnStroke? " + strokeModel.IsOnStrokeSize);


            if (strokeModel.IsOnStrokeSize)
            {
                dwModel.DrawingAttributesInkCanvas.Width = (sender as Slider).Value;
                dwModel.DrawingAttributesInkCanvas.Height = (sender as Slider).Value;

            }
            else { 
                
                dwModel.ShapeStrokeThickness = (sender as Slider).Value;
            }
        }
    }
}
