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

namespace opencv_camera_calibrate.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModels.MainWindowViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            this.ViewModel = new ViewModels.MainWindowViewModel(this);
            this.DataContext = this.ViewModel;
        }
    }
}
