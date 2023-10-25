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


        private System.Windows.Point iniP;

        private EditModeType _currentMode = EditModeType.Draw;

        public DrawingWin()
        {
            InitializeComponent();

            inkc.DefaultDrawingAttributes.FitToCurve = true;
            inkc.DefaultDrawingAttributes.Color = Color.FromArgb(255, 255, 0, 255);
            DoStrokes = new Stack<DoStroke>();

            UndoStrokes = new Stack<DoStroke>();


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
            handle = false;
            DoStrokes.Clear();
            UndoStrokes.Clear();
            inkc.Strokes.Clear();
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
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                iniP = e.GetPosition(inkc);
            }

        }

        private void inkc_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                switch (_currentMode)
                {
                    case EditModeType.Shape_Rect:
                        inkc.EditingMode = InkCanvasEditingMode.None;
                        DrawRectangle(e.GetPosition(inkc));
                        break;
                    case EditModeType.Shape_Ellipse:
                        inkc.EditingMode = InkCanvasEditingMode.None;
                        DrawEllipse(e.GetPosition(inkc));
                        break;
                    case EditModeType.Shape_Triangle:
                        inkc.EditingMode = InkCanvasEditingMode.None;
                        DrawTriangle(e.GetPosition(inkc));
                        break;

                    default:
                        break;
                }


                



                

            }



        }


        private void DrawRectangle(Point endP)
        {

            
            List<Point> pointList = new List<Point> {

                    new Point(iniP.X, iniP.Y),
                    new Point(iniP.X, endP.Y),
                    new Point(endP.X, endP.Y),
                    new Point(endP.X, iniP.Y),
                    new Point(iniP.X, iniP.Y),

                };

            DrawingAttributes rectDrawAttribute = inkc.DefaultDrawingAttributes.Clone();

            rectDrawAttribute.FitToCurve = false;
            rectDrawAttribute.StylusTip = StylusTip.Rectangle;




            StylusPointCollection stylusPoints = new StylusPointCollection(pointList);
            Stroke stroke = new Stroke(stylusPoints)
            {
                DrawingAttributes = rectDrawAttribute

            };
            inkc.Strokes.Clear();
            inkc.Strokes.Add(stroke);
        }


        private void DrawEllipse(Point endP)
        {
            List<Point> pointList = GenerateElipseGeometry(iniP, endP);

            DrawingAttributes ellipseDrawAttribute = inkc.DefaultDrawingAttributes.Clone();

            ellipseDrawAttribute.FitToCurve = false;
            ellipseDrawAttribute.StylusTip = StylusTip.Ellipse;


            StylusPointCollection stylusPoints = new StylusPointCollection(pointList);
            Stroke stroke = new Stroke(stylusPoints)
            {
                DrawingAttributes = ellipseDrawAttribute

            };
            inkc.Strokes.Clear();
            inkc.Strokes.Add(stroke);
        }


        private void DrawTriangle(Point endP)
        {
            DrawingAttributes rectDrawAttribute = inkc.DefaultDrawingAttributes.Clone();

            rectDrawAttribute.FitToCurve = false;
            rectDrawAttribute.StylusTip = StylusTip.Rectangle;
            Point midTopP = new Point((iniP.X + endP.X) / 2 - rectDrawAttribute.Width / 2, iniP.Y);

            List<Point> pointList = new List<Point> {

                    new Point(midTopP.X, midTopP.Y),
                    new Point(endP.X, endP.Y),
                    new Point(iniP.X, endP.Y),
                    new Point(midTopP.X, midTopP.Y),

                };




            StylusPointCollection stylusPoints = new StylusPointCollection(pointList);
            Stroke stroke = new Stroke(stylusPoints)
            {
                DrawingAttributes = rectDrawAttribute

            };
            inkc.Strokes.Clear();
            inkc.Strokes.Add(stroke);

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


    }
}
