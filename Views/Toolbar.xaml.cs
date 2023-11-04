﻿using Microsoft.Win32;
using Painter.Models;
using Painter.ViewModels;
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


        DrawingWinViewModel dwModel = DrawingWinViewModel.Instance;
        ToolbarViewModel toolModel = new ToolbarViewModel(); 

        public Toolbar()
        {
            InitializeComponent();
            Start();
        }

        void Start(bool loadSettings = true)
        {

            DataContext = new
            {
                drawData = dwModel,
                toolData = toolModel
            };
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



        private void rbMode_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton obj = sender as RadioButton;

            Console.WriteLine("{0}, {1}", obj.Name, obj.IsChecked);


            switch (obj.Name) {
                case "Mode_Draw":
                    ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeMode(EditModeType.Draw);
                    break;
                case "Mode_Erase":
                    ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeMode(EditModeType.Erase);
                    break;
                case "Mode_Select":
                    ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeMode(EditModeType.Select);
                    break;
                default:
                    break;
            }

        }

        private void rbShape_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton obj = sender as RadioButton;

            Console.WriteLine("{0}, {1}", obj.Name, obj.IsChecked);


            switch (obj.Name)
            {
                case "Shape_Ellipse":
                    ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeMode(EditModeType.Shape_Ellipse);
                    break;
                case "Shape_Rectangle":
                    ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeMode(EditModeType.Shape_Rect);
                    break;
                case "Shape_Triangle":
                    ((DrawingWin)System.Windows.Application.Current.MainWindow).ChangeMode(EditModeType.Shape_Triangle);
                    break;
                default:
                    break;
            }
        }


    }
}
