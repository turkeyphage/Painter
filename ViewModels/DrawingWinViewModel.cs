using Painter.Models;
using Painter.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace Painter.ViewModels
{
    public class DrawingWinViewModel : ViewModel
    {
        private static DrawingWinViewModel instance = null;
        public static DrawingWinViewModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new DrawingWinViewModel();
                return instance;
            }
        }

        private InkCanvasEditingMode _inkCanvasMode = new InkCanvasEditingMode();

        public InkCanvasEditingMode InkCanvasMode
        {
            get => _inkCanvasMode;
            set => Set(ref _inkCanvasMode, value);
        }


        private EditModeType _currentPaintingMode = EditModeType.Draw;

        public EditModeType CurrentPaintingMode
        {
            get => _currentPaintingMode;
            set => Set(ref _currentPaintingMode, value);

        }


        //private DrawingAttributes _drawingAttributesInkCanvas = new DrawingAttributes();

        //public DrawingAttributes DrawingAttributesInkCanvas
        //{
        //    get => _drawingAttributesInkCanvas;
        //    set => Set(ref _drawingAttributesInkCanvas, value);
        //}

        //private double _strokeSize = 1;

        //public double StrokeSize
        //{
        //    get => _strokeSize;
        //    set
        //    {
        //        if (EditModeInkCanvas == InkCanvasEditingMode.Ink)
        //        {
        //            var newSize = value;

        //            DrawingAttributesInkCanvas.Width = newSize;
        //            DrawingAttributesInkCanvas.Height = newSize;

        //        }

        //        Set(ref _strokeSize, value);
        //    }
        //}



        public DrawingWinViewModel()
        {
            //Default 
            _inkCanvasMode = InkCanvasEditingMode.Ink;
            _currentPaintingMode = EditModeType.Draw;

        }
    }
}
