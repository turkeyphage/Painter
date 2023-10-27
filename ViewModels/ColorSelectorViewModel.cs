using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painter.ViewModels
{
    internal class ColorSelectorViewModel : ViewModel
    {

        public ColorSelectorViewModel()
        {
            //Default 

        }

        private List<string> _colorList = new List<string>() 
        {
                "Black" ,
                "LightSlateGray" ,
                "DarkRed",
                "Red",
                "DarkOrange",
                "Yellow",
                "Green",
                "DeepSkyBlue",
                "RoyalBlue",
                "MediumOrchid",
                "White",
                "LightGray",
                "RosyBrown",
                "Pink",
                "Orange",
                "Beige",
                "YellowGreen",
                "PaleTurquoise",
                "SteelBlue",
                "Lavender"
        };

        public List<string> ColorList
        {
            get => _colorList;

        }
    }
}
