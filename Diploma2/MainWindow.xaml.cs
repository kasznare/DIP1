using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Diploma2.Annotations;
using Diploma2.Model;
using Diploma2.Services;
using Diploma2.Utilities;
using Action = System.Action;
using ShapeLine = System.Windows.Shapes.Line;

namespace Diploma2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        private bool isPainting;
        private int selectedLineIndex = -1;
        public int selectedPointIndex = -1;
        public string StatusMessage { get; set; }
        public ObservableCollection<_Point> Points { get; set; } = new ObservableCollection<_Point>();
        public ObservableCollection<_Line> Lines { get; set; }= new ObservableCollection<_Line>();
        public ObservableCollection<_Room> Rooms { get; set; } = new ObservableCollection<_Room>();
        public _Model model { get; set; }
        public ObservableCollection<Costs> SimulationCosts { get; set; } = new ObservableCollection<Costs>();
        public Simulate s = new Simulate();
        public ObservableCollection<LineAndCost> LineAndCostActualStep { get; set; } = new ObservableCollection<LineAndCost>();

        public ObservableCollection<_RoomType> roomtypes { get; set; } = new ObservableCollection<_RoomType>();
        public int LineGridSelectedIndex { get; set; }
        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        public int actualSimulationIndex = 0;
        readonly object locker = new object();
        public int moveDistance = 10;
        public string runPath = "";
        public int SelectedLineIndex
        {
            get
            {
                return selectedLineIndex;

            }
            set
            {
                selectedLineIndex = value;
                OnPropertyChanged();
                Paint();
            }
        }

        public int SelectedPointIndex
        {
            get
            {
                return selectedPointIndex;

            }
            set
            {
                selectedPointIndex = value;
                OnPropertyChanged();
                Paint();


            }
        }

        private void Log(string message) {
            if (message != null && message != StatusMessage) StatusMessage = message;
        }
        public MainWindow()
        {
            DataContext = this;
            InitRoomTypes();
            InitializeComponent();
            model = ModelConfigurations.InitSimpleModel();
            LoadDataFromModel();
            s.model = model;
            s.ModelChanged += ModelChangeHandler;
            Paint();
        }

        private void LoadModels() {
            //model = ModelConfigurations.InitSimplestModel();
            model = ModelConfigurations.InitNormalModel();
            LoadDataFromModel();
        }

        List<int> selectedLineIndices = new List<int>();

        private void Paint() {
            isPainting = true;
            testcanvas.Children.Clear();

            var allLinesFlat = model.AllLinesFlat();
            for (var i = 0; i < allLinesFlat.Count; i++) {
                _Line line = allLinesFlat[i];
                ShapeLine _line = new ShapeLine();
                Brush solidColorBrush = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
                solidColorBrush.Opacity = 0.5;
                if (selectedLineIndices.Contains(i)) {
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
                if (i.Equals(SelectedPointIndex)) {
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
                List<_Point> boundaries = room.GetPoints();
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
            LoadDataFromModel();
            Paint();
        }

        private void LoadDataFromModel()
        {
            Points = new ObservableCollection<_Point>(model.AllPointsFlat());
            Lines = new ObservableCollection<_Line>(model.AllLinesFlat());
            Rooms = model.rooms;
            LineGrid.ItemsSource = Lines;
            PointGrid.ItemsSource = Points;
            RoomGrid.ItemsSource = null;
            RoomGrid.ItemsSource = Rooms;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RoomGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            _Room index = RoomGrid.CurrentCell.Item as _Room;
            selectedLineIndices.Clear();
            var i = 0;
            foreach (var line in Lines)
            {
                if (index.Lines.Contains(line))
                {
                    selectedLineIndices.Add(i);
                }

                i++;
            }
            //e.AddedCells.Select(i => i.Item as _Room);
            Paint();
            RoomGrid.SelectedIndex = -1;

        }

        private void LineGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedLineIndices.Clear();
            selectedLineIndices.Add(LineGrid.SelectedIndex);
            Paint();
        }

        private void PointGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SelectedPointIndex = PointGrid.SelectedIndex;

        }

        private void CreateRunFolderAndInitPath() {

            runPath = $@"C:\Users\{Environment.UserName}\Documents\DIP1\Screens\{DateTime.Now:yy-MM-dd-hh-ss-tt}_{model.loadedModelType}\";
            try {
                Directory.CreateDirectory(runPath);
            }
            catch (Exception e) {
                Logger.WriteLog("Directory can not be created, it already exists");
            }
        }

        private void InitRoomTypes() {
            roomtypes.Add(_RoomType.BedRoom);
            roomtypes.Add(_RoomType.LivingRoom);
            roomtypes.Add(_RoomType.RestRoom);
            roomtypes.Add(_RoomType.Kitchen);
        }
        private void ModelChangeHandler(object sender, ProgressEventArgs e) {

            Dispatcher.BeginInvoke(new Action(() => {

                lock (locker) {

                    //SaveStateToPng();
                    model = e.model;
                    SimulationCosts.Add(new Costs(e.simIndex, e.cost, e.areacost, e.layoutcost, 0, e.stepAction));
                    LoadDataFromModel();
                    Paint();


                }
            }), DispatcherPriority.SystemIdle);

        }

        private void StartSimulation_OnClick(object sender, RoutedEventArgs e)
        {
            s.model = model;
            Paint();

            Thread t = new Thread(s.run);
            t.Start();
        }
    }
}
