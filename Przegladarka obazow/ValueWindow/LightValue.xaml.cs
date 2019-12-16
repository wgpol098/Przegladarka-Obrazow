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

namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy LightValue.xaml
    /// </summary>
    public partial class LightValue : Window
    {
        private int lightval = 0;
        public LightValue()
        {
            InitializeComponent();
        }

        private void OkButtonLight_Click(object sender, RoutedEventArgs e)
        {
            lightval = (int)(SliderLight.Value/1000*255);
            Close();
        }

        public int lightvalue() => lightval;
    }
}
