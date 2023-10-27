using Painter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Painter.ViewModels
{
    public class ToolbarViewModel : ViewModel
    {

        private static ToolbarViewModel instance = null;
        
        public static ToolbarViewModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new ToolbarViewModel();
                return instance;
            }
        }

        public ToolbarViewModel()
        {
            //Default 
            
        }
    }
}
