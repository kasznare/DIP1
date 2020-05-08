using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using GeoLib;

namespace GeoLib
{
    /// <summary>
    /// Class to facilitate drawing geolib objects using Microsoft classes.
    /// </summary>
    public class CGeoDraw
    {
        /// <summary>
        /// Draws a line.
        /// </summary>
        public void Draw(C2DLine Line, Graphics graphics, Pen pen)
        {
            C2DPoint pt1 = Line.GetPointFrom();
            C2DPoint pt2 = Line.GetPointTo();
            this.ScaleAndOffSet(pt1);
            this.ScaleAndOffSet(pt2);
            graphics.DrawLine(pen, (int)pt1.x, (int)pt1.y, (int)pt2.x, (int)pt2.y);
        }

        /// <summary>
        /// Gets the parameters required to draw an arc.
        /// </summary>
        private void GetArcParameters(C2DArc Arc, C2DRect Rect, ref int nStartAngle, ref int nSweepAngle)
        {
            C2DPoint Centre = Arc.GetCircleCentre();

            Rect.Set(Centre.x - Arc.Radius, Centre.y + Arc.Radius,
                                        Centre.x + Arc.Radius, Centre.y - Arc.Radius);

            ScaleAndOffSet(Rect.TopLeft);
            ScaleAndOffSet(Rect.BottomRight);
            ScaleAndOffSet(Centre); // CR 19-1-09

            C2DPoint bottomRightTemp = new C2DPoint(Rect.BottomRight); // to make valid // CR 11-3-09
            Rect.BottomRight.Set(Rect.TopLeft); // to make valid // CR 11-3-09
            Rect.ExpandToInclude(bottomRightTemp); // to make valid // CR 11-3-09

            C2DPoint pt1 = Arc.Line.GetPointFrom();
            C2DPoint pt2 = Arc.Line.GetPointTo();
            this.ScaleAndOffSet(pt1);
            this.ScaleAndOffSet(pt2);

            C2DVector vec1 = new C2DVector(Centre, pt1);
            C2DVector vec2 = new C2DVector(Centre, pt2);

            C2DVector vecx = new C2DVector(100, 0); // x - axis

            double dStartAngle = vecx.AngleToLeft(vec1) * Constants.conDegreesPerRadian;
            double dSweepAngle = 0;

            bool bAlreadyFlipped = Scale.x * Scale.y < 0;

            if (Arc.ArcOnRight ^ bAlreadyFlipped)
                dSweepAngle = vec1.AngleToLeft(vec2) * Constants.conDegreesPerRadian;
            else
                dSweepAngle = -vec1.AngleToRight(vec2) * Constants.conDegreesPerRadian;

            nStartAngle = (int)dStartAngle;
            if (nStartAngle == 360)
                nStartAngle = 0;
            nSweepAngle = (int)dSweepAngle;
        }

        /// <summary>
        /// Draws an arc.
        /// </summary>
        public void Draw(C2DArc Arc, Graphics graphics, Pen pen)
        {
            C2DRect Rect = new C2DRect();
            int nStartAngle = 0;
            int nSweepAngle = 0;

            GetArcParameters(Arc, Rect, ref nStartAngle, ref nSweepAngle);

            if (nSweepAngle == 0)
                nSweepAngle = 1;

            int Width = (int)Rect.Width();
            if (Width == 0)
                Width = 1;
            int Height = (int)Rect.Height();
            if (Height == 0)
                Height = 1;

            graphics.DrawArc(pen, (int)Rect.TopLeft.x, (int)Rect.BottomRight.y,
                Width, Height, nStartAngle, nSweepAngle);
        }

        /// <summary>
        /// Draws a polygon
        /// </summary>
        public void Draw(C2DPolyBase Poly, Graphics graphics, Pen pen)
        {
            for (int i = 0; i < Poly.Lines.Count; i++)
            {
                if (Poly.Lines[i] is C2DLine)
                {
                    Draw(Poly.Lines[i] as C2DLine, graphics, pen);
                }
                else if (Poly.Lines[i] is C2DArc)
                {
                    Draw(Poly.Lines[i] as C2DArc, graphics, pen);
                }
            }
        }

        /// <summary>
        /// Draws a polygon
        /// </summary>
        public void Draw(C2DHoledPolyBase Poly, Graphics graphics, Pen pen)
        {
            Draw(Poly.Rim, graphics, pen);

            for (int h = 0; h < Poly.HoleCount; h++)
            {
                Draw(Poly.GetHole(h), graphics, pen);
            }
        }

        /// <summary>
        /// Draws a rectangle
        /// </summary>
        public void Draw(C2DRect Rect, Graphics graphics, Pen pen)
        {
            C2DPoint pt1 = new C2DPoint(Rect.TopLeft);
            C2DPoint pt2 = new C2DPoint(Rect.BottomRight);
            this.ScaleAndOffSet(pt1);
            this.ScaleAndOffSet(pt2);

            int TLx = (int)pt1.x;
            if (Scale.x < 0)
                TLx = (int)pt2.x;

            int TLy = (int)pt2.y;
            if (Scale.x < 0)
                TLy = (int)pt1.y;

            graphics.DrawRectangle(pen, TLx, TLy,   (int)Math.Abs(pt1.x - pt2.x), 
                                                    (int)Math.Abs(pt1.y - pt2.y));
        }

        /// <summary>
        /// Draws a circle
        /// </summary>
        public void Draw(C2DCircle Circle, Graphics graphics, Pen pen)
        {
            C2DRect Rect = new  C2DRect();
            Circle.GetBoundingRect(Rect);
            this.ScaleAndOffSet(Rect.BottomRight);
            this.ScaleAndOffSet(Rect.TopLeft);

            graphics.DrawEllipse(pen, (int)Rect.TopLeft.x, (int)Rect.BottomRight.y, (int)Rect.Width(), (int)Rect.Height());
        }

        /// <summary>
        /// Draws a triangle
        /// </summary>
        public void Draw(C2DTriangle Triangle, Graphics graphics, Pen pen)
        {
            Draw(new C2DLine( Triangle.p1, Triangle.p2), graphics, pen);
            Draw(new C2DLine(Triangle.p2, Triangle.p3), graphics, pen);
            Draw(new C2DLine( Triangle.p3, Triangle.p1), graphics, pen);
        }

        /// <summary>
        /// Draws a Segment
        /// </summary>
        public void Draw(C2DSegment Segment, Graphics graphics, Pen pen)
        {
            Draw(Segment.Arc, graphics, pen);
            Draw(Segment.Arc.Line, graphics, pen);
        }

        /// <summary>
        /// Draws a triangle filled.
        /// </summary>
        public void DrawFilled(C2DTriangle Triangle, Graphics graphics, Brush brush)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

            C2DPoint pt1 = new C2DPoint(Triangle.p1);
            C2DPoint pt2 = new C2DPoint(Triangle.p2);
            C2DPoint pt3 = new C2DPoint(Triangle.p3);

            ScaleAndOffSet(pt1);
            ScaleAndOffSet(pt2);
            ScaleAndOffSet(pt3);

            gp.AddLine((int)pt1.x, (int)pt1.y, (int)pt2.x, (int)pt2.y);
            gp.AddLine((int)pt2.x, (int)pt2.y, (int)pt3.x, (int)pt3.y);
            gp.AddLine((int)pt3.x, (int)pt3.y, (int)pt1.x, (int)pt1.y);

            graphics.FillPath(brush, gp);
        }

        /// <summary>
        /// Draws a segment filled.
        /// </summary>
        public void DrawFilled(C2DSegment Segment, Graphics graphics, Brush brush)
        {
            C2DRect Rect = new C2DRect();
            int nStartAngle = 0;
            int nSweepAngle = 0;

            GetArcParameters(Segment.Arc, Rect, ref nStartAngle, ref nSweepAngle);

            if (nSweepAngle == 0)
                nSweepAngle = 1;

            int Width = (int)Rect.Width();
            if (Width == 0)
                Width = 1;
            int Height = (int)Rect.Height();
            if (Height == 0)
                Height = 1;

            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

            gp.AddArc((int)Rect.TopLeft.x, (int)Rect.BottomRight.y,
                Width, Height, nStartAngle, nSweepAngle);

            C2DPoint ptFrom = Segment.Arc.Line.GetPointFrom();
            C2DPoint ptTo = Segment.Arc.Line.GetPointTo();
            ScaleAndOffSet(ptFrom);
            ScaleAndOffSet(ptTo);
            gp.AddLine((int)ptTo.x, (int)ptTo.y, (int)ptFrom.x, (int)ptFrom.y);

            graphics.FillPath(brush, gp);
        }

