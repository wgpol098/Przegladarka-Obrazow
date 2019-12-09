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
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy Foldery.xaml
    /// </summary>
    public partial class Foldery : Window
    {
        readonly String fileName;

        public Foldery(string _fileName)
        {
            InitializeComponent();

            fileName = _fileName;
            StreamReader file = new StreamReader(fileName);
            String content = file.ReadToEnd();
            FileContentBox.Text = content;
            file.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            StreamReader tmp = new StreamReader(fileName);
            String temp = tmp.ReadToEnd();
            tmp.Close();
            if(temp != FileContentBox.Text)
            {
                DialogResult dlg = System.Windows.Forms.MessageBox.Show("Zatwierdzić niezapisane zmiany?", "Ostrzeżenie!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlg != System.Windows.Forms.DialogResult.No) SaveButton_Click(sender, e);              
            }
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.ShowDialog();
            String path = d.SelectedPath;

            FileContentBox.Text = FileContentBox.Text + Environment.NewLine + path;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter zapis = new StreamWriter(fileName);
            String content = FileContentBox.Text;
            zapis.Write(content);
            zapis.Close();
            System.Windows.MessageBox.Show("Zapis zakończony powodzeniem!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
