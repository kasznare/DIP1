using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ONLAB2;

namespace WindowsFormsApp1 {
    public partial class SimulationForm : Form {
        Model model = new Model();
        private Graphics g;
        public SimulationForm() {
            InitializeComponent();
            model.InitModel();
            g = this.CreateGraphics();
        }

        private int actualSimulationThreshold = 0;
        private int MaxSimulationThreshold = 5;

        protected override void OnPaint(PaintEventArgs e) {
            Logger.WriteLog("Onpaint started");
            //e.Graphics.DrawString("Hello World", this.Font, new SolidBrush(Color.Black), 10, 10);
            foreach (MyLine line in model.modelLines) {
                e.Graphics.DrawLine(Pens.Blue, ConvertToFormCoordinate(line.StartMyPoint), ConvertToFormCoordinate(line.EndMyPoint));
                List<MyPoint> points = new List<MyPoint>();
                points.Add(line.StartMyPoint);
                points.Add(line.EndMyPoint);
                foreach (MyPoint point in points) {
                    e.Graphics.DrawEllipse(Pens.Blue, ConvertToFormCoordinate(point).X - 5, ConvertToFormCoordinate(point).Y - 5, 10, 10);
                    Font f = DefaultFont;
                    FontFamily fontFamily = new FontFamily("Arial");
                    Font font = new Font(fontFamily, 12, FontStyle.Regular, GraphicsUnit.Pixel);
                    e.Graphics.DrawString(point.X + ":" + point.Y, font, Brushes.Black, ConvertToFormCoordinate(point).X - 5, ConvertToFormCoordinate(point).Y - 5);
                }
            }
            drawModelRooms(e);
            ReBindGrid();
            ImageSaver.SaveControlImage(this);
        }

        private void drawModelRooms(PaintEventArgs e) {
            Logger.WriteLog("draw model rooms");
            foreach (Room modelRoom in model.modelRooms) {
                List<PointF> points = new List<PointF>();
                for (var index = 0; index < modelRoom.BoundaryPoints.AsReadOnly().Count; index++) {
                    MyPoint myPoint = modelRoom.BoundaryPoints.AsReadOnly()[index];
                    Logger.WriteLog($"MyPoint at index {index} is {myPoint}");
                    points.Add(ConvertToFormCoordinate(myPoint));
                }

                e.Graphics.FillPolygon(Brushes.Aquamarine, points.ToArray());
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            ReBindGrid();
        }

        private void ReBindGrid() {
            lineGrid.DataSource = null;
            if (model.modelLines != null) {
                lineGrid.DataSource = model.modelLines;
            }

            roomGrid.DataSource = null;
            if (model.modelRooms != null) {
                roomGrid.DataSource = model.modelRooms;
            }

            pointGrid.DataSource = null;
            if (model.ModelPoints != null) {
                pointGrid.DataSource = model.ModelPoints;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            this.Invalidate();
        }

        private void splitButton_Click(object sender, EventArgs e) {
            int moveDistance = int.Parse(this.textBox1.Text);
            model.SplitEdge(moveDistance, model.GetRandomLine());
            Invalidate();
        }

        private void moveButton_Click(object sender, EventArgs e) {
            if (actualSimulationThreshold < MaxSimulationThreshold) {
                SimulationStep();
                Invalidate();
            }
            else {
                MessageBox.Show("Simulation threshold exit. Optimum reached.");
            }

        }

        private void SimulationStep() {
            int moveDistance = int.Parse(this.textBox2.Text);

            //Dictionary<MyLine, double> Costs = new Dictionary<MyLine, double>();
            double mincost = 100000;
            MyLine minline = null;

            //make threadpool like - room pool
            //fix number of modells, (number of threads) move elemnet, calculate cost

            //todo: make parallel
            //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-foreach-loop

            double actualCost = model.CalculateCost();
            foreach (MyLine line in model.modelLines)
            {
                MyLine newMyLine;
                Model tempModel = model.DeepCopy(line, out newMyLine);
                tempModel.MoveLine(moveDistance, newMyLine);

                double cost = tempModel.CalculateCost();
                //Costs.Add(myLine, cost);

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

        private void initButton_Click(object sender, EventArgs e) {
            model = new Model();
            model.InitModel();
            Invalidate();
        }

        //convert to offset coordinates
        private PointF ConvertToFormCoordinate(MyPoint P) {
            if (P == null) return new PointF(0, 0);

            int x = Convert.ToInt32(P.X);
            int y = Convert.ToInt32(-P.Y + 600);
            //Logger.WriteLog($"Convert from {P} to {x},{y}");
            PointF point = new PointF(x, y);
            return point;
        }

        private void button1_Click(object sender, EventArgs e) {
            Invalidate();
        }
    }
}
