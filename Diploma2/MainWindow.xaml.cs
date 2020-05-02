using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Odbc;
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
using Diploma2.Views;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Action = System.Action;
using ShapeLine = System.Windows.Shapes.Line;

namespace Diploma2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {

        #region Variables

        private bool isPainting;
        private int selectedLineIndex = -1;
        public int selectedPointIndex = -1;
        public string StatusMessage { get; set; }
        public ObservableCollection<_Point> Points { get; set; } = new ObservableCollection<_Point>();
        public ObservableCollection<_Line> Lines { get; set; } = new ObservableCollection<_Line>();
        public ObservableCollection<_Room> Rooms { get; set; } = new ObservableCollection<_Room>();
        public ObservableCollection<_Model> modelHistory { get; set; } = new ObservableCollection<_Model>();
        private _Model GeneratingBase { get; set; }
        public _Model model { get; set; }
        public ObservableCollection<Cost> SimulationCosts { get; set; } = new ObservableCollection<Cost>();
        public Simulation simulation = new Simulation();
        public ObservableCollection<LineAndCost> LineAndCostActualStep { get; set; } = new ObservableCollection<LineAndCost>();
        List<int> selectedLineIndices = new List<int>();
        List<_Line> selectedLines = new List<_Line>();
        public ObservableCollection<_RoomType> roomtypes { get; set; } = new ObservableCollection<_RoomType>(_RoomType.getRoomTypes());
        public int LineGridSelectedIndex { get; set; }
        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        public int actualSimulationIndex = 0;
        readonly object locker = new object();
        public int moveDistance = 10;
        ModelStorage ms = new ModelStorage();

        #endregion
        public int SelectedLineIndex {
            get {
                return selectedLineIndex;

            }
            set {
                selectedLineIndex = value;
                OnPropertyChanged();
                Paint();
            }
        }

        public int SelectedPointIndex {
            get {
                return selectedPointIndex;

            }
            set {
                selectedPointIndex = value;
                OnPropertyChanged();
                Paint();
            }
        }

        public MainWindow() {
            DataContext = this;
            InitializeComponent();
            model = ModelConfigurations.InitSimpleModel();

            LoadDataFromModel();
            simulation.Model = model;
            simulation.ModelChanged += ModelChangeHandler;
            Paint();
        }

        private void LoadModels() {
            //Model = ModelConfigurations.InitSimplestModel();
            model = ModelConfigurations.InitSimpleModel();
            LoadDataFromModel();
        }

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

            foreach (_Room room in Rooms) {
                try {
                    List<_Point> boundaries = room.GetPoints();
                    if (!boundaries.Any()) continue;
                    List<Point> convertedPoints = boundaries.Select(i => new Point(i.X, i.Y)).ToList();
                    Polygon p = new Polygon();
                    p.Points = new PointCollection(convertedPoints);
                    p.Fill = new SolidColorBrush(room.type.fillColor.ToMediaColor());
                    p.Opacity = 0.25;
                    p.ToolTip = room.ToString();
                    testcanvas.Children.Add(p);
                }
                catch (Exception e) {
                    Logger.WriteLog(e);
                }
            }

