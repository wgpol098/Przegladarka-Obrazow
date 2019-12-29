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
    /// <summary>
    /// Logika interakcji dla klasy HueModifierValue.xaml
    /// </summary>
    public partial class HueModifierValue : Window
    {
        private int hueval = 0;
        private bool modified = false;
        public HueModifierValue()
        {
            InitializeComponent();
        }

        private void OkButtonLight_Click(object sender, RoutedEventArgs e)
        {
            hueval = (int)(SliderHueModifier.Value/1000*359);
            modified = true;
            Close();
        }

        public bool ModifiedStatus() => modified;
        public int huevalue() => hueval;
    }
}
