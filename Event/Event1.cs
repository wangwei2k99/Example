using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Event
{
    public static class Event1
    {
        public static event EventHandler<SolidColorBrush> Eve1;
        public static void Eve1Run1(SolidColorBrush scb)
        {
            if (Eve1!=null)
            {
                Eve1(null, scb);
            }
        }
    }
}
