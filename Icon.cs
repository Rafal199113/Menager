using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Notatki
{
    public class Icon:getSettings
    {
        public string name { get; set; } = "cipeczka";
        public string src { get; set; }
        public BitmapImage bitmap { get; set; } 
        public SolidColorBrush borderColor2 { get; set; }
        public Action<object, MouseButtonEventArgs> MouseButtonEventHandlerMouseButton { get; set; }
    

        public Icon ShallowCopy()
        {
            return (Icon)this.MemberwiseClone();
        }

    }
}