            isPainting = false;
        }
        private void LoadDataFromModel() {
            Points = new ObservableCollection<_Point>(model.AllPointsFlat());
            Lines = new ObservableCollection<_Line>(model.AllLinesFlat());
            Rooms = model.rooms;
            LineGrid.ItemsSource = Lines;
            PointGrid.ItemsSource = Points;
            RoomGrid.ItemsSource = null;
            RoomGrid.ItemsSource = Rooms;
        }
        #region UI eventhandlers

        private void LoadModel_OnClick(object sender, RoutedEventArgs e) {
            LoadModels();
        }

        private void MoveLine_OnClick(object sender, RoutedEventArgs e) {
            if (selectedLines.FirstOrDefault() == null) {
                model.MoveLine();
            }
            else {
                model.MoveLine(10, selectedLines.FirstOrDefault());
            }
            LoadDataFromModel();
            Paint();
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

        private void RoomGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            _Room index = RoomGrid.CurrentCell.Item as _Room;
            selectedLineIndices.Clear();
            selectedLines.Clear();
            var i = 0;
            foreach (var line in Lines) {
                if (index != null && index.Lines.Contains(line)) {
                    selectedLineIndices.Add(i);
                    selectedLines.Add(line);
                }
                i++;
            }
            Paint();
            RoomGrid.SelectedIndex = -1;

        }

        private void LineGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            selectedLineIndices.Clear();
            selectedLines.Clear();
            var ined = LineGrid.SelectedIndex;
            if (ined != -1) {
                selectedLineIndices.Add(ined);
                selectedLines.Add(Lines.ElementAt(ined));
                Paint();

            }
        }

        private void PointGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            SelectedPointIndex = PointGrid.SelectedIndex;
        }
        private void StartSimulation_OnClick(object sender, RoutedEventArgs e) {
            simulation.Model = model;
            Paint();

            Thread t = new Thread(simulation.RunSteps);
            t.Start();
        }
        private void UndoStep_OnClick(object sender, RoutedEventArgs e) {
            simulation.UndoStep();
        }


        private void SaveToJson_OnClick(object sender, RoutedEventArgs e) {
            SaveHistoryModel(model, GenerateModelNameFromState(model));
        }

        private void LoadFromJson_OnClick(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = $"{savepath}\\Models";
            ofd.ShowDialog();

            string openthis = ofd.FileName;
            string json = File.ReadAllText(openthis);
            _Model account = JsonConvert.DeserializeObject<_Model>(json);
            model = account;
            LoadDataFromModel();
            Paint();
        }

        #region GenerateAllModels
        private void Gentest_OnClick(object sender, RoutedEventArgs e) {
            GeneratingBase = ModelConfigurations.InitTestModel();
            List<_Model> new_Models = new List<_Model>();
            new_Models.Add(GeneratingBase);
            List<_Model> all_Models = new List<_Model>();
            while (new_Models.Any()) {
                List<_Model> loop = new List<_Model>();
                foreach (_Model _Model in new_Models) {
                    List<_Model> current_Models = AllRoomPairs(_Model);

                    foreach (_Model current_Model in current_Models) {
                        if (current_Model.rooms.Count > 1) {
                            loop.Add(current_Model);
                        }
                    }

                    all_Models.AddRange(current_Models);
                }

                new_Models = loop;
                GC.Collect();
            }

            foreach (_Model _Model in all_Models) {
                Ommitsteps(_Model);
            }

            foreach (_Model m1 in ms.getHistory()) {
                SaveHistoryModel(m1, GenerateModelNameFromState(m1));
            }
        }
        private string GenerateModelNameFromState(_Model currentModel) {
            return
                $"{currentModel.loadedModelType}_{currentModel.rooms.Count}_" +
                $"{currentModel.AllLinesFlat().Count}";
        }

        public List<_Model> AllRoomPairs(_Model m_mod) {
            List<_Model> returnList = new List<_Model>();
            for (var i = 0; i < m_mod.rooms.Count; i++) {
                _Room room = m_mod.rooms[i];
                for (var j = i + 1; j < m_mod.rooms.Count; j++) {
                    _Room modelRoom = m_mod.rooms[j];
                    //if (room2.Guid == room1.Guid) continue;

                    bool a = DoTheyHaveCommmonWall(room, modelRoom);
                    if (!a) continue;

                    else {
                        _Room room2;
                        _Room modelRoom2;
                        _Model m_mod2 = m_mod.DeepCopy(room, modelRoom, out room2, out modelRoom2);

                        _Model newModel = MergeRooms(m_mod2, room2, modelRoom2);
                        returnList.Add(newModel);
                    }
                }
            }

            return returnList;
        }

        private _Model MergeRooms(_Model mMod, _Room room, _Room modelRoom) {
            //Model m = mMod.DeepCopy();
            mMod = RemoveCommonWalls(mMod, room, modelRoom);
            //MergeBoundaryLineListToSmallerIdRooms();
            return mMod;
        }

        private _Model RemoveCommonWalls(_Model m, _Room room1, _Room room2) {
            List<_Line> common = room1.Lines.Intersect(room2.Lines).ToList();
            foreach (_Line line in common) {
                room1.Lines.Remove(line);
                room2.Lines.Remove(line);
            }
            room1.Lines.AddRange(room2.Lines);
            room2.Lines.AddRange(room1.Lines);

            int result1 = room1.Number;
            int result2 = room2.Number;
            if (result1 < result2) {
                m.rooms.Remove(room2);
            }
            else {
                m.rooms.Remove(room1);
            }

            return m;
        }


        private bool DoTheyHaveCommmonWall(_Room room, _Room modelRoom) {
            if (room.Lines.Intersect(modelRoom.Lines).Any()) return true;
            return false;
        }


        public void Ommitsteps(_Model m_mod) {
            ms.AddModel(m_mod.DeepCopy());

            if (ExitCondition(m_mod)) return;

            Ommit(m_mod.DeepCopy());

        }

        private void Ommit(_Model mMod) {
            foreach (_Room room in mMod.rooms) {
                _Room room2;
                _Model m_mod2 = mMod.DeepCopy(room, out room2);

                m_mod2.rooms.Remove(room2);
                Ommitsteps(m_mod2);
            }

        }

        private bool ExitCondition(_Model model) {
            if (GeneratingBase.rooms.Count == 1) return true;
            else return false;
        }

        private string savepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private void SaveHistoryModel(_Model ms, string name) {
            //string data = JsonConvert.SerializeObject(jsondata);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Include;

            using (StreamWriter sw = new StreamWriter($"{savepath}\\Models\\{name}.json"))
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                serializer.Serialize(writer, ms);
            }
        }

        #endregion
        #endregion

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void ModelChangeHandler(object sender, ProgressEventArgs e) {

            Dispatcher.BeginInvoke(new Action(() => {

                lock (locker) {

                    //SaveStateToPng();
                    modelHistory.Add(model);
                    model = e.model;
                    SimulationCosts.Add(new Cost(e.simIndex, e.cost, e.areacost, e.layoutcost, 0, e.stepAction));
                    LoadDataFromModel();
                    Paint();
                }
            }), DispatcherPriority.Normal); //changed from idle

        }

        public void Draw()
        {
            double[] sum = SimulationCosts.Select(i => i.SummaryCost).ToArray();
            double[] area = SimulationCosts.Select(i => i.AreaCost).ToArray();
            double[] layout = SimulationCosts.Select(i => i.LayoutCost).ToArray();
            string[] index = SimulationCosts.Select(i => i.Index.ToString()).ToArray();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "SummaryCosts",
                    //Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                    Values =  sum.AsChartValues()
                },
                new LineSeries
                {
                    Title = "AreaCosts",
                    //Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
                    Values = area.AsChartValues(),
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "LayoutCosts",
                    //Values = new ChartValues<double> { 4,2,7,2,7 },
                    Values =layout.AsChartValues(),
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 15
                }
            };

            //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            Labels = index;
            YFormatter = value => value.ToString("C");
        }

        #region Mandatory stuff

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion

        private void DrawChart_OnClick(object sender, RoutedEventArgs e)
        {
            Draw();
            Chart.Series = SeriesCollection;
        }
    }
}
