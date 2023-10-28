using Painter.Models;
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
using System.Xaml;
using Newtonsoft.Json.Linq;
using System.Windows.Markup;
using Painter.ViewModels;

namespace Painter
{
    /// <summary>
    /// Interaction logic for DrawingWin.xaml
    /// </summary>
    public partial class DrawingWin : Window
    {

        public Stack<DoStroke> DoStrokes { get; set; }

        public Stack<DoStroke> UndoStrokes { get; set; }


        public struct DoStroke
        {
            public string ActionFlag { get; set; }
            public System.Windows.Ink.Stroke Stroke { get; set; }
        }


        private bool handle = true;


        private Point iniP;

        private bool isCreatingShape = false;
        private UIElement currentShape;
        

        DrawingWinViewModel model = DrawingWinViewModel.Instance;

        public DrawingWin()
        {
            InitializeComponent();

            DoStrokes = new Stack<DoStroke>();

            UndoStrokes = new Stack<DoStroke>();

            inkc.Strokes.StrokesChanged += Strokes_StrokesChanged;
            DataContext = model;

            model.DrawingAttributesInkCanvas.FitToCurve = true;
        }



        private void Strokes_StrokesChanged(object sender, System.Windows.Ink.StrokeCollectionChangedEventArgs e)
        {
            if (handle)
            {
                DoStrokes.Push(new DoStroke
                {
                    ActionFlag = e.Added.Count > 0 ? "ADD" : "REMOVE",
                    Stroke = e.Added.Count > 0 ? e.Added[0] : e.Removed[0]
                });
            }
        }


        public void CreateNew() {
            // ChangeMode(EditModeType.Draw);
            model.CurrentPaintingMode = EditModeType.Draw;
            handle = false;
            DoStrokes.Clear();
            UndoStrokes.Clear();
            inkc.Strokes.Clear();
            inkc.Children.Clear();
            handle = true;
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


                        //Console.WriteLine(rect.Width);
                        //Console.WriteLine(rect.Height);
                        //Console.WriteLine(rect.Fill);
                        //Console.WriteLine(rect.Stroke);
                        //Console.WriteLine(rect.StrokeThickness);
                        //Console.WriteLine(location);

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

                        //Console.WriteLine(polygon.Fill);
                        //Console.WriteLine(polygon.Stroke);
                        //Console.WriteLine(polygon.StrokeThickness);
                        //Console.WriteLine(polygon.Points);
                        //Console.WriteLine(parent);
                        //Console.WriteLine(location);

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


                        //Console.WriteLine(ellipse.Width);
                        //Console.WriteLine(ellipse.Height);
                        //Console.WriteLine(ellipse.Fill);
                        //Console.WriteLine(ellipse.Stroke);
                        //Console.WriteLine(ellipse.StrokeThickness);
                        //Console.WriteLine(location);


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


                                triangle.RenderTransform = new TranslateTransform(shape.StartPoint.X, shape.StartPoint.Y);
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

        public void Redo()
        {
            Console.WriteLine("Redo");
            Console.WriteLine(inkc);


            handle = false;
            if (UndoStrokes.Count > 0)
            {
                DoStroke @do = UndoStrokes.Pop();
                if (@do.ActionFlag.Equals("ADD"))
                {
                    inkc.Strokes.Add(@do.Stroke);
                }
                else
                {
                    inkc.Strokes.Remove(@do.Stroke);
                }
            }
            handle = true;




        }

        public void Undo()
        {
            Console.WriteLine("Undo");
            Console.WriteLine(inkc);

            handle = false;

            if (DoStrokes.Count > 0)
            {
                DoStroke @do = DoStrokes.Pop();
                if (@do.ActionFlag.Equals("ADD"))
                {
                    inkc.Strokes.Remove(@do.Stroke);
                }
                else
                {
                    inkc.Strokes.Add(@do.Stroke);
                }

                UndoStrokes.Push(@do);
            }
            handle = true;


        }

        public void ChangeMode(EditModeType mode)
        {

            model.CurrentPaintingMode = mode;
            

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
                triangle.RenderTransform = new TranslateTransform(iniP.X, iniP.Y);
                triangle.Points = new PointCollection() { new Point(0, bgY-smY), new Point(bgX-smX, bgY-smY), new Point(0 +((bgX - smX) / 2), 0) };
               
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


        private List<Point> GenerateElipseGeometry(Point st, Point ed) { 
            double a = 0.5 * (ed.X - st.X);
            double b = 0.5 * (ed.Y - st.Y);

            List<Point> pointList = new List<Point>();
            for(double r = 0; r <= 2*Math.PI; r = r + 0.01)
            {
                pointList.Add(new System.Windows.Point(0.5 * (st.X + ed.X) + a * Math.Cos(r), 0.5 * (st.Y + ed.Y) + b * Math.Sin(r)));


            }
            return pointList;
        }

        private void inkc_SelectionChanged(object sender, EventArgs e)
        {
            if (inkc.GetSelectedStrokes().Count > 0 || inkc.GetSelectedElements().Count > 0)
            {
                // Object(s) are selected
                // You can perform actions here when objects are selected
                Console.WriteLine(inkc.GetSelectedStrokes());
                Console.WriteLine(inkc.GetSelectedElements());
                 

            }
            else
            {
                // No objects are selected
                // Perform actions when there are no selected objects
            }
        }

        private void inkc_SelectionResized(object sender, EventArgs e)
        {
            Console.WriteLine("Resize");


        }

        private void DrawWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
