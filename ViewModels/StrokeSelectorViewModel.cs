using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Painter.ViewModels
{
    public class StrokeSelectorViewModel : ViewModel
    {


        public StrokeSelectorViewModel()
        {
            //Default 

            IsOnStrokeSize = true;
            setupToggleButtonStatus(true);

        }


        private Boolean _isOnStrokeSize = false;
        public Boolean IsOnStrokeSize
        {

            get => _isOnStrokeSize;
            set => Set(ref _isOnStrokeSize, value);
        }

        private Visibility _strokeCanvasVisibility = Visibility.Collapsed;
        public Visibility StrokeCanvasVisibility
        {

            get => _strokeCanvasVisibility;
            set => Set(ref _strokeCanvasVisibility, value);
        }



        private Boolean _isOnShapeBorderSize = false;
        public Boolean IsOnShapeBorderSize
        {

            get => _isOnShapeBorderSize;
            set => Set(ref _isOnShapeBorderSize, value);
        }

        private Visibility _shapeBorderSizeCanvasVisibility = Visibility.Collapsed;
        public Visibility ShapeBorderSizeCanvasVisibility
        {

            get => _shapeBorderSizeCanvasVisibility;
            set => Set(ref _shapeBorderSizeCanvasVisibility, value);
        }

        private double _sliderMinValue = 0;
        public double SliderMinValue {
            get => _sliderMinValue;
            set => Set(ref _sliderMinValue, value);
        }


        private double _sliderMaxValue = 0;
        public double SliderMaxValue
        {
            get => _sliderMaxValue;
            set => Set(ref _sliderMaxValue, value);
        }

        private double _strokeSizeMin = 2;
        private double _strokeSizeMax = 30;

        private double _shapeBorderSizeMin = 0;
        private double _shapeBorderSizeMax = 10;


        public void setupToggleButtonStatus(Boolean IsOnStrokeOption)
        {

            IsOnStrokeSize = IsOnStrokeOption;
            IsOnShapeBorderSize = !IsOnStrokeOption;

            if (IsOnStrokeOption == true)
            {
                StrokeCanvasVisibility = Visibility.Visible;
                ShapeBorderSizeCanvasVisibility = Visibility.Collapsed;
                SetSliderValueForStroke();
            }
            else
            {
                StrokeCanvasVisibility = Visibility.Collapsed;
                ShapeBorderSizeCanvasVisibility = Visibility.Visible;
                SetSliderValueForShapeBorder();
            }


        }

        private void SetSliderValueForStroke()
        {
            SliderMinValue = _strokeSizeMin;
            SliderMaxValue = _strokeSizeMax;
        }

        private void SetSliderValueForShapeBorder() {
            SliderMinValue = _shapeBorderSizeMin;
            SliderMaxValue = _shapeBorderSizeMax;
        }
    }
    
}
