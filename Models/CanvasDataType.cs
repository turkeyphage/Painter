using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace Painter
{


    public class ShapeObject
    {
        public string ShapeType { get; set; }

        public double Width { get; set; } = 0;

        public double Height { get; set; } = 0;

        public Brush Fill { get; set; }

        public Brush Stroke { get; set; }

        public double StrokeThickness { get; set; } 

        public PointCollection Points { get; set; } = new PointCollection();

        public Point StartPoint { get; set; } = new Point(0, 0);

    }

    public class SaveObjects
    {
        public StrokeCollection inkStrokeData { get; set; } = null;

        public List<ShapeObject> shapesData { get; set; } = null;


    }


}
