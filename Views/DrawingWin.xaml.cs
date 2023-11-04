﻿using Painter.Models;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Painter.ViewModels;


namespace Painter
{
    /// <summary>
    /// Interaction logic for DrawingWin.xaml
    /// </summary>
    public partial class DrawingWin : Window
    {

        private Point iniP;

        private bool isCreatingShape = false;
        private UIElement currentShape;
        

        DrawingWinViewModel model = DrawingWinViewModel.Instance;
        InkPresenter inkPresenter;

        public DrawingWin()
        {
            InitializeComponent();

            inkc.Strokes.StrokesChanged += Strokes_StrokesChanged;
            //inkc.EraserShape = new EllipseStylusShape(20, 20);
            inkc.EraserShape = new RectangleStylusShape(20, 20);
            DataContext = model;

            model.DrawingAttributesInkCanvas.FitToCurve = true;
            
        }



        private void Strokes_StrokesChanged(object sender, System.Windows.Ink.StrokeCollectionChangedEventArgs e)
        {

        }


        public void CreateNew() {
            // ChangeMode(EditModeType.Draw);
            model.CurrentPaintingMode = EditModeType.Draw;
            inkc.Strokes.Clear();
            inkc.Children.Clear();
        }

        public void Save(string Filename)
        {
           
            if(Filename == "")
            {
                Console.WriteLine("Empty Filename");
                return;
            }

            try
            {
                List<ShapeObject> childrenObjects = new List<ShapeObject>();

                foreach (UIElement child in inkc.Children)
                {
                    Console.WriteLine(child.ToString());
                    if (child is Rectangle rect)
                    {
                        var parent = rect.Parent as UIElement;
                        var location = rect.TranslatePoint(new Point(0, 0), parent);


                        childrenObjects.Add(new ShapeObject()
                        {
                            ShapeType = "Rectangle",
                            Fill = rect.Fill,
                            Width = rect.Width,
                            Height = rect.Height,
                            Stroke = rect.Stroke,
                            StrokeThickness = rect.StrokeThickness,
                            StartPoint = location

                        });


                    }
                    else if (child is Polygon polygon)
                    {
                        var parent = polygon.Parent as UIElement;
                        var location = polygon.TranslatePoint(new Point(0, 0), parent);


                        childrenObjects.Add(new ShapeObject()
                        {
                            ShapeType = "Polygon",
                            Fill = polygon.Fill,
                            Stroke = polygon.Stroke,
                            StrokeThickness = polygon.StrokeThickness,
                            Points = polygon.Points,
                            StartPoint = location

                        });


                    }
                    else if (child is Ellipse ellipse)
                    {
                        var parent = ellipse.Parent as UIElement;
                        var location = ellipse.TranslatePoint(new Point(0, 0), parent);

                        childrenObjects.Add(new ShapeObject()
                        {
                            ShapeType = "Ellipse",
                            Fill = ellipse.Fill,
                            Width = ellipse.Width,
                            Height = ellipse.Height,
                            Stroke = ellipse.Stroke,
                            StrokeThickness = ellipse.StrokeThickness,
                            StartPoint = location

                        });

                    }



                }

                SaveObjects currentSaved = new SaveObjects();
                currentSaved.inkStrokeData = inkc.Strokes;
                currentSaved.shapesData = childrenObjects;

            
                string inkcDataStr = JsonConvert.SerializeObject(currentSaved);

                System.IO.File.WriteAllText(Filename, inkcDataStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }

        }

        public void LoadFile(string Filename) {
            try
            {
                
                using (StreamReader file = File.OpenText(Filename))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    CreateNew();

                    JObject o2 = (JObject)JToken.ReadFrom(reader);

                    var savedInks = o2["inkStrokeData"];

                    foreach (JObject strokeObject in savedInks)
                    {

                        var attribute = strokeObject["DrawingAttributes"];

                        var aColor = attribute["Color"].ToObject<Brush>();
                        var aStylusTip = attribute["StylusTip"].ToObject<int>();
                        var aWidth = attribute["Width"].ToObject<double>();
                        var aHeight = attribute["Height"].ToObject<double>();
                        var aFitToCurve = attribute["FitToCurve"].ToObject<Boolean>();
                        var aIgnorePressure = attribute["IgnorePressure"].ToObject<Boolean>();
                        var aIsHighlighter = attribute["IsHighlighter"].ToObject<Boolean>();
                        var aStylusTipTransform = attribute["StylusTipTransform"].ToObject<String>();

                        var sDrawAttribute = new DrawingAttributes();
                        sDrawAttribute.Color = ((SolidColorBrush)aColor).Color;
                        sDrawAttribute.IsHighlighter = aIsHighlighter;
                        sDrawAttribute.IgnorePressure = aIgnorePressure;
                        sDrawAttribute.FitToCurve = aFitToCurve;
                        sDrawAttribute.StylusTip = aStylusTip == 0 ? StylusTip.Rectangle : StylusTip.Ellipse;
                        sDrawAttribute.Height = aWidth;
                        sDrawAttribute.Width = aHeight;

                        var stylusPoints = strokeObject["StylusPoints"].ToObject<StylusPoint[]>();
                        StylusPointCollection points = new StylusPointCollection(stylusPoints);
                        Stroke newStroke = new Stroke(points, sDrawAttribute);
                        inkc.Strokes.Add(newStroke);

                    }

                    var savedShapes = o2["shapesData"].ToObject<ShapeObject[]>();

                    foreach (var shape in savedShapes)
                    {

                        switch (shape.ShapeType)
                        {
                            case "Rectangle":
                                Rectangle rectangle = new Rectangle
                                {
                                    Fill = shape.Fill,
                                    Stroke = shape.Stroke,
                                    StrokeThickness = shape.StrokeThickness,
                                    Width = shape.Width,
                                    Height = shape.Height
                                };



                                // Set the position of the rectangle
                                InkCanvas.SetLeft(rectangle, shape.StartPoint.X);
                                InkCanvas.SetTop(rectangle, shape.StartPoint.Y);

                                // Add the ellipse to the InkCanvas
                                inkc.Children.Add(rectangle);

                                break;
                            case "Polygon":
                                Polygon triangle = new Polygon
                                {
                                    Fill = shape.Fill,
                                    Stroke = shape.Stroke,
                                    StrokeThickness = shape.StrokeThickness
                                    
                                };


                                // triangle.RenderTransform = new TranslateTransform(shape.StartPoint.X, shape.StartPoint.Y);
                                
                                triangle.SetValue(InkCanvas.LeftProperty, shape.StartPoint.X);
                                triangle.SetValue(InkCanvas.TopProperty, shape.StartPoint.Y);
                                triangle.Points = shape.Points;

                                // Add the ellipse to the InkCanvas
                                inkc.Children.Add(triangle);

                                break;
                            case "Ellipse":

                                Ellipse ellipse = new Ellipse
                                {
                                    Fill = shape.Fill,
                                    Stroke = shape.Stroke,
                                    StrokeThickness = shape.StrokeThickness,
                                    Width = shape.Width,
                                    Height = shape.Height
                                };

                                // Set the position of the ellipse
                                InkCanvas.SetLeft(ellipse, shape.StartPoint.X);
                                InkCanvas.SetTop(ellipse, shape.StartPoint.Y);

                                // Add the ellipse to the InkCanvas
                                inkc.Children.Add(ellipse);

                                break;
                            default:
                                break;

                        }
                    }
                }

            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public void ChangeMode(EditModeType mode)
        {
            model.CurrentPaintingMode = mode;
            
        }


        public void ChangeSelectedStrokesIfNecessary()
        {
            foreach (var strokeItem in inkc.GetSelectedStrokes()) { 
                strokeItem.DrawingAttributes.Width = model.DrawingAttributesInkCanvas.Width;
                strokeItem.DrawingAttributes.Height = model.DrawingAttributesInkCanvas.Height;
            }
        }

        

        public void ChangeSelectedShapeFillColorIfNecessary()
        {
            foreach (var shapeItem in inkc.GetSelectedElements())
            {
                ((Shape)shapeItem).Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(model.ShapeFillColor)); 
            }
        }


        public void ChangeSelectedStrokeColorIfNecessary()
        {
            foreach (var strokeItem in inkc.GetSelectedStrokes())
            {
                strokeItem.DrawingAttributes.Color = model.DrawingAttributesInkCanvas.Color;
            }

            foreach (var shapeItem in inkc.GetSelectedElements())
            {
                ((Shape)shapeItem).Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(model.StrokeColor));
            }
        }



