using Newtonsoft.Json.Bson;
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
            set {
                updateEditModeFlag(value);
                switch (value)
                {
                    
                    case EditModeType.Draw:
                        InkCanvasMode = InkCanvasEditingMode.Ink;
                        break;
                    case EditModeType.Erase:
                        InkCanvasMode = InkCanvasEditingMode.EraseByPoint;
                        break;
                    case EditModeType.Select:
                        InkCanvasMode = InkCanvasEditingMode.Select;
                        break;
                    case EditModeType.Shape_Rect:
                    case EditModeType.Shape_Ellipse:
                    case EditModeType.Shape_Triangle:
                        InkCanvasMode = InkCanvasEditingMode.None;
                        break;
                    default:
                        InkCanvasMode = InkCanvasEditingMode.Ink;
                        break;
                }
                Set(ref _currentPaintingMode, value);
                
            }
               
        }



        private Boolean _isDraw = false;
        public Boolean IsDraw
        {
            get => _isDraw;
            set => Set(ref _isDraw, value);
        }

        private Boolean _isSelect = false;
        public Boolean IsSelect
        {
            get => _isSelect;
            set => Set(ref _isSelect, value);

        }
        private Boolean _isErase = false;
        public Boolean IsErase
        {

            get => _isErase;
            set => Set(ref _isErase, value);
            

        }

        private Boolean _isShapeRect = false;
        public Boolean IsShapeRect
        {
            get => _isShapeRect;
            set => Set(ref _isShapeRect, value);

        }
        private Boolean _isShapeEllipse = false;
        public Boolean IsShapeEllipse
        {
            get => _isShapeEllipse;
            set => Set(ref _isShapeEllipse, value);

        }
        private Boolean _isShapeTriangle = false;
        public Boolean IsShapeTriangle
        {

            get => _isShapeTriangle;
            set => Set(ref _isShapeTriangle, value);
        }


        private void updateEditModeFlag(EditModeType value)
        {
            IsDraw = value == EditModeType.Draw;
            IsErase = value == EditModeType.Erase;
            IsSelect = value == EditModeType.Select;
            IsShapeEllipse = value == EditModeType.Shape_Ellipse;
            IsShapeRect = value == EditModeType.Shape_Rect;
            IsShapeTriangle = value == EditModeType.Shape_Triangle;
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
            CurrentPaintingMode = EditModeType.Draw;
            updateEditModeFlag(CurrentPaintingMode);

        }
    }
}
