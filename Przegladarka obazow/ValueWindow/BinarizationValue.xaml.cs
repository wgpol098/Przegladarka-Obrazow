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
    public partial class BinarizationValue : Window
    {
        private int binarizationval = 0;
        private bool modified = false;
        private int treshold = 1;
        public BinarizationValue()
        {
            InitializeComponent();
        }

        private void OkButtonBinarization_Click(object sender, RoutedEventArgs e)
        {
            binarizationval = (int)(SliderBinarizationValue.Value / 1000 * 255);
            modified = true;
            Close();
        }

        public bool ModifiedStatus() => modified;
        public int binarizationvalue() => binarizationval;
        public int tresholdvalue() => treshold;

        private void rb1_Checked(object sender, RoutedEventArgs e)
        {
            treshold = -1;
        }

        private void rb2_Checked(object sender, RoutedEventArgs e)
        {
            treshold = 1;
        }
    }
}