        public void ChangeSelectedShapeBorderSizeIfNecessary()
        {
            foreach (var shapeItem in inkc.GetSelectedElements())
            {
                ((Shape)shapeItem).StrokeThickness = model.ShapeStrokeThickness;
            }
        }


        private void inkc_MouseDown(object sender, MouseButtonEventArgs e)
        {

            iniP = e.GetPosition(inkc);
            isCreatingShape = true;

            SolidColorBrush fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(model.ShapeFillColor));
            SolidColorBrush strokeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(model.StrokeColor));

            switch (model.CurrentPaintingMode)
            {
                case EditModeType.Shape_Ellipse:
                    
                    CreateEllipse(fillColor, strokeColor, model.ShapeStrokeThickness);
                    break;
                case EditModeType.Shape_Triangle:
                    CreateTriangle(fillColor, strokeColor, model.ShapeStrokeThickness);
                    break;
                case EditModeType.Shape_Rect:
                    CreateRectangle(fillColor, strokeColor, model.ShapeStrokeThickness);
                    break;
                default:
                    break;

            }
        }

        private void inkc_MouseMove(object sender, MouseEventArgs e)
        {

            if (isCreatingShape && currentShape != null)
            {
                // Calculate the size of the shape based on the mouse position
                Point currentMousePosition = e.GetPosition(inkc);

                switch (model.CurrentPaintingMode)
                {
                    case EditModeType.Shape_Ellipse:
                        DrawEllipse(currentMousePosition);
                        break;
                    case EditModeType.Shape_Triangle:
                        DrawTriangle(currentMousePosition);
                        break;
                    case EditModeType.Shape_Rect:
                        DrawRectangle(currentMousePosition);
                        break;
                    default:
                        break;

                }
            } 
            else if(model.CurrentPaintingMode == EditModeType.Erase)
            {
                bool mouseIsDown = e.LeftButton == MouseButtonState.Pressed;
                if (mouseIsDown)
                {
                    InkPresenter inkPresenter = GetVisualChild<InkPresenter>(inkc);
                    HitTestResult hitTestResult = VisualTreeHelper.HitTest(inkPresenter, e.GetPosition(inkc));
                    inkc.Children.Remove((UIElement)hitTestResult.VisualHit);
                }
            }
        }


        private void inkc_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isCreatingShape = false;
            currentShape = null;

        }



        private void CreateEllipse(Brush fillColor, Brush strokeColor, double strokeThickness)
        {
            Ellipse ellipse = new Ellipse
            {
                Fill = fillColor,
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                Width = 0,
                Height = 0
            };



            // Set the position of the ellipse
            InkCanvas.SetLeft(ellipse, iniP.X);
            InkCanvas.SetTop(ellipse, iniP.Y);

            // Add the ellipse to the InkCanvas
            inkc.Children.Add(ellipse);

            currentShape = ellipse;
        }


        private void CreateRectangle(Brush fillColor, Brush strokeColor, double strokeThickness)
        {
            Rectangle rectangle = new Rectangle
            {
                Fill = fillColor,
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                Width = 0,
                Height = 0
            };



            // Set the position of the rectangle
            InkCanvas.SetLeft(rectangle, iniP.X);
            InkCanvas.SetTop(rectangle, iniP.Y);

            // Add the ellipse to the InkCanvas
            inkc.Children.Add(rectangle);

            currentShape = rectangle;
        }



        private void CreateTriangle(Brush fillColor, Brush strokeColor, double strokeThickness)
        {
            Polygon triangle = new Polygon
            {
                Fill = fillColor,
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,            
                Points = new PointCollection()
                {
                    new Point(iniP.X, iniP.Y),
                    new Point(iniP.X, iniP.Y),
                    new Point(iniP.X, iniP.Y),
                }

            };

            // Add the ellipse to the InkCanvas
            inkc.Children.Add(triangle);

            currentShape = triangle;
        }


        private void DrawEllipse(Point endP)
        {
            
            double width = Math.Max(0, endP.X - iniP.X);
            double height = Math.Max(0, endP.Y - iniP.Y);

            // Update the shape's size
            if (currentShape is Ellipse ellipse)
            {
                ellipse.Width = width;
                ellipse.Height = height;
            }

        }


        private void DrawTriangle(Point endP)
        {
            
            double smX = iniP.X < endP.X ? (double)iniP.X : (double)endP.X;
            double bgX = iniP.X < endP.X ? (double)endP.X : (double)iniP.X;

            double smY = iniP.Y < endP.Y ? (double)iniP.Y : (double)endP.Y;
            double bgY = iniP.Y < endP.Y ? (double)endP.Y : (double)iniP.Y;

            double width = Math.Max(0, bgX - smX);
            double height = Math.Max(0, bgY - smY);

            // Update the shape's size
            if (currentShape is Polygon triangle)
            {
                // triangle.RenderTransform = new TranslateTransform(iniP.X, iniP.Y);
                triangle.Points = new PointCollection() { new Point(0, bgY-smY), new Point(bgX-smX, bgY-smY), new Point(0 +((bgX - smX) / 2), 0) };
                triangle.SetValue(InkCanvas.LeftProperty, smX);
                triangle.SetValue(InkCanvas.TopProperty, smY);

            }
        }

        private void DrawRectangle(Point endP)
        {

            double width = Math.Max(0, endP.X - iniP.X);
            double height = Math.Max(0, endP.Y - iniP.Y);

            // Update the shape's size
            if (currentShape is Rectangle rectangle)
            {
                rectangle.Width = width;
                rectangle.Height = height;
            }
        }




        private void DrawWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        public T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                    break;
            }
            return child;
        }


        private void inkc_SelectionResized(object sender, EventArgs e)
        {
            Console.WriteLine("Resize");
        }

        private void inkc_SelectionResizing(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            foreach (var shapeItem in inkc.GetSelectedElements())
            {
                if (shapeItem is Polygon triangle)
                {
                    var widthDiff = e.NewRectangle.Width-e.OldRectangle.Width;
                    var heightDiff = e.NewRectangle.Height-e.OldRectangle.Height;
   
                    var triPoints = triangle.Points;

                    var x2Value = triPoints[1].X;
                    var y2Value = triPoints[0].Y;

                    x2Value += widthDiff;
                    y2Value += heightDiff;

                    triangle.Points = new PointCollection() { new Point(0, y2Value), new Point(x2Value, y2Value), new Point(x2Value / 2, 0) };
                    triangle.SetValue(InkCanvas.LeftProperty, e.OldRectangle.X);
                    triangle.SetValue(InkCanvas.TopProperty, e.OldRectangle.Y);
                }
            }

        }
    }
}
