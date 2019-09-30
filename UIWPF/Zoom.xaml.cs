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

namespace UIWPF {
    /// <summary>
    /// Interaction logic for Zoom.xaml
    /// </summary>
    public partial class Zoom : Window {
        public Zoom() {
            InitializeComponent();

            ZoomViewbox.Width = 100;
            ZoomViewbox.Height = 100;
        }

        private void MainWindow_OnMouseWheel(object sender, MouseWheelEventArgs e) {
            UpdateViewBox((e.Delta > 0) ? 5 : -5);
        }

        private void UpdateViewBox(int newValue) {
            if ((ZoomViewbox.Width >= 0) && ZoomViewbox.Height >= 0) {
                ZoomViewbox.Width += newValue;
                ZoomViewbox.Height += newValue;
            }
        }
    }
}

