using System;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using Przegladarka_obazow.AppWindow;

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

            if(FileContentBox.Text.ToString().Contains(path) == false)
                FileContentBox.Text = FileContentBox.Text + Environment.NewLine + path;
            else System.Windows.Forms.MessageBox.Show("Folder jest już na liście!", "Informacja!", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter zapis = new StreamWriter(fileName);
            String content = FileContentBox.Text;
            zapis.Write(content);
            zapis.Close();
            System.Windows.MessageBox.Show("Zapis zakończony powodzeniem!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            FileContentBox.Focus();
            string tmp = FileContentBox.SelectedText;
            System.Windows.MessageBox.Show(tmp);
            string tmp1 = FileContentBox.Text;
            tmp1 = tmp1.Replace(tmp, "");
            FileContentBox.Text = tmp1;
        }
    }
}
