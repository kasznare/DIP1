﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ONLAB2;

//TODO: körüljárás alapján lehet megmondani, hogy melyik szobába kerüljün
//TODO: körüljárás jó a kirajzoláshoz és a karbantartáshoz is
//le kell kezelni minden módosítás során a szobák állapotváltozásait, ha nem jó a lépés, dobjuk el
//a lépés előtt lehetne tárolni az előző állapotot
//TODO: implement deepcopy for model class
//TODO: hogyan változik a loss
namespace WindowsFormsApp1 {
    public class Model {
        public List<Line> modelLines = new List<Line>();
        public List<Room> modelRooms = new List<Room>();
        Random rand = new Random(10);
        public Model(List<Line> lines = null, List<Room> rooms = null) {
            if (lines != null) {
                this.modelLines = lines;

            }
            if (rooms != null) {
                modelRooms = rooms;
            }
        }
        /// <summary>
        /// create new model
        /// </summary>
        /// <returns></returns>
        public void InitModel() {
            Point a = new Point(100, 100);
            Point b = new Point(500, 100);
            Point c = new Point(500, 500);
            Point d = new Point(100, 500);

            modelLines.Add(new Line(a, b));
            modelLines.Add(new Line(b, c));
            modelLines.Add(new Line(c, d));
            modelLines.Add(new Line(d, a));
            Room first = new Room("FirstName", "1");
            foreach (Line modelLine in modelLines) {
                modelLine.relatedRooms.Add(first);
            }
            CalculateRooms();
            Trace.WriteLine("InitModel() finished");
        }
        public Line GetRandomLine() {
            int randint = rand.Next(0, modelLines.Count);
            return modelLines.ElementAt(randint);
        }
        //public List<Line> ModelLines
        //{
        //    get { return modelLines; }
        //    set { modelLines = value; }
        //}
        //public List<Room> ModelRooms
        //{
        //    get { return modelRooms; }
        //    set { modelRooms = value; }
        //}
        public void SplitEdge(int splitPercentage, Line selectedEdge) {
            if (modelLines.Count == 0) return;

            double length = selectedEdge.GetLength();

            List<Room> selectedEdgeRelatedRooms = selectedEdge.relatedRooms;
            modelLines.Remove(selectedEdge);
            selectedEdge.startPoint.RelatedLines.Remove(selectedEdge);
            selectedEdge.endPoint.RelatedLines.Remove(selectedEdge);

            Point splitPoint = selectedEdge.GetPointAt(splitPercentage);
            Line a = new Line(selectedEdge.startPoint, splitPoint);
            a.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(a);
            Line b = new Line(splitPoint, selectedEdge.endPoint);
            b.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(b);
        }
        public void MoveLine(int offsetDistance, Line lineToMove) {
            try {
                Point p1 = lineToMove.startPoint;
                Point lineToMoveNormal = lineToMove.GetNV(true);
                #region MyRegion

                Point p3 = p1 + lineToMoveNormal * offsetDistance;
                Point p2 = lineToMove.endPoint;
                Point p4 = p2 + lineToMoveNormal * offsetDistance;

                #region exception checking
                foreach (Line relatedLine in p1.RelatedLines) {
                    if (!relatedLine.Equals(lineToMove) && (!relatedLine.GetNV(true).Equals(lineToMoveNormal)
                         || !relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        if (relatedLine.GetLength() < offsetDistance) {
                            throw new Exception("Vonalhossz hiba: " + relatedLine.GetLength());
                        }
                    }
                }

                foreach (Line relatedLine in p2.RelatedLines) {
                    if (!relatedLine.Equals(lineToMove) && (!relatedLine.GetNV(true).Equals(lineToMoveNormal)
                                                      || !relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        if (relatedLine.GetLength() < offsetDistance) {
                            throw new Exception("Hiba");
                        }
                    }
                }
                #endregion


                bool copyp1 = false;
                Line parallelLine = null;

                foreach (Line relatedLine in p1.RelatedLines) {
                    //ha van olyan vonal, ami miatt másolni kell:
                    if (!relatedLine.Equals(lineToMove) && (relatedLine.GetNV(true).Equals(lineToMoveNormal)
                                                      || relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        copyp1 = true;
                        parallelLine = relatedLine;
                        break;
                    }
                }
                if (!copyp1) //then relocate, this is easy
                {
                    p1.X = p3.X;
                    p1.Y = p3.Y;
                    //itt szoba nem változik
                }
                else {
                    Line lineInMoveDirection = null;
                    foreach (Line relatedLine in p1.RelatedLines) {
                        //ha a mozgatás irányába van vonal
                        bool on = IsOnLine(p3, relatedLine);
                        if (on) {
                            //nem kéne itt copy-t készíteni?
                            lineInMoveDirection = relatedLine;
                            break;
                        }
                    }
                    if (lineInMoveDirection != null) {
                        if (lineInMoveDirection.startPoint.Equals(p1)) {
                            lineInMoveDirection.startPoint = p3;

                        }
                        else {
                            lineInMoveDirection.endPoint = p3;
                        }
                        p3.RelatedLines.Add(lineInMoveDirection);
                        p1.RelatedLines.Remove(lineInMoveDirection);
                    }

                    lineToMove.startPoint = p3;
                    p3.RelatedLines.Add(lineToMove);
                    p1.RelatedLines.Remove(lineToMove);

                    List<Room> p1Rooms = p1.RelatedRooms;
                    List<Room> p3Rooms = p3.RelatedRooms;

                    List<Room> commonRooms = p1Rooms.Intersect(p3Rooms).ToList();

                    Line newConnectionEdge1 = new Line(p1, p3);
                    //TODO: add related modelRooms to this new line.
                    newConnectionEdge1.relatedRooms = commonRooms;

                    modelLines.Add(newConnectionEdge1);
                }
                #endregion

                #region MyRegion
                bool copyp2 = false;
                Line parallelLine2 = null;
                foreach (Line relatedLine in p2.RelatedLines) {
                    if (!relatedLine.Equals(lineToMove) &&
                        (relatedLine.GetNV(true).Equals(lineToMoveNormal) || relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                        copyp2 = true;
                        parallelLine2 = relatedLine;
                        break;
                    }
                }
                if (!copyp2) {
                    p2.X = p4.X;
                    p2.Y = p4.Y;
                }
                else {
                    Line lineInMoveDirection = null;
                    foreach (Line relatedLine in p2.RelatedLines) {
                        bool on = IsOnLine(p4, relatedLine);
                        if (on) {
                            lineInMoveDirection = relatedLine;
                            break;
                        }
                    }
                    if (lineInMoveDirection != null) {
                        if (lineInMoveDirection.startPoint.Equals(p2)) {
                            lineInMoveDirection.startPoint = p4;
                        }
                        else {
                            lineInMoveDirection.endPoint = p4;
                        }
                        p4.RelatedLines.Add(lineInMoveDirection);
                        p2.RelatedLines.Remove(lineInMoveDirection);
                    }

                    lineToMove.endPoint = p4;

                    p4.RelatedLines.Add(lineToMove);
                    p2.RelatedLines.Remove(lineToMove);

                    List<Room> p2Rooms = p2.RelatedRooms;
                    List<Room> p4Rooms = p4.RelatedRooms;

                    List<Room> commonRooms = p2Rooms.Intersect(p4Rooms).ToList();

                    Line newConnectionEdge2 = new Line(p2, p4);
                    newConnectionEdge2.relatedRooms = commonRooms;
                    modelLines.Add(newConnectionEdge2);
                }

                #endregion
            }
            catch (Exception e) {
                Trace.Write("Not legal move " + e.Message);
                //MessageBox.Show();
            }


            List<Line> toremove = new List<Line>();
            foreach (Line line1 in modelLines) {
                if (line1.startPoint.Equals(line1.endPoint) || line1.GetLength() == 0) {
                    toremove.Add(line1);

                    foreach (Line endLine in line1.endPoint.RelatedLines) {
                        if (endLine != line1) {
                            line1.startPoint.RelatedLines.Add(endLine);
                            if (endLine.startPoint == line1.endPoint) {
                                endLine.startPoint = line1.startPoint;

                            }
                            else if (endLine.endPoint == line1.endPoint) {
                                endLine.endPoint = line1.startPoint;
                            }
                        }
                    }

                    line1.startPoint.RelatedLines.Remove(line1);
                    line1.endPoint.RelatedLines.Clear();
                }
            }

            foreach (var line2 in toremove) {
                modelLines.Remove(line2);
            }
            CalculateRooms();
        }
        public void CalculateRooms() {
            if (modelRooms == null)
                modelRooms = new List<Room>();

            modelRooms.Clear();
            foreach (Line line in modelLines) {
                foreach (Room room in line.relatedRooms) {
                    if (!room.bundaryLines.Contains(line)) {
                        room.bundaryLines.Add(line);
                        Trace.WriteLine($"CalculateRooms for line {line} {room.Name} ");
                    }
                }
                modelRooms.AddRange(line.relatedRooms);
            }

            modelRooms = modelRooms.Distinct().ToList();
            Trace.WriteLine(modelRooms.ToString());
            TraceValues();
        }
        void TraceValues() {
            foreach (Room room in modelRooms)
                Trace.WriteLine(room.Name);
        }
        private void RunRedundancyCheck(Point p1, Point p2) {
            #region test
            //List<Line> conflictList = new List<Line>();
            //foreach (Line edge in ModelLines)
            //{
            //    if (edge.startPoint.Equals(p1) ||
            //        edge.startPoint.Equals(p2) ||
            //        edge.endPoint.Equals(p2) ||
            //        edge.endPoint.Equals(p1))
            //    {
            //        conflictList.Add(edge);
            //    }
            //} 
            #endregion
            List<Line> toRemoveLines = new List<Line>();
            List<Line> toAddLines = new List<Line>();
            List<Point> toRemovePoints = new List<Point>();

            #region test
            //foreach (Line line1 in conflictList)
            //{
            //    foreach (Line line2 in conflictList)
            //    {
            //        bool lineEQ = line1.Equals(line2);
            //        bool sameDir = line1.GetNV(true).Equals(line2.GetNV(true));
            //        bool oppDir = (line1.GetNV(true) * (-1)).Equals(line2.GetNV(true));
            //        if (!lineEQ && sameDir || oppDir)
            //        {

            //            if (line1.startPoint.Equals(line2.startPoint) && !line1.endPoint.Equals(line2.endPoint))
            //            {
            //                Line goodline = new Line(line1.endPoint, line2.endPoint);
            //                toAddLines.Add(goodline);
            //                if (!toRemoveLines.Contains(line1))
            //                {
            //                    toRemoveLines.Add(line1);

            //                }
            //                if (!toRemoveLines.Contains(line2))
            //                {
            //                    toRemoveLines.Add(line2);

            //                }

            //                if (!toRemovePoints.Contains(line1.startPoint))
            //                {
            //                    toRemovePoints.Add(line1.startPoint);

            //                }
            //            }
            //            if (line1.startPoint.Equals(line2.endPoint) && !line1.endPoint.Equals(line2.startPoint))
            //            {
            //                Line goodline = new Line(line1.endPoint, line2.startPoint);
            //                toAddLines.Add(goodline);
            //                toRemoveLines.Add(line1);
            //                toRemoveLines.Add(line2);
            //                toRemovePoints.Add(line1.startPoint);
            //            }
            //            if (line1.endPoint.Equals(line2.startPoint) && !line1.startPoint.Equals(line2.endPoint))
            //            {
            //                Line goodline = new Line(line1.startPoint, line2.endPoint);
            //                toAddLines.Add(goodline);
            //                toRemoveLines.Add(line1);
            //                toRemoveLines.Add(line2);
            //                toRemovePoints.Add(line1.endPoint);
            //            }
            //            if (line1.endPoint.Equals(line2.startPoint) && !line1.startPoint.Equals(line2.endPoint))
            //            {
            //                Line goodline = new Line(line1.startPoint, line2.endPoint);
            //                toAddLines.Add(goodline);
            //                toRemoveLines.Add(line1);
            //                toRemoveLines.Add(line2);
            //                toRemovePoints.Add(line1.endPoint);
            //            }
            //        }
            //    }
            //} 
            #endregion


            List<List<Line>> results = new List<List<Line>>();
            toAddLines.Clear();
            toRemoveLines.Clear();
            for (var index = 0; index < modelLines.Count; index++) {
                Line line1 = modelLines[index];
                for (var i = index + 1; i < modelLines.Count; i++) {
                    Line line2 = modelLines[i];
                    bool isLineEQ = (line1 == line2);
                    if (!isLineEQ) {
                        results = CalculateContaining(line1, line2);
                        toAddLines.AddRange(results.First());
                        toRemoveLines.AddRange(results.Last());
                    }
                }
            }

            MessageBox.Show("ToAddLines: " + WriteOutList(toAddLines));
            MessageBox.Show("ToRemoveLines: " + WriteOutList(toRemoveLines));

            foreach (var goodline in toAddLines) {
                try {
                    modelLines.Add(goodline);
                }
                catch (Exception) {
                }
            }
            foreach (var badline in toRemoveLines) {
                try {
                    modelLines.Remove(badline);
                }
                catch (Exception) {
                }
            }
            //delete duplicate points
            //join points if possible
        }
        public static string WriteOutList<T>(List<T> list) {
            string output = String.Empty;

            foreach (T listItem in list) {
                Line item = listItem as Line;

                output += String.Concat(item.startPoint.X + "," + item.startPoint.Y + "  " +
                                        item.endPoint.X + "," + item.endPoint.Y, Environment.NewLine);
            }

            return output;
        }
        private List<List<Line>> CalculateContaining(Line line1, Line line2) {

            List<List<Line>> results = new List<List<Line>>();
            List<Line> addLines = new List<Line>();
            List<Line> remLines = new List<Line>();
            bool isSameDir = line1.GetNV(true).Equals(line2.GetNV(true));
            bool isOppDir = (line1.GetNV(true) * (-1)).Equals(line2.GetNV(true));
            if (isOppDir || isSameDir) {
                bool line1IncludesLine2 = LineIncludes(line1, line2);
                if (line1IncludesLine2) {
                    remLines.Add(line2);
                }

                bool line2IncludesLine1 = LineIncludes(line2, line1);
                if (line2IncludesLine1) {
                    remLines.Add(line1);
                }

                if (!line2IncludesLine1 && !line1IncludesLine2) {
                    Line newLine = LineHaveCommonPoint(line1, line2);

                    if (newLine != null) {
                        remLines.Add(line1);
                        remLines.Add(line2);
                        addLines.Add(newLine);
                    }
                }
            }
            results.Add(addLines);
            results.Add(remLines);

            return results;
        }
        //todo: include partial overlapping
        private bool LineIncludes(Line line1, Line line2) {
            if (IsOnLine(line2.startPoint, line1) && IsOnLine(line2.endPoint, line1)) {
                return true;
            }
            return false;
        }
        private bool IsOnLine(Point point, Line line) {
            return PointOnLine2D(point, line.startPoint, line.endPoint);
        }

        public static bool PointOnLine2D(Point p, Point a, Point b, float t = 1E-03f) {
            // ensure points are collinear
            var zero = (b.X - a.X) * (p.Y - a.Y) - (p.X - a.X) * (b.Y - a.Y);
            if (zero > t || zero < -t) return false;

            // check if X-coordinates are not equal
            if (a.X - b.X > t || b.X - a.X > t)
                // ensure X is between a.X & b.X (use tolerance)
                return a.X > b.X
                    ? p.X + t > b.X && p.X - t < a.X
                    : p.X + t > a.X && p.X - t < b.X;

            // ensure Y is between a.Y & b.Y (use tolerance)
            return a.Y > b.Y
                ? p.Y + t > b.Y && p.Y - t < a.Y
                : p.Y + t > a.Y && p.Y - t < b.Y;
        }

        private Line LineHaveCommonPoint(Line line1, Line line2) {
            Point s1 = line1.startPoint;
            Point s2 = line2.startPoint;
            Point e1 = line1.endPoint;
            Point e2 = line2.endPoint;
            Line newLine = null;
            if (s1.Equals(s2)) {
                newLine = new Line(e1, e2);
            }
            else if (e1.Equals(e2)) {
                newLine = new Line(s1, s2);
            }
            else if (e1.Equals(s2)) {
                newLine = new Line(e2, s1);
            }
            else if (s1.Equals(e2)) {
                newLine = new Line(e1, s2);
            }

            return newLine;
        }

        public void SwitchRoom() {

        }
    }


}