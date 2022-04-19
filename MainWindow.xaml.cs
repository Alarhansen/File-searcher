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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;



namespace File_searcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BrowserButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog(); //objekt dlg af folder objektet
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderInputTextBox.Text = dlg.SelectedPath;
            }
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        public List<Item> Items { get; set; }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            FileListView.Items.Clear();
            var folder = FolderInputTextBox.Text; //.Text gør at jeg kun kan få texten i objektet.
            var fil = Directory.GetFiles(folder,SearchPatternTextBox.Text).ToList(); //i et normalt array kan man ikke tilføje ting, det kan man i en liste.
            var dirs = Directory.GetDirectories(folder,SearchPatternTextBox.Text,SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                fil.AddRange(Directory.GetFiles(dir,SearchPatternTextBox.Text)); //addRange kan tage alle filer på en gang. hvis  man bare brugt add kunne man kun tage en af gangen.
            
            }

            Items = new List<Item>();

            foreach (var fn in fil) //første gang er fn = første element i arraylisten. Anden gang nr. 2 osv osv.
            {
                var found = FindWord(fn, SearchFor.Text);
                if (found)
                {
                    Items.Add(new Item(fn));//adder fn til FileListView

                }
                
            }
            foreach(var i in Items)
            {
                FileListView.Items.Add(i.FileName);
            }
            

            
        }
        private bool FindWord(string fil, string searchWord)
        {
            var caseCheck = StringComparison.Ordinal;
            if ((bool)IgnoreCaseCheckBox.IsChecked)
            {
                caseCheck = StringComparison.OrdinalIgnoreCase;
            }
            
            string text = File.ReadAllText(fil);
            return text.IndexOf(searchWord,0,caseCheck) != -1;
            
        }

      
    }
    public class Item
    {
        public Item(string fileName)
        {
            FileName = fileName;
        }
        public string FileName { get; set; }
    }
}
