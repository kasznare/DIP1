using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsFormsApp1;
using WindowsFormsApp1.GeometryModel;
using WindowsFormsApp1.Utilities;
using Logger = WindowsFormsApp1.Logger;
using MessageBox = System.Windows.MessageBox;
using ShapeEllipse = System.Windows.Shapes.Ellipse;
using ShapeLine = System.Windows.Shapes.Line;
using ShapeRectangle = System.Windows.Shapes.Rectangle;
namespace UIWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public Model model;
        public ObservableCollection<MyPoint> Points { get; set; }
        public ObservableCollection<MyLine> Lines { get; set; }
        public ObservableCollection<Room> Rooms { get; set; }

        public ObservableCollection<Costs> SimulationCosts { get; set; }

        public MainWindow() {
            model = new Model();
            Points = new ObservableCollection<MyPoint>();
            Lines = new ObservableCollection<MyLine>();
            Rooms = new ObservableCollection<Room>();
            SimulationCosts = new ObservableCollection<Costs>();
            model.InitModel();
            //model.InitRoomTypes();
            DataContext = this;
            InitializeComponent();
            //ZoomViewbox.Width = 1000;
            //ZoomViewbox.Height = 1000;
            Paint();
        }

        public int actualSimulationIndex = 0;
        readonly object locker = new object();
        private void SimulationStepMove() {
            int moveDistance = int.Parse("10");

            Dictionary<MyLine, double> Costs = new Dictionary<MyLine, double>();
            double mincost = 10000000;
            MyLine minline = null;

            //make threadpool like - room pool
            //fix number of modells, (number of threads) move elemnet, calculate cost

            double actualCost = model.CalculateCost();
            Parallel.For(0, model.modelLines.Count,
                index => {
                    MyLine myLine = model.modelLines.ElementAt(index);
                    MyLine newMyLine = null;
                    Model tempModel = model.DeepCopy(myLine, out newMyLine);
                    tempModel.MoveLine(moveDistance, newMyLine);

                    double cost = tempModel.CalculateCost();
                    lock (locker) {
                        Costs.Add(myLine, cost);
                        if (mincost > cost) {
                            mincost = cost;
                            minline = myLine;
                        }
                    }
                });

            foreach (MyLine line in model.modelLines) {

                //MyLine newLine = null;
                //Model tempModel = model.DeepCopy(line, out newLine);
                //tempModel.MoveLine(moveDistance, newLine);

                //double cost = tempModel.CalculateCost();
                //if (!Costs.ContainsKey(line)) {
                //    Costs.Add(line, cost);
                //}

                //if (mincost > cost) {
                //    mincost = cost;
                //    minline = line;
                //}
            }

            double minCost = Costs.Values.Min();

            foreach (KeyValuePair<MyLine, double> pair in Costs) {
                if (pair.Value.Equals(mincost)) {
                    minline = pair.Key;
                }
            }

            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }


            if (minline != null) {

                model.MoveLine(moveDistance, minline);
            }
            else {
                MessageBox.Show("no line to move");
            }

            SimulationCosts.Add(new Costs(actualSimulationIndex, actualCost));
            actualSimulationIndex++;
        }

        private void SimulationStepSwitch() {
            Dictionary<Room, double> RoomCosts = new Dictionary<Room, double>();

            double mincost = 1000000;
            double actualCost = model.CalculateCost();
            int rooms = model.modelRooms.Count;
            Room switchThisRoomFrom=null;
            Room switchThisRoomTo=null;
            Parallel.For(0, rooms,
                index => {
                    Parallel.For(index+1, rooms, secondindex => {

                        Room r1 = model.modelRooms.ElementAt(index);
                        Room r2 = model.modelRooms.ElementAt(secondindex);
                        Room r1target = null;
                        Room r2target = null;
                        Model tempModel = model.DeepCopy(r1, r2, out r1target, out r2target);
                        tempModel.SwitchRooms(ref r1target, ref r2target);

                        double cost = tempModel.CalculateCost();
                        lock (locker) {
                            RoomCosts.Add(r1, cost);
                            if (mincost > cost) {
                                mincost = cost;
                                //this might need to be switched later
                                switchThisRoomFrom = r1;
                                switchThisRoomTo = r2;
                            }
                        }
                    });
                });

            //double minCost = RoomCosts.Values.Min();

            //foreach (KeyValuePair<Room, double> pair in RoomCosts) {
            //    if (pair.Value.Equals(mincost)) {
            //        minline = pair.Key;
            //    }
            //}

            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }


            if (switchThisRoomFrom != null && switchThisRoomTo!=null) {

                model.SwitchRooms(ref switchThisRoomFrom, ref switchThisRoomTo);
            }
            else {
                MessageBox.Show("no room to switch");
            }

            SimulationCosts.Add(new Costs(actualSimulationIndex, actualCost));
            actualSimulationIndex++;
        }
        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        private void drawModelRooms() {
            //Logger.WriteLog("draw model rooms");
            //foreach (Room modelRoom in model.modelRooms) {
            //    List<PointF> points = new List<PointF>();
            //    for (var index = 0; index < modelRoom.BoundaryPoints.AsReadOnly().Count; index++) {
            //        MyPoint myPoint = modelRoom.BoundaryPoints.AsReadOnly()[index];
            //        Logger.WriteLog($"MyPoint at index {index} is {myPoint}");
            //        points.Add(ConvertToFormCoordinate(myPoint));
            //    }

            //    e.Graphics.FillPolygon(Brushes.Aquamarine, points.ToArray());
            //}
        }
        //convert to offset coordinates
        //private PointF ConvertToFormCoordinate(MyPoint P) {
        //    if (P == null) return new PointF(0, 0);

        //    int x = Convert.ToInt32(P.X);
        //    int y = Convert.ToInt32(-P.Y + 600);
        //    //Logger.WriteLog($"Convert from {P} to {x},{y}");
        //    PointF myPoint = new PointF(x, y);
        //    return myPoint;
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
            foreach (MyPoint point in model.ModelPoints) {
                Points.Add(point);
            }

            foreach (MyLine line in model.modelLines) {
                Lines.Add(line);
            }

            foreach (Room room in model.modelRooms) {
                Rooms.Add(room);
            }
            testcanvas.Children.Clear();
            Logger.WriteLog("paint started");
            foreach (MyLine line in model.modelLines) {
                ShapeLine myLine = new ShapeLine();

                myLine.Stroke = System.Windows.Media.Brushes.Black;
                myLine.X1 = line.StartMyPoint.X;
                myLine.X2 = line.EndMyPoint.X;
                myLine.Y1 = line.StartMyPoint.Y;
                myLine.Y2 = line.EndMyPoint.Y;
                myLine.StrokeEndLineCap = PenLineCap.Triangle;

                myLine.StrokeStartLineCap = PenLineCap.Round;
                //myLine.HorizontalAlignment = HorizontalAlignment.Left;
                //myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 5;

                //zoomviewboxgrid.Children.Add(myLine);
                //zoomviewboxgrid2.Children.Add(myLine);
                testcanvas.Children.Add(myLine);
            }

            foreach (MyPoint point in model.ModelPoints) {
                //Rectangle myRec = new Rectangle();
                ShapeLine myLine = new ShapeLine();

                myLine.Stroke = System.Windows.Media.Brushes.Red;
                myLine.X1 = point.X;
                myLine.X2 = point.X + 1;
                myLine.Y1 = point.Y;
                myLine.Y2 = point.Y + 1;

                myLine.StrokeStartLineCap = PenLineCap.Round;
                myLine.StrokeEndLineCap = PenLineCap.Triangle;
                myLine.StrokeThickness = 5;

                testcanvas.Children.Add(myLine);


                //myLine.HorizontalAlignment = HorizontalAlignment.Left;
                //myLine.VerticalAlignment = VerticalAlignment.Center;

                //testcanvas.Children.Add(myEllipse);

                //CreateCanvasWithEllipse(200,200.0);
            }

            foreach (Room room in model.modelRooms) {
                List<MyPoint> boundaries = room.GetBoundaryPointsSorted();
                if (!boundaries.Any()) {
                    continue;
                }
                List<Point> convertedPoints = boundaries.Select(i => new Point(i.X, i.Y)).ToList();
                Polygon p = new Polygon();
                p.Points = new PointCollection(convertedPoints);
                p.Fill = new SolidColorBrush(room.type.fillColor.ToMediaColor());
                p.Opacity = 0.5;
                testcanvas.Children.Add(p);

            }
        }
        void CreateCanvasWithEllipse(double desiredLeft, double desiredTop) {
            Canvas canvas = new Canvas();
            testcanvas.Children.Add(canvas);
            ShapeEllipse ellipse = CreateEllipse(50, 50, 0, 0);
            Canvas.SetLeft(ellipse, desiredLeft);
            Canvas.SetTop(ellipse, desiredTop);
            canvas.Children.Add(ellipse);
        }

        ShapeEllipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY) {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            return ellipse;
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (actualSimulationThreshold < MaxSimulationThreshold) {
                SimulationStepMove();
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
            int splitPercentage = int.Parse("50");
            model.SplitEdge(splitPercentage, model.GetRandomLine());
            Paint();
        }


        private void SwitchRoomClick(object sender, RoutedEventArgs e)
        {
            SimulationStepSwitch();
            Paint();
        }
    }
}
