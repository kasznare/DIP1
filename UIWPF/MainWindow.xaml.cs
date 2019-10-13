﻿using System;
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
using WindowsFormsApp1.Simulation;
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
        public Simulate s = new Simulate();
        public ObservableCollection<LineAndCost> LineAndCostActualStep { get; set; }

        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        public int actualSimulationIndex = 0;
        readonly object locker = new object();
        public int moveDistance = 10;
        public MainWindow() {
            model = new Model();
            Points = new ObservableCollection<MyPoint>();
            Lines = new ObservableCollection<MyLine>();
            Rooms = new ObservableCollection<Room>();
            SimulationCosts = new ObservableCollection<Costs>();
            LineAndCostActualStep = new ObservableCollection<LineAndCost>();
            model.InitModel();
            DataContext = this;
            InitializeComponent();
            Paint();
            s.ModelChanged += ModelChangeHandler;
        }

        private void ModelChangeHandler(object sender, ProgressEventArgs e) {
            model = e.Status;
            Paint();
        }

        private void SimulationStepMove() {

            Dictionary<MyLine, double> Costs = new Dictionary<MyLine, double>();
            MyLine minline = null;

            double actualCost = model.CalculateCost().First();
            double mincost = actualCost;
            Parallel.For(0, model.modelLines.Count,
                index => {
                    MyLine myLine = model.modelLines.ElementAt(index);
                    MyLine newMyLine = null;
                    Model tempModel = model.DeepCopy(myLine, out newMyLine);
                    tempModel.MoveLine(moveDistance, newMyLine);

                    double cost = tempModel.CalculateCost().First();
                    lock (locker) {
                        Costs.Add(myLine, cost);
                        if (mincost > cost) {
                            mincost = cost;
                            minline = myLine;
                        }
                    }
                });
            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }
            if (minline != null) {
                model.MoveLine(moveDistance, minline);
            }
            else {
                MessageBox.Show("no line to move");
            }
            LineAndCostActualStep.Clear();
            foreach (var item in Costs) {
                LineAndCostActualStep.Add(new LineAndCost(item.Key, item.Value, actualSimulationIndex));
            }
            //System.Windows.Forms.MessageBox.Show(mincost.ToString());
            //SimulationCosts.Add(new Costs(actualSimulationIndex, mincost));

            double[] costArray = model.CalculateCost();
            SimulationCosts.Add(new Costs(actualSimulationIndex, costArray[0], costArray[1], costArray[2], costArray[3]));

            actualSimulationIndex++;
        }
        private void SimulationStepSwitch() {
            Dictionary<Room, double> RoomCosts = new Dictionary<Room, double>();

            double actualCost = model.CalculateCost().First();
            double mincost = actualCost;
            int rooms = model.modelRooms.Count;
            Room switchThisRoomFrom = null;
            Room switchThisRoomTo = null;
            Parallel.For(0, rooms,
                index => {
                    Parallel.For(index + 1, rooms, secondindex => {

                        Room r1 = model.modelRooms.ElementAt(index);
                        Room r2 = model.modelRooms.ElementAt(secondindex);
                        Room r1target = null;
                        Room r2target = null;
                        Model tempModel = model.DeepCopy(r1, r2, out r1target, out r2target);
                        tempModel.SwitchRooms(ref r1target, ref r2target);

                        double cost = tempModel.CalculateCost().First();
                        lock (locker) {
                            RoomCosts.Add(r1, cost);
                            if (mincost >= cost) {
                                mincost = cost;
                                //this might need to be switched later
                                switchThisRoomFrom = r1;
                                switchThisRoomTo = r2;
                            }
                        }
                    });
                });

            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }


            if (switchThisRoomFrom != null && switchThisRoomTo != null) {

                model.SwitchRooms(ref switchThisRoomFrom, ref switchThisRoomTo);
            }
            else {
                MessageBox.Show("no room to switch");
            }

            double[] costArray = model.CalculateCost();

            SimulationCosts.Add(new Costs(actualSimulationIndex, costArray[0], costArray[1], costArray[2], costArray[3]));
            actualSimulationIndex++;
        }

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
                myLine.StrokeThickness = 5;
                testcanvas.Children.Add(myLine);
            }

            foreach (MyPoint point in model.ModelPoints) {
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
            }

            foreach (Room room in model.modelRooms) {
                List<MyPoint> boundaries = room.GetBoundaryPointsSorted();
                if (!boundaries.Any()) continue;

                List<Point> convertedPoints = boundaries.Select(i => new Point(i.X, i.Y)).ToList();
                Polygon p = new Polygon();
                p.Points = new PointCollection(convertedPoints);
                p.Fill = new SolidColorBrush(room.type.fillColor.ToMediaColor());
                p.Opacity = 0.5;
                testcanvas.Children.Add(p);
            }
        }
        private void MoveWallClick(object sender, RoutedEventArgs e) {
            if (actualSimulationThreshold < MaxSimulationThreshold) {
                SimulationStepMove();
                Paint();
            }
            else {
                MessageBox.Show("Simulation threshold exit. Optimum reached.");
            }
        }
        private void StartSimulationClick(object sender, RoutedEventArgs e) {
            model = new Model();
            model.InitModel();
            Paint();

            s.run();
        }
        private void SplitWallClick(object sender, RoutedEventArgs e) {
            int splitPercentage = int.Parse("50");
            model.SplitEdge(splitPercentage, model.GetRandomLine());
            Paint();
        }
        private void SwitchRoomClick(object sender, RoutedEventArgs e) {
            SimulationStepSwitch();
            Paint();
        }
    }
}
