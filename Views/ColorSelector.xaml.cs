﻿using Painter.ViewModels;
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

        DrawingWinViewModel dwModel = DrawingWinViewModel.Instance;
        ColorSelectorViewModel colModel = new ColorSelectorViewModel();
        public ColorSelector()
        {
            InitializeComponent();
            tbLineColor.IsChecked = true;
            tbShapeColor.IsChecked = false;

            DataContext = new
            {
                drawModel = dwModel,
                colorModel = colModel,
            };

            lbColors.ItemsSource = colModel.ColorList;
        }


        private void lbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine((sender as ListBox).SelectedValue);

            var currTab = tbLineColor.IsChecked == true ? tbLineColor : tbShapeColor;

            if (currTab == tbLineColor)
            {
                dwModel.StrokeColor = ((sender as ListBox).SelectedValue) as string;
                ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeSelectedStrokeColorIfNecessary();
            }
            else {
                dwModel.ShapeFillColor = ((sender as ListBox).SelectedValue) as string;
                ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeSelectedShapeFillColorIfNecessary();
            }

        }




        private void toggleStatusChanged(object sender, RoutedEventArgs e)
        {
            ToggleButton obj = sender as ToggleButton;

            if (obj.Name == "tbLineColor")
            {
                tbLineColor.IsChecked = obj.IsChecked;
                tbShapeColor.IsChecked = !obj.IsChecked;

            }
            else if(obj.Name == "tbShapeColor") {
                tbShapeColor.IsChecked = obj.IsChecked;
                tbLineColor.IsChecked = !obj.IsChecked;
            }

        }
    }
}
