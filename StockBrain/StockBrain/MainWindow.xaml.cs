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

namespace StockBrain
{
    using StockBrainModules.Caching;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime startTime = new DateTime(1980, 12, 12);
            DateTime endTime = new DateTime(2015, 7, 31);

            StockHistory history = new StockHistory("AAPL", StockHistory.RangeType.Daily, startTime, endTime);
            history.LoadPriceHistory();
        }
    }
}
