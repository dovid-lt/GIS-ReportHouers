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

namespace GetHouers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewS ViewS { get; set; }
        List<WorkerTimeEntry> listWorkers;

        public MainWindow()
        {
            ViewS = new ViewS();
            ViewS.Settings = Settings.Load();
            var lastMonth = DateTime.Now.AddDays(-12);
            ViewS.Month = lastMonth.Month;
            ViewS.Year = lastMonth.Year;
            InitializeComponent();
            DataContext = ViewS;

        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            progres.Visibility = Visibility.Visible;
            var down = new DownloadData();
            var rows = await down.getRows(ViewS.Year, ViewS.Month, ViewS.Settings);
            listWorkers = rows.Select(WorkerTimeEntry.FromRawData).Where(x => x.WorkerNumber > 0 && x.At > ViewS.Settings.TakeAfter).ToList();
            dgResults.ItemsSource = listWorkers;
            progres.Visibility = Visibility.Hidden;
        }

        private void btnSaveSett(object sender, RoutedEventArgs e)
        {
            ViewS.Settings.Save();
        }

        private async void btnExportClick(object sender, RoutedEventArgs e)
        {
            if (listWorkers == null || listWorkers.Count == 0)
                return;
            else
            {
                progres.Visibility = Visibility.Visible;
                btnExport.IsEnabled = false;

                var exp = new ExportData();

                await Task.WhenAll(exp.ExportOkets(listWorkers, ViewS.Settings), Task.Delay(1500));
                var last = listWorkers.Max(x => x.At);

                if (last > ViewS.Settings.TakeAfter && MessageBox.Show(this, "האם לעדכן את התאריך האחרון לייצוא?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ViewS.Settings.TakeAfter = last;
                    ViewS.Settings.Save();
                }

                btnExport.IsEnabled = true;
                progres.Visibility = Visibility.Hidden;
            }
        }
    }

    class ViewS
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Settings Settings { get; set; }
    }

}
