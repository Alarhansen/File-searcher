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
using System.Diagnostics;
using Microsoft.Office.Interop;

namespace File_searcher
{
    public partial class MainWindow : Window
    {
        public MainWindow() //en constructer til at initalisere class
        {
            InitializeComponent();
            

        }

 
        /// <summary>
        /// Åbner browser så man kan vælge en folder.
        /// </summary>
        private void BrowserButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog(); //objekt dlg af folder objektet
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderInputTextBox.Text = dlg.SelectedPath;
            }
            
        }
        /// <summary>
        /// Knap til at lukke programmet.
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e) //event
        {
            Environment.Exit(0);
        }
        
        /// <summary>
        /// Her søger man efter filerne, som indeholder det ord man leder efter samt giver en error hvis ingen folder er valgt.
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e) //event (pga bruger input fra GUI)
        {

            
            var folder = FolderInputTextBox.Text; //.Text gør at jeg kun kan få texten i objektet.
            if (Directory.Exists(folder) == false) //hvis der ikke er nogen folder, så = false hvor den bare returnere
            {
                System.Windows.MessageBox.Show("No folder input","ERROR",MessageBoxButton.OK,MessageBoxImage.Error) ;
                return;
            }
            var fil = GetFiles(folder); //i et normalt array kan man ikke tilføje ting, det kan man i en liste.
            

            var items = new List<Item>();
            
            foreach (var fn in fil) //første gang er fn = første element i arraylisten. Anden gang nr. 2 osv osv.
            {
                var found = FindWordTxt(fn, SearchFor.Text);
                if (found)
                {
                    items.Add(new Item(fn));//adder fn til FileListView. New Item laver en ny instans.

                }
                found = FindWordDocx(fn, SearchFor.Text);
                if (found)
                {
                    items.Add(new Item(fn));

                }
            }
           
            FileListView.ItemsSource = items; //binder filerne ind i listView (fra det andet dokument)



        }
        /// <summary>
        /// Søger efter ord i txt filer.
        /// </summary>
        /// <param name="fil">beskriver hele fil stien.</param>
        /// <param name="searchWord">Ordet man søger efter.</param>
        /// <returns></returns>
        private bool FindWordTxt(string fil, string searchWord) //komando
        {
            if (System.IO.Path.GetExtension(fil) != ".txt")
            {
                return false;
            }
            var caseCheck = StringComparison.Ordinal;
            if ((bool)IgnoreCaseCheckBox.IsChecked)
            {
                caseCheck = StringComparison.OrdinalIgnoreCase;
            }
            
            string text = File.ReadAllText(fil); //komando
            return text.IndexOf(searchWord,0,caseCheck) != -1;
            
        }


        /// <summary>
        /// Søger efter ord i word filer.
        /// </summary>
        /// <param name="fil">beskriver hele fil stien.</param>
        /// <param name="searchWord">Ordet man søger efter.</param>
        /// <returns></returns>
        private bool FindWordDocx (string fil, string searchWord) //metode
        {
            if (System.IO.Path.GetExtension(fil) != ".docx")
            {
                return false;
            }
            // Create an application object if the passed in object is null
            var winword = new Microsoft.Office.Interop.Word.Application(); //winword er en instans af application.

            // Use the application object to open our word document in ReadOnly mode
            var wordDoc = winword.Documents.Open(fil, ReadOnly: true);

            // Search for our string in the document
            Boolean result;
            if ((bool)IgnoreCaseCheckBox.IsChecked)
                result = wordDoc.Content.Text.IndexOf(searchWord, StringComparison.CurrentCultureIgnoreCase) >= 0;
            else
                result = wordDoc.Content.Text.IndexOf(searchWord) >= 0;
                

            // Close the document and the application since we're done searching
            wordDoc.Close();
            winword.Quit();

            return result;

        }

        /// <summary>
        /// Finder filer i folderen, og finder også filer i underfolderne.
        /// </summary>
        /// <param name="folder">folderen den søger i (fulde sti) </param>
        /// <returns>Returnere en liste af filerne.</returns>
        private List<string> GetFiles(string folder)  
        {
            var fil = Directory.GetFiles(folder, SearchPatternTextBox.Text).ToList();
            foreach (var dir in Directory.GetDirectories(folder)) //finder folderne i mappen.
            {
                var newFiles = GetFiles(dir); //henter de filer der inde under hver mappe. Den kalder sig selv igen og igen (rekursion)
                fil.AddRange(newFiles);
            }
            return fil;
            
        }

        /// <summary>
        /// Åbner filerne når man trykker på de enkelte filer.
        /// </summary>
        /// <param name="sender">Knappens fil</param>
        private void Button_Click(object sender, RoutedEventArgs e) //event
        {
            var button = (System.Windows.Controls.Button)sender; //caster objektet til button fordi jeg ved at det er en button. (computeren ved det ikke)
            var item = (Item)button.DataContext; //hvilken item/button man trykker på i sin itemSource. Som består af item liste (de filer den har fundet)
            var fil = item.FileName; //får filnavnet på den item jeg trykkede på.
            if (System.IO.Path.GetExtension(fil) == ".docx")
            {
                var winword = new Microsoft.Office.Interop.Word.Application();
                winword.Documents.Open(fil);
            }
            else
            {
                Process.Start("notepad.exe", fil);
            }
         
            
        }
    }


    /// <summary>
    /// class for filerne.
    /// </summary>
    public class Item
    {
        public Item(string fileName) //constructer for at initialisere FileName
        {
            FileName = fileName;
        }
        public string FileName { get; set; } //det er en property, fordi der står get;set; (en field ville ikke virke pga binding i WPF)
    }
}
