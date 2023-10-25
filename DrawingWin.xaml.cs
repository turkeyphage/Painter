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

        //System.Windows.Ink.StrokeCollection _added;
        //System.Windows.Ink.StrokeCollection _removed;
        private bool handle = true;


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
                //_added = e.Added;
                //_removed = e.Removed;
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

        public void ChangeMode(int mode)
        {
            switch (mode)
            {

                case 0: inkc.EditingMode = InkCanvasEditingMode.Ink; break;
                case 1: inkc.EditingMode = InkCanvasEditingMode.EraseByPoint; break;
                case 2: inkc.EditingMode = InkCanvasEditingMode.EraseByStroke; break;
                case 3: inkc.EditingMode = InkCanvasEditingMode.Select; break;
                case 4: inkc.EditingMode = InkCanvasEditingMode.None; break;
                default: break;
            }

        }

    }
}
