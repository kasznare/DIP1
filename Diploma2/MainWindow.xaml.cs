using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UIWPF.Model;
using UIWPF.Services;
using ShapeLine = System.Windows.Shapes.Line;

namespace Diploma2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool isPainting;
        private int selectedLineIndex = -1;
        private int selectedPointIndex = -1;
        public string StatusMessage { get; set; }
        public ObservableCollection<_Point> Points { get; set; }
        public ObservableCollection<_Line> Lines { get; set; }
        public ObservableCollection<_Room> Rooms { get; set; }
        public _Model model { get; set; }
        private void Log(string message) {
            if (message != null && message != StatusMessage) StatusMessage = message;
        }
        public MainWindow() {
            Points = new ObservableCollection<_Point>();
            Lines = new ObservableCollection<_Line>();
            Rooms = new ObservableCollection<_Room>();
            InitializeComponent();
            LoadModels();
            Paint();
        }

        private void LoadModels() {
            model = ModelConfigurations.InitSimplestModel();
            Points = new ObservableCollection<_Point>(model.AllPointsFlat());
            Lines = new ObservableCollection<_Line>(model.AllLinesFlat());
            Rooms = model.rooms;
            LineGrid.ItemsSource = Lines;
            PointGrid.ItemsSource = Points;
            RoomGrid.ItemsSource = Rooms;
        }

        private void Paint() {
            isPainting = true;
            testcanvas.Children.Clear();

            var allLinesFlat = model.AllLinesFlat();
            for (var i = 0; i < allLinesFlat.Count; i++) {
                _Line line = allLinesFlat[i];
                ShapeLine _line = new ShapeLine();
                Brush solidColorBrush = new SolidColorBrush(Color.FromArgb(95, 0, 0, 0));
                solidColorBrush.Opacity = 0.5;
                if (i.Equals(selectedLineIndex)) {
                    solidColorBrush = Brushes.Yellow;
                }

                _line.Stroke = solidColorBrush;
                _line.X1 = line.StartPoint.X;
                _line.X2 = line.EndPoint.X;
                _line.Y1 = line.StartPoint.Y;
                _line.Y2 = line.EndPoint.Y;
                _line.StrokeEndLineCap = PenLineCap.Triangle;
                _line.StrokeStartLineCap = PenLineCap.Round;
                _line.StrokeThickness = 10;
                _line.ToolTip = line.ToString();
                testcanvas.Children.Add(_line);
            }

            List<_Point> allPointsFlat = model.AllPointsFlat();
            for (var i = 0; i < allPointsFlat.Count; i++) {
                _Point point = allPointsFlat[i];
                ShapeLine _line = new ShapeLine();

                var solidColorBrush = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
                solidColorBrush.Opacity = 0.5;
                if (i.Equals(selectedPointIndex)) {
                    solidColorBrush = Brushes.GreenYellow;
                }

                _line.Stroke = solidColorBrush;
                _line.X1 = point.X;
                _line.X2 = point.X + 1;
                _line.Y1 = point.Y;
                _line.Y2 = point.Y + 1;
                _line.StrokeStartLineCap = PenLineCap.Round;
                _line.StrokeEndLineCap = PenLineCap.Triangle;
                _line.StrokeThickness = 10;
                _line.ToolTip = point.ToString();
                testcanvas.Children.Add(_line);
            }

            //foreach (MyRoom room in model.modelRooms) {
            foreach (_Room room in Rooms) {
                List<_Point> boundaries = room.GetBoundaryPointsSorted();
                if (!boundaries.Any()) continue;
                //boundaries.RemoveAll(item => item == null); //this is error handling, but I would need to figure out why nulls exist
                List<Point> convertedPoints = boundaries.Select(i => new Point(i.X, i.Y)).ToList();
                Polygon p = new Polygon();
                p.Points = new PointCollection(convertedPoints);
                //p.Fill = new SolidColorBrush(room.type.fillColor.ToMediaColor());
                p.Opacity = 0.25;
                p.ToolTip = room.ToString();
                testcanvas.Children.Add(p);
            }

            isPainting = false;
        }

        private void LoadModel_OnClick(object sender, RoutedEventArgs e) {
            LoadModels();
        }

        private void MoveLine_OnClick(object sender, RoutedEventArgs e) {
            model.MoveLine();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Paint_OnClick(object sender, RoutedEventArgs e) {
            Paint();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.A:
                    Dispatcher.BeginInvoke((Action)(() => TabControl.SelectedIndex = 0));
                    break;
                case Key.D:
                    Dispatcher.BeginInvoke((Action)(() => TabControl.SelectedIndex = 1));
                    break;
                case Key.M:
                    model.MoveLine();
                    break;
                case Key.L:
                    LoadModels();
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
        }
    }
}
