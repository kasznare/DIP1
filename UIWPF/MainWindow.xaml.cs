using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WindowsFormsApp1;
using Point = WindowsFormsApp1.Point;
using Logger = WindowsFormsApp1.Logger;
using ShapeLine = System.Windows.Shapes.Line;
namespace UIWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public Model model;
        public ObservableCollection<Point> Points { get; set; }
        public ObservableCollection<Line> Lines { get; set; }
        public ObservableCollection<Room> Rooms { get; set; }
        public MainWindow() {
            model = new Model();
            Points = new ObservableCollection<Point>();
            Lines = new ObservableCollection<Line>();
            Rooms = new ObservableCollection<Room>();
            model.InitModel();
            DataContext = this;
            InitializeComponent();
            ZoomViewbox.Width = 1000;
            ZoomViewbox.Height = 1000;
            Paint();
        }

        private void SimulationStep() {
            int moveDistance = int.Parse("10");

            Dictionary<Line, double> Costs = new Dictionary<Line, double>();
            double mincost = 100000;
            Line minline = null;

            //make threadpool like - room pool
            //fix number of modells, (number of threads) move elemnet, calculate cost

            //todo: make parallel
            //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-foreach-loop

            double actualCost = model.CalculateCost();
            foreach (Line line in model.modelLines) {

                Model tempModel = model.DeepCopy();
                tempModel.MoveLine(moveDistance, line);

                double cost = tempModel.CalculateCost();
                Costs.Add(line, cost);

                if (mincost > cost) {
                    mincost = cost;
                    minline = line;
                }
            }

            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }

            //model.MoveLine(moveDistance, model.GetRandomLine());
            model.MoveLine(moveDistance, minline);
        }

        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        private void drawModelRooms() {
            //Logger.WriteLog("draw model rooms");
            //foreach (Room modelRoom in model.modelRooms) {
            //    List<PointF> points = new List<PointF>();
            //    for (var index = 0; index < modelRoom.BoundaryPoints.AsReadOnly().Count; index++) {
            //        Point point = modelRoom.BoundaryPoints.AsReadOnly()[index];
            //        Logger.WriteLog($"Point at index {index} is {point}");
            //        points.Add(ConvertToFormCoordinate(point));
            //    }

            //    e.Graphics.FillPolygon(Brushes.Aquamarine, points.ToArray());
            //}
        }
        //convert to offset coordinates
        //private PointF ConvertToFormCoordinate(Point P) {
        //    if (P == null) return new PointF(0, 0);

        //    int x = Convert.ToInt32(P.X);
        //    int y = Convert.ToInt32(-P.Y + 600);
        //    //Logger.WriteLog($"Convert from {P} to {x},{y}");
        //    PointF point = new PointF(x, y);
        //    return point;
        //}
        private void MainWindow_OnMouseWheel(object sender, MouseWheelEventArgs e) {
            
        }
        //private void ZoomViewbox_MouseWheel(object sender, MouseWheelEventArgs e) {
        //    UpdateViewBox((e.Delta > 0) ? 5 : -5);
        //    //MessageBox.Show("mousewheel");
        //}
        //private void Window_MouseWheel(object sender, MouseWheelEventArgs e) {
        //    UpdateViewBox((e.Delta > 0) ? 5 : -5);
        //    MessageBox.Show("mousewheel2");

        //}

        //private void UpdateViewBox(int newValue) {
        //    if ((ZoomViewbox.Width >= 0) && ZoomViewbox.Height >= 0) {
        //        ZoomViewbox.Width += newValue;
        //        ZoomViewbox.Height += newValue;
        //    }
        //}
        private void Paint() {
            Points.Clear();
            Lines.Clear();
            Rooms.Clear();
            Points.Clear();
            Lines.Clear();
            Rooms.Clear();
            foreach (Point point in model.ModelPoints)
            {
                Points.Add(point);
            }

            foreach (Line line in model.modelLines)
            {
                Lines.Add(line);
            }

            foreach (Room room in model.modelRooms)
            {
                Rooms.Add(room);
            }

            Logger.WriteLog("paint started");
            foreach (Line line in model.modelLines)
            {
                ShapeLine myLine = new ShapeLine();

                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.X1 = line.startPoint.X;
                myLine.X2 = line.endPoint.X;
                myLine.Y1 = line.startPoint.Y;
                myLine.Y2 = line.endPoint.Y;
                myLine.StrokeEndLineCap = PenLineCap.Triangle;
                myLine.StrokeStartLineCap = PenLineCap.Round;
                //myLine.HorizontalAlignment = HorizontalAlignment.Left;
                //myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 3;

                //zoomviewboxgrid.Children.Add(myLine);
                zoomviewboxgrid2.Children.Add(myLine);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (actualSimulationThreshold < MaxSimulationThreshold) {
                SimulationStep();
                Paint();
            }
            else {
                MessageBox.Show("Simulation threshold exit. Optimum reached.");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            model = new Model();
            model.InitModel();
            Paint();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            int moveDistance = int.Parse("50");
            model.SplitEdge(moveDistance, model.GetRandomLine());
            Paint();
        }

        
    }
}
