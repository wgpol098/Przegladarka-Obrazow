using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Przegladarka_obazow.ValueWindow
{
    public partial class PixelsRangeValue : Window
    {
        int MinRange = 0;
        int MaxRange = 255;
        bool R = false;
        bool G = false;
        bool B = false;
        public PixelsRangeValue()
        {
            InitializeComponent();
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {          
            MinRange = (int)(SliderMin.Value / 1000 * 255);
            MaxRange = (int)(SliderMax.Value / 1000 * 255);          
            if(CheckB.IsChecked == false && CheckG.IsChecked == false && CheckR.IsChecked == false)
            {
                R = true;
                G = true;
                B = true;
            }
            else
            {
                if (CheckR.IsChecked == true) R = true;
                if (CheckG.IsChecked == true) G = true;
                if (CheckB.IsChecked == true) B = true;
            }
            Close();
        }

        public int minrangevalue() => MinRange;
        public int maxrangevalue() => MaxRange;
        public bool rvalue() => R;
        public bool gvalue() => G;
        public bool bvalue() => B;


    }
}
