using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsFormsApp1;
using WindowsFormsApp1.GeometryModel;
using WindowsFormsApp1.Simulation;
using WindowsFormsApp1.Utilities;
using GenerateTest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Action = System.Action;
using Formatting = System.Xml.Formatting;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Logger = WindowsFormsApp1.Logger;
using MessageBox = System.Windows.MessageBox;
using ShapeEllipse = System.Windows.Shapes.Ellipse;
using ShapeLine = System.Windows.Shapes.Line;
using ShapeRectangle = System.Windows.Shapes.Rectangle;
using Model = WindowsFormsApp1.Model;
namespace UIWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public string StatusMessage { get; set; }
        public Model model { get; set; }
        public ObservableCollection<MyPoint> Points { get; set; }
        public ObservableCollection<MyLine> Lines { get; set; }
        public ObservableCollection<MyRoom> Rooms { get; set; }
        public ObservableCollection<Costs> SimulationCosts { get; set; }
        public Simulate s = new Simulate();
        public ObservableCollection<LineAndCost> LineAndCostActualStep { get; set; }

        private void Log(string message) {
            StatusMessage = message;
        }

        public ObservableCollection<RoomType> roomtypes { get; set; }
        public int LineGridSelectedIndex { get; set; }
        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;
        public int actualSimulationIndex = 0;
        readonly object locker = new object();
        public int moveDistance = 10;
        public string runPath = "";
        public MainWindow() {
            //CreateRunFolderAndInitPath();
            model = new Model();
            Points = new ObservableCollection<MyPoint>();
            Lines = new ObservableCollection<MyLine>();
            Rooms = new ObservableCollection<MyRoom>();
            SimulationCosts = new ObservableCollection<Costs>();
            LineAndCostActualStep = new ObservableCollection<LineAndCost>();

            InitRoomTypes();
            model.InitSimpleModel();
            s.model = model;
            s.ModelChanged += ModelChangeHandler;
            DataContext = this;
            InitializeComponent();
            Paint();
            LoadDataFromModel();
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
            roomtypes = new ObservableCollection<RoomType>();
            roomtypes.Add(RoomType.BedRoom);
            roomtypes.Add(RoomType.LivingRoom);
            roomtypes.Add(RoomType.RestRoom);
            roomtypes.Add(RoomType.Kitchen);
        }

        private void ModelChangeHandler(object sender, ProgressEventArgs e) {

            Dispatcher.BeginInvoke(new Action(() => {

                lock (locker) {

                    SaveStateToPng();
                    model = e.model;
                    SimulationCosts.Add(new Costs(e.simIndex, e.cost, e.areacost, e.layoutcost, 0, e.stepAction));
                    LoadDataFromModel();
                    Paint();


                }
            }), DispatcherPriority.SystemIdle);

        }



        private void SimulationStepMove() {

            Dictionary<string, double> Costs = new Dictionary<string, double>();
            MyLine minline = null;
            int currentMoveDistance = moveDistance;
            double actualCost = CostCalculationService.CalculateCost(model).First();
            double mincost = actualCost;
            Parallel.For(0, model.modelLines.Count,
                index => {
                    MyLine myLine = model.modelLines.ElementAt(index);
                    MyLine newMyLine = null;
                    Model tempModel = model.DeepCopy(myLine, out newMyLine);
                    tempModel.MoveLine(moveDistance, newMyLine);

                    double cost = CostCalculationService.CalculateCost(tempModel).First();
                    lock (locker) {
                        Costs.Add("+" + myLine.ToString(), cost);
                        if (mincost > cost) {
                            mincost = cost;
                            minline = myLine;
                            currentMoveDistance = moveDistance;
                        }
                    }
                });

            Parallel.For(0, model.modelLines.Count,
                index => {
                    MyLine myLine = model.modelLines.ElementAt(index);
                    MyLine newMyLine = null;
                    Model tempModel = model.DeepCopy(myLine, out newMyLine);
                    tempModel.MoveLine(-moveDistance, newMyLine);

                    double cost = CostCalculationService.CalculateCost(tempModel).First();
                    lock (locker) {
                        Costs.Add("-" + myLine.ToString(), cost);
                        if (mincost > cost) {
                            mincost = cost;
                            minline = myLine;
                            currentMoveDistance = -moveDistance;
                        }
                    }
                });

            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }
            if (minline != null) {
                model.MoveLine(currentMoveDistance, minline);
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

            double[] costArray = CostCalculationService.CalculateCost(model);
            SimulationCosts.Add(new Costs(actualSimulationIndex, costArray[0], costArray[1], costArray[2], costArray[3]));

            actualSimulationIndex++;
        }
        private void SimulationStepSwitch() {
            Dictionary<MyRoom, double> RoomCosts = new Dictionary<MyRoom, double>();

            double actualCost = CostCalculationService.CalculateCost(model).First();
            double mincost = actualCost;
            int rooms = model.modelRooms.Count;
            MyRoom switchThisMyRoomFrom = null;
            MyRoom switchThisMyRoomTo = null;
            Parallel.For(0, rooms,
                index => {
                    Parallel.For(index + 1, rooms, secondindex => {

                        MyRoom r1 = model.modelRooms.ElementAt(index);
                        MyRoom r2 = model.modelRooms.ElementAt(secondindex);
                        MyRoom r1target = null;
                        MyRoom r2target = null;
                        Model tempModel = model.DeepCopy(r1, r2, out r1target, out r2target);
                        tempModel.SwitchRooms(ref r1target, ref r2target);

                        double cost = CostCalculationService.CalculateCost(tempModel).First();
                        lock (locker) {
                            RoomCosts.Add(r1, cost);
                            if (mincost >= cost) {
                                mincost = cost;
                                //this might need to be switched later
                                switchThisMyRoomFrom = r1;
                                switchThisMyRoomTo = r2;
                            }
                        }
                    });
                });

            if (mincost >= actualCost) {
                actualSimulationThreshold++;
            }


            if (switchThisMyRoomFrom != null && switchThisMyRoomTo != null) {

                model.SwitchRooms(ref switchThisMyRoomFrom, ref switchThisMyRoomTo);
            }
            else {
                MessageBox.Show("no room to switch");
            }

            double[] costArray = CostCalculationService.CalculateCost(model);

            SimulationCosts.Add(new Costs(actualSimulationIndex, costArray[0], costArray[1], costArray[2], costArray[3]));
            actualSimulationIndex++;
        }

        private bool isPainting = false;

        private void Paint() {
            AutoScrollCosts();
            isPainting = true;
            //LoadDataFromModel();
            testcanvas.Children.Clear();
            //DrawAxis(testcanvas);

            Logger.WriteLog("paint started");
            for (var i = 0; i < model.modelLines.Count; i++) {
                MyLine line = model.modelLines[i];
                ShapeLine myLine = new ShapeLine();
                Brush solidColorBrush = new SolidColorBrush(Color.FromArgb(95, 0, 0, 0));
                solidColorBrush.Opacity = 0.5;
                if (i.Equals(selectedLineIndex)) {
                    solidColorBrush = Brushes.Yellow;
                }

                myLine.Stroke = solidColorBrush;
                myLine.X1 = line.StartMyPoint.X;
                myLine.X2 = line.EndMyPoint.X;
                myLine.Y1 = line.StartMyPoint.Y;
                myLine.Y2 = line.EndMyPoint.Y;
                myLine.StrokeEndLineCap = PenLineCap.Triangle;
                myLine.StrokeStartLineCap = PenLineCap.Round;
                myLine.StrokeThickness = 10;
                myLine.ToolTip = line.ToString();
                testcanvas.Children.Add(myLine);
            }

            for (var i = 0; i < model.ModelPoints.Count; i++) {
                MyPoint point = model.ModelPoints[i];
                ShapeLine myLine = new ShapeLine();

                var solidColorBrush = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
                solidColorBrush.Opacity = 0.5;
                if (i.Equals(selectedPointIndex)) {
                    solidColorBrush = Brushes.GreenYellow;
                }

                myLine.Stroke = solidColorBrush;
                myLine.X1 = point.X;
                myLine.X2 = point.X + 1;
                myLine.Y1 = point.Y;
                myLine.Y2 = point.Y + 1;
                myLine.StrokeStartLineCap = PenLineCap.Round;
                myLine.StrokeEndLineCap = PenLineCap.Triangle;
                myLine.StrokeThickness = 10;
                myLine.ToolTip = point.ToString();
                testcanvas.Children.Add(myLine);
            }

            //foreach (MyRoom room in model.modelRooms) {
            foreach (MyRoom room in Rooms) {
                List<MyPoint> boundaries = room.GetBoundaryPointsSorted();
                if (!boundaries.Any()) continue;
                //boundaries.RemoveAll(item => item == null); //this is error handling, but I would need to figure out why nulls exist
                List<Point> convertedPoints = boundaries.Select(i => new Point(i.X, i.Y)).ToList();
                Polygon p = new Polygon();
                p.Points = new PointCollection(convertedPoints);
                p.Fill = new SolidColorBrush(room.type.fillColor.ToMediaColor());
                p.Opacity = 0.25;
                p.ToolTip = room.ToString();
                testcanvas.Children.Add(p);
            }

            isPainting = false;
        }

        private void DrawAxis(Canvas canvas) {
            Logger.WriteLog("paint started");
            for (var i = -400; i < 401; i += 10) {
                ShapeLine myLine = new ShapeLine();
                Brush solidColorBrush = new SolidColorBrush(Color.FromArgb(95, 250, 250, 250));
                if (i != 0) {
                    solidColorBrush.Opacity = 0.5;
                    if (i % 100 == 0) {
                        solidColorBrush.Opacity = 0.75;
                    }
                }

                myLine.Stroke = solidColorBrush;
                myLine.X1 = 0 + i;
                myLine.X2 = 0 + i;
                myLine.Y1 = -400;
                myLine.Y2 = 400;
                myLine.StrokeThickness = 1;
                myLine.ToolTip = i.ToString();
                testcanvas.Children.Add(myLine);
            }
            for (var i = -400; i < 401; i += 10) {
                ShapeLine myLine = new ShapeLine();
                Brush solidColorBrush = new SolidColorBrush(Color.FromArgb(95, 250, 250, 250));
                if (i != 0) {
                    solidColorBrush.Opacity = 0.15;
                    if (i % 100 == 0) {
                        solidColorBrush.Opacity = 0.25;
                    }
                }

                myLine.Stroke = solidColorBrush;
                myLine.X1 = -400;
                myLine.X2 = 400;
                myLine.Y1 = 0 + i;
                myLine.Y2 = 0 + i;
                myLine.StrokeThickness = 2;
                myLine.ToolTip = i.ToString();
                testcanvas.Children.Add(myLine);
            }
        }

        private void LoadDataFromModel() {
            //Points.Clear();
            //Lines.Clear();
            //Rooms.Clear();
            //Points.Clear();
            //Lines.Clear();
            //Rooms.Clear();


            List<MyPoint> common = Points.Intersect(model.ModelPoints).ToList();
            List<MyPoint> diff = model.ModelPoints.Except(common).ToList();
            List<MyPoint> diffBAD = Points.Except(model.ModelPoints).ToList();

            foreach (MyPoint point in diffBAD) {
                Points.Remove(point);
            }
            foreach (MyPoint point in diff) {
                Points.Add(point);
            }

            List<MyLine> commonL = Lines.Intersect(model.modelLines).ToList();
            List<MyLine> diffL = model.modelLines.ToList().Except(commonL).ToList();
            List<MyLine> diffLBAD = Lines.Except(model.modelLines).ToList();

            foreach (MyLine line in diffLBAD) {
                Lines.Remove(line);

            }
            foreach (MyLine line in diffL) {
                Lines.Add(line);
            }

            List<MyRoom> commonR = Rooms.Intersect(model.modelRooms).ToList();
            List<MyRoom> diffR = model.modelRooms.ToList().Except(commonR).ToList();
            List<MyRoom> diffRBAD = Rooms.Except(model.modelRooms).ToList();

            foreach (MyRoom room in diffRBAD) {
                Rooms.Remove(room);
            }

            foreach (MyRoom room in diffR) {
                Rooms.Add(room);
            }

            //foreach (MyLine line in model.modelLines) {
            //    Lines.Add(line);
            //}

            //foreach (MyRoom room in model.modelRooms) {
            //    Rooms.Add(room);
            //}
        }

        private void AutoScrollCosts() {
            if (CostGrid.Items.Count > 0) {
                var border = VisualTreeHelper.GetChild(CostGrid, 0) as Decorator;
                if (border != null) {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        private void MoveWallClick(object sender, RoutedEventArgs e) {
            //if (actualSimulationThreshold < MaxSimulationThreshold) {
            //    SimulationStepMove();
            //    Paint();
            //}
            //else {
            //    MessageBox.Show("Simulation threshold exit. Optimum reached.");
            //}
            s.Move(LineGrid.SelectedItem as MyLine, 10);
            Paint();
            //model.MoveLine(10, LineGrid.SelectedItem as MyLine);
            //Paint();
        }
        private void MoveWallClick2(object sender, RoutedEventArgs e) {
            s.Move(LineGrid.SelectedItem as MyLine, -10);
            //model.MoveLine(-10, LineGrid.SelectedItem as MyLine);
            Paint();
        }
        private void StartSimulationClick(object sender, RoutedEventArgs e) {
            //model = new Model();
            //model.InitSimpleModel();
            s.model = model;
            Paint();

            Thread t = new Thread(s.run);
            t.Start();

        }
        private void ReStartSimulationClick(object sender, RoutedEventArgs e) {
            model = new Model();
            model.InitSimpleModel();
            s.model = model;
            Paint();

            Thread t = new Thread(s.run);
            t.Start();

        }

        private void SplitWallClick(object sender, RoutedEventArgs e) {
            int splitPercentage = int.Parse("50");
            //model.SplitEdge(splitPercentage, model.GetRandomLine());
            s.Split(splitPercentage, LineGrid.SelectedItem as MyLine);
            //int index = LineGrid.SelectedIndex;
            //model.SplitEdge(splitPercentage, LineGrid.SelectedItem as MyLine);
            //Paint();
        }
        private void SwitchRoomClick(object sender, RoutedEventArgs e) {
            //SimulationStepSwitch();
            int roomcount = s.model.modelRooms.Count;
            if (roomcount >= 2) {
                Random r = new Random(10);
                MyRoom r1 = s.model.modelRooms.ElementAt(0);//r.Next(s.model.modelRooms.Count));
                MyRoom r2 = s.model.modelRooms.ElementAt(1);//r.Next(s.model.modelRooms.Count));

                s.SwitchRoom(ref r1, ref r2);
            }
            Paint();
        }

        private int selectedLineIndex = -1;
        private int selectedPointIndex = -1;


        private void LineGrid_OnCurrentCellChanged(object sender, EventArgs e) {
            //selectedLineIndex = LineGrid.SelectedIndex;
            //MessageBox.Show(selectedLineIndex.ToString());

        }

        private void LineGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!isPainting) {
                selectedLineIndex = LineGrid.SelectedIndex;
                Paint();

            }

            //LineGrid.SelectedIndex = selectedLineIndex;
            //MessageBox.Show("selectionchanged" + selectedLineIndex.ToString());
        }
        private void PointGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!isPainting) {
                selectedPointIndex = PointGrid.SelectedIndex;
                Paint();

            }
        }

        private void CostGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void CostGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            int actualCostIndex = CostGrid.SelectedIndex;
            Model requested = s.modelCopyHistory.ElementAt(actualCostIndex);
            model = requested;
            Paint();
        }

        private void LoadSelectedClick(object sender, RoutedEventArgs e) {
            int actualCostIndex = CostGrid.SelectedIndex;
            Model requested = s.modelCopyHistory.ElementAt(actualCostIndex);
            model = requested;
            s.model = requested;
            Paint();
        }

        private void LoadSimpleModelClick(object sender, RoutedEventArgs e) {
            s.model.InitSimpleModel();
        }

        private void LoadNormalModelClick(object sender, RoutedEventArgs e) {
            s.model.InitNormalModel();
        }

        private void LoadSkewedModelClick(object sender, RoutedEventArgs e) {
            s.model.InitSkewedModel();
        }

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                this.Close();
            }
            if (e.Key == Key.F10) {
                s.Move(LineGrid.SelectedItem as MyLine, 10);
            }
            if (e.Key == Key.F11) {
                s.Move(LineGrid.SelectedItem as MyLine, -10);
            }
        }

        private bool isStopped = false;
        private void StopClick(object sender, RoutedEventArgs e) {
            isStopped = !isStopped;
            s.IsStopped = isStopped;

        }

        private void LoadSimplestModelClick(object sender, RoutedEventArgs e) {
            s.model.InitSimplestModel();
        }

        private void LoadAdvancedModelClick(object sender, RoutedEventArgs e) {
            s.model.InitAdvancedModel();
        }



        private void SaveStateToPng() {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)MainFormWindow.RenderSize.Width,
                (int)MainFormWindow.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(MainFormWindow);

            //var crop = new CroppedBitmap(rtb, new Int32Rect(50, 50, 250, 250));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            if (runPath == "") {
                CreateRunFolderAndInitPath();
            }
            var path = runPath + DateTime.Now.ToString("HH_mm_ss_fff") + ".png";
            using (var fs = System.IO.File.OpenWrite(path)) {
                pngEncoder.Save(fs);
            }
        }





        /// <summary>
        /// Take screenshot of a Window.
        /// </summary>
        /// <remarks>
        /// - Usage example: screenshot icon in every window header.                
        /// - Keep well away from any Windows Forms based methods that involve screen pixels. You will run into scaling issues at different
        ///   monitor DPI values. Quote: "Keep in mind though that WPF units aren't pixels, they're device-independent @ 96DPI
        ///   "pixelish-units"; so really what you want, is the scale factor between 96DPI and the current screen DPI (so like 1.5 for
        ///   144DPI) - Paul Betts."
        /// </remarks>
        public async Task<bool> TryScreenshotToClipboardAsync(FrameworkElement frameworkElement) {
            frameworkElement.ClipToBounds = true; // Can remove if everything still works when the screen is maximised.

            Rect relativeBounds = VisualTreeHelper.GetDescendantBounds(frameworkElement);
            double areaWidth = frameworkElement.RenderSize.Width; // Cannot use relativeBounds.Width as this may be incorrect if a window is maximised.
            double areaHeight = frameworkElement.RenderSize.Height; // Cannot use relativeBounds.Height for same reason.
            double XLeft = relativeBounds.X;
            double XRight = XLeft + areaWidth;
            double YTop = relativeBounds.Y;
            double YBottom = YTop + areaHeight;
            var bitmap = new RenderTargetBitmap((int)Math.Round(XRight, MidpointRounding.AwayFromZero),
                                                (int)Math.Round(YBottom, MidpointRounding.AwayFromZero),
                                                96, 96, PixelFormats.Default);

            // Render framework element to a bitmap. This works better than any screen-pixel-scraping methods which will pick up unwanted
            // artifacts such as the taskbar or another window covering the current window.
            var dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen()) {
                var vb = new VisualBrush(frameworkElement);
                ctx.DrawRectangle(vb, null, new Rect(new Point(XLeft, YTop), new Point(XRight, YBottom)));
            }
            bitmap.Render(dv);
            return await TryCopyBitmapToClipboard(bitmap);
        }

        private static async Task<bool> TryCopyBitmapToClipboard(BitmapSource bmpCopied) {
            var tries = 3;
            while (tries-- > 0) {
                try {
                    // This must be executed on the calling dispatcher.
                    System.Windows.Clipboard.SetImage(bmpCopied);
                    return true;
                }
                catch (COMException) {
                    // Windows clipboard is optimistic concurrency. On fail (as in use by another process), retry.
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }
            return false;
        }

        private void SaveStateClick(object sender, RoutedEventArgs e) {
            SaveStateToPng();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e) {

        }

        private void LoadFactoryModelClick(object sender, RoutedEventArgs e) {
            s.model.InitModelWithGivenRooms();
        }


        private void LoadTestModelClick(object sender, RoutedEventArgs e) {
            s.model.InitTestModel();
        }


        ModelStorage ms = new ModelStorage();
        public void AllCasesTestGeneration() {
            Model m = new Model();
            m.InitTestModel();

            List<Model> newModelsBeforeStep = new List<Model>() { m };

            List<Model> allModels = new List<Model>();
            int i = 0;
            while (newModelsBeforeStep.Any() && i <10) {
                List<Model> newModelsInStep = new List<Model>();
                foreach (Model model in newModelsBeforeStep) {
                    List<Model> joinedModels = MergeAllRoomPairs(model);

                    foreach (Model currentModel in joinedModels) {
                        if (currentModel.modelRooms.Count > 1) {
                            newModelsInStep.Add(currentModel);
                        }
                    }

                    allModels.AddRange(joinedModels);
                }

                newModelsBeforeStep = newModelsInStep;
                i++;
            }

            MessageBox.Show(allModels.Count.ToString());
            ommitstepcount = 0;
            foreach (Model model in allModels)
            {
                Ommitsteps(model);
                ommitstepcount++;
                //if (ommitstepcount%10==0)
                //{
                //    MessageBox.Show(ommitstepcount.ToString());
                //}
            }
            string t = DateTime.Now.ToString("");
            //foreach (Model m1 in ms.getHistory()) {
            //    SaveHistoryModel(m1, GenerateModelNameFromState(m1));
            //}
            this.Close();
        }

        private string GenerateModelNameFromState(Model currentModel) {
            return
                $"Modell_{string.Join("-",currentModel.modelRooms.Select(i=>i.Number))}_{currentModel.modelLines.Count}_{DateTime.Now.ToString("hh-mm-ss-fff")}";
        }

        List<int> fullist = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public List<Model> MergeAllRoomPairs(Model m_mod) {
            int missinglargest = 0;
            m_mod.CalculateAllRooms();
            IEnumerable<int> enumerable = fullist.Except(m_mod.modelRooms.Select(i => i.GetNumberAsInt()));
            if (enumerable.Any()) {
                missinglargest = enumerable.Max();

            }
            //missinglargest = m_mod.modelRooms.Select(i => int.Parse(i.Number)).Union(fullist).Max();
            List<Model> returnList = new List<Model>();
            for (var i = 0; i < m_mod.modelRooms.Count; i++) {
                MyRoom room = m_mod.modelRooms[i];
                if (room.GetNumberAsInt() < missinglargest) continue;

                for (var j = i + 1; j < m_mod.modelRooms.Count; j++) {
                    MyRoom modelRoom = m_mod.modelRooms[j];
                    if (modelRoom.GetNumberAsInt() < missinglargest) continue;
                    //if (room2.Guid == room1.Guid) continue;

                    bool a = DoTheyHaveCommmonWall(room, modelRoom);
                    if (!a) continue;

                    else {
                        MyRoom room2;
                        MyRoom modelRoom2;
                        Model m_mod2 = m_mod.DeepCopy(room, modelRoom, out room2, out modelRoom2);

                        Model newModel = MergeRooms(m_mod2, room2, modelRoom2);
                        returnList.Add(newModel);
                    }
                }
            }

            return returnList;
        }

        private Model MergeRooms(Model mMod, MyRoom room, MyRoom modelRoom) {
            //Model m = mMod.DeepCopy();
            mMod = RemoveCommonWalls(mMod, room, modelRoom);
            //MergeBoundaryLineListToSmallerIdRooms();
            return mMod;
        }

        private Model RemoveCommonWalls(Model m, MyRoom room1, MyRoom room2) {
            List<MyLine> common = room1.BoundaryLines.Intersect(room2.BoundaryLines).ToList();
            foreach (MyLine line in common) {
                room1.BoundaryLines.Remove(line);
                room2.BoundaryLines.Remove(line);
                m.modelLines.Remove(line);
                foreach (MyRoom room in m.modelRooms)
                {
                    room.BoundaryLines.Remove(line);
                }
                line.EndMyPoint.RelatedLines.Remove(line);
                line.StartMyPoint.RelatedLines.Remove(line);
            }
            room1.BoundaryLines.AddRange(room2.BoundaryLines);
            room2.BoundaryLines.AddRange(room1.BoundaryLines);

            int result1;
            int result2;
            bool parsed1 = int.TryParse(room1.Number, out result1);
            bool parsed2 = int.TryParse(room2.Number, out result2);
            if (parsed1 && parsed2 && (result1 < result2 && room2.GetNumberAsInt()!=2)) {
                m.modelRooms.Remove(room2);
                foreach (MyLine line in m.modelLines)
                {
                    line.relatedRooms.Remove(room2);
                }
            }
            else {
                m.modelRooms.Remove(room1);
                foreach (MyLine line in m.modelLines) {
                    line.relatedRooms.Remove(room1);
                }
            }

            return m;
        }


        private bool DoTheyHaveCommmonWall(MyRoom room, MyRoom modelRoom) {
            if (room.BoundaryLines.Intersect(modelRoom.BoundaryLines).Any()) return true;
            return false;
        }

        private int ommitstepcount = 0;
        public void Ommitsteps(Model m_mod) {
            GC.Collect();
            //ms.AddModel(m_mod.DeepCopy());
            Model deepCopy = m_mod.DeepCopy();
            List<Model> allModels = new List<Model>(){deepCopy};
            List<Model> lastModel = new List<Model>(){deepCopy};
            while (lastModel.Any())
            {
                List<Model>currentModels = new List<Model>();
                foreach (Model currentModel in lastModel)
                {
                    List<Model> allPossibleOneOmmits = OmmitOne(currentModel);
                    allModels.AddRange(allPossibleOneOmmits);
                    currentModels.AddRange(allPossibleOneOmmits);
                }

                lastModel = currentModels;
            }

            //MessageBox.Show(allModels.Count+" models in ommit step");
            foreach (Model allModel in allModels)
            {
                //ms.AddModel(allModel);
                SaveHistoryModel(allModel, GenerateModelNameFromState(allModel));
            }
        }

        private List<Model> OmmitOne(Model currentModel) {
            if (currentModel.modelRooms.Count<2) return new List<Model>();
            List<Model> returns = new List<Model>();
            foreach (var room in currentModel.modelRooms)
            {
                int missinglargest = 0;
                IEnumerable<int> enumerable = fullist.Except(currentModel.modelRooms.Select(i => i.GetNumberAsInt()));

                if (enumerable.Any()) {
                    missinglargest = enumerable.Max();
                }
                if (room.GetNumberAsInt() < missinglargest || room.GetNumberAsInt()==2) continue;
                
                MyRoom room2;
                Model m_mod2 = currentModel.DeepCopy(room, out room2);

                foreach (MyLine line in room2.BoundaryLines) {
                    line.relatedRooms.Remove(room2);
                }
                m_mod2.modelRooms.Remove(room2);

                foreach (MyLine line in m_mod2.modelLines)
                {
                    line.relatedRooms.Remove(room2);
                }



                List<MyLine> toremove = new List<MyLine>();
                foreach (MyLine line in m_mod2.modelLines)
                {
                    if (!line.relatedRooms.Any())
                    {
                        toremove.Add(line);
                    }
                }

                foreach (MyLine line in toremove)
                {
                    m_mod2.modelLines.Remove(line);
                    foreach (var rooms in model.modelRooms)
                    {
                        rooms.BoundaryLines.Remove(line);
                    }
                    line.StartMyPoint.RelatedLines.Remove(line);
                    line.EndMyPoint.RelatedLines.Remove(line);
                }
                returns.Add(m_mod2);
            }

            return returns;
        }



        private string savepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private void SaveHistoryModel(Model ms, string name) {
            //string data = JsonConvert.SerializeObject(jsondata);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            using (StreamWriter sw = new StreamWriter($"{savepath}\\Models\\{name}.json"))
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                serializer.Serialize(writer, ms);
            }
        }

        private void LoadFirstModel()
        {
            string path2 = @"C:\Users\Master\Desktop\Models\Modell_1-2_6_04-17-16-821.json";
            string path = @"C:\Users\Master\Desktop\Models\Modell_1-2-3-4-5-6-7-9_23_04-17-17-251.json";
            using (StreamReader r = new StreamReader(path)) {
                string json = r.ReadToEnd();
                Model items = JsonConvert.DeserializeObject<Model>(json);
                model = items;

            }
            Paint();
        }

        private void Test_OnClick(object sender, RoutedEventArgs e) {
            AllCasesTestGeneration();

        }

        private void LoadModel_OnClick(object sender, RoutedEventArgs e)
        {
            LoadFirstModel();
        }
    }



    //TODO: create serializable model class with serialiable myline, and points --- avoid self referencing
    //TODO: only incrementing join and incremeting ommit allowed
}
//TODO: create lists for the join steps - to keep track of 3,4,5 and 4,5,3 merges --- catch them sooner!!! ---  ((not nessesary)!!!!
