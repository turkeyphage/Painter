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
        

        private EditModeType _currentMode = EditModeType.Draw;

        public DrawingWin()
        {
            InitializeComponent();

            inkc.DefaultDrawingAttributes.FitToCurve = true;
            inkc.DefaultDrawingAttributes.Color = Color.FromArgb(255, 255, 0, 255);
            DoStrokes = new Stack<DoStroke>();

            UndoStrokes = new Stack<DoStroke>();


            ChangeMode(EditModeType.Draw);
            inkc.Strokes.StrokesChanged += Strokes_StrokesChanged;
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
            ChangeMode(EditModeType.Draw);
            handle = false;
            DoStrokes.Clear();
            UndoStrokes.Clear();
            inkc.Strokes.Clear();
            inkc.Children.Clear();
            handle = true;
        }

        public void Save(string Filename)
        {
            try
            {
                FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                
                inkc.Strokes.Save(fs, false);
                
                
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            //inkc.EraserShape = new EllipseStylusShape(20, 20);
            //inkc.EraserShape = new RectangleStylusShape(20, 20);
        }

        public void LoadFile(string Filename) {
            try
            {
                FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                inkc.Strokes = new StrokeCollection(fs);
                



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
            _currentMode = mode;
            switch (mode)
            {

                case EditModeType.Draw: 
                    inkc.EditingMode = InkCanvasEditingMode.Ink; 
                    break;
                case EditModeType.Erase: 
                    inkc.EditingMode = InkCanvasEditingMode.EraseByPoint; 
                    break;
                case EditModeType.Select: 
                    inkc.EditingMode = InkCanvasEditingMode.Select; 
                    break;
                case EditModeType.Shape_Rect:
                case EditModeType.Shape_Ellipse:
                case EditModeType.Shape_Triangle:
                    inkc.EditingMode = InkCanvasEditingMode.None; 
                    break;
                default: 
                    break;
            }

        }

        private void inkc_MouseDown(object sender, MouseButtonEventArgs e)
        {

            iniP = e.GetPosition(inkc);
            isCreatingShape = true;

            switch (_currentMode)
            {
                case EditModeType.Shape_Ellipse:
                    CreateEllipse();
                    break;
                case EditModeType.Shape_Triangle:
                    CreateTriangle();
                    break;
                case EditModeType.Shape_Rect:
                    CreateRectangle();
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

                switch (_currentMode)
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






        private void CreateEllipse()
        {
            Ellipse ellipse = new Ellipse
            {
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
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


        private void CreateRectangle()
        {
            Rectangle rectangle = new Rectangle
            {
                Fill = Brushes.Yellow,
                Stroke = Brushes.Red,
                StrokeThickness = 2,
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



        private void CreateTriangle()
        {
            Polygon triangle = new Polygon
            {
                Fill = Brushes.Yellow,
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                
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
                triangle.Points = new PointCollection() { new Point(smX, bgY), new Point(bgX, bgY), new Point(smX + ((bgX - smX) / 2), smY) };
               
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
    }
}