        /// <summary>
        /// Draws a circle filled.
        /// </summary>
        public void DrawFilled(C2DCircle Circle, Graphics graphics, Brush brush)
        {
            C2DRect Rect = new C2DRect();
            Circle.GetBoundingRect(Rect);
            this.ScaleAndOffSet(Rect.BottomRight);
            this.ScaleAndOffSet(Rect.TopLeft);

            graphics.FillEllipse(brush, (int)Rect.TopLeft.x, (int)Rect.BottomRight.y, (int)Rect.Width(), (int)Rect.Height());
        }

        /// <summary>
        /// Draws a rectangle filled.
        /// </summary>
        public void DrawFilled(C2DRect Rect, Graphics graphics, Brush brush)
        {
            C2DPoint pt1 = new C2DPoint(Rect.TopLeft);
            C2DPoint pt2 = new C2DPoint(Rect.BottomRight);
            this.ScaleAndOffSet(pt1);
            this.ScaleAndOffSet(pt2);

            int TLx = (int)pt1.x;
            if (Scale.x < 0)
                TLx = (int)pt2.x;

            int TLy = (int)pt2.y;
            if (Scale.x < 0)
                TLy = (int)pt1.y;

            graphics.FillRectangle(brush, TLx, TLy, (int)Math.Abs(pt1.x - pt2.x),
                                                    (int)Math.Abs(pt1.y - pt2.y));
        }

        /// <summary>
        /// Draws a polygon filled.
        /// </summary>
        public void DrawFilled(C2DPolyBase Poly, Graphics graphics, Brush brush)
        {

            System.Drawing.Drawing2D.GraphicsPath gp = CreatePath(Poly);

            graphics.FillPath(brush, gp);
        }

        /// <summary>
        /// Creates a path based on a polygon.
        /// </summary>
        private System.Drawing.Drawing2D.GraphicsPath CreatePath( C2DPolyBase Poly)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();


            if (Poly.Lines.Count == 0)
                return gp;

            for (int i = 0; i < Poly.Lines.Count; i++)
            {
                if (Poly.Lines[i] is C2DLine)
                {
                    C2DPoint ptFrom = Poly.Lines[i].GetPointFrom();
                    C2DPoint ptTo = Poly.Lines[i].GetPointTo();
                    ScaleAndOffSet(ptFrom);
                    ScaleAndOffSet(ptTo);
                    gp.AddLine((int)ptFrom.x, (int)ptFrom.y, (int)ptTo.x, (int)ptTo.y);
                }
                else if (Poly.Lines[i] is C2DArc)
                {

                    C2DRect Rect = new C2DRect();
                    int nStartAngle = 0;
                    int nSweepAngle = 0;

                    GetArcParameters(Poly.Lines[i] as C2DArc, Rect, ref nStartAngle, ref nSweepAngle);

                    if (nSweepAngle == 0)
                        nSweepAngle = 1;

                    int Width = (int)Rect.Width();
                    if (Width == 0)
                        Width = 1;
                    int Height = (int)Rect.Height();
                    if (Height == 0)
                        Height = 1;

                    gp.AddArc((int)Rect.TopLeft.x, (int)Rect.BottomRight.y,
                        Width, Height, nStartAngle, nSweepAngle);

                }
            }

            gp.CloseFigure();

            return gp;
        }

        /// <summary>
        /// Draws a polygon filled.
        /// </summary>
        public void DrawFilled(C2DHoledPolyBase Poly, Graphics graphics, Brush brush)
        {
            if (Poly.Rim.Lines.Count == 0)
                return;

            System.Drawing.Drawing2D.GraphicsPath gp = CreatePath(Poly.Rim);

            for (int h = 0; h < Poly.HoleCount; h++)
            {
                if (Poly.GetHole(h).Lines.Count > 2)
                {
                    gp.AddPath(  CreatePath(  Poly.GetHole(h)), false);
                }
            }
            gp.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;

            graphics.FillPath(brush, gp);
        }

        /// <summary>
        /// Function to scale and offset a point.
        /// </summary>
        private void ScaleAndOffSet(C2DPoint pt)
        {
            pt.x -= Offset.x;
            pt.y -= Offset.y;
            pt.x *= Scale.x;
            pt.y *= Scale.y;
        }

        /// <summary>
        /// The offset to be used.
        /// </summary>
        public C2DPoint Offset = new C2DPoint(0, 0);

        /// <summary>
        ///  The scale to be used.
        /// </summary>
        public C2DPoint Scale = new C2DPoint(1, 1);
    }
}
