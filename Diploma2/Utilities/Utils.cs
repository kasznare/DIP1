using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Diploma2.Annotations;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace Diploma2.Utilities {
    public static class Utils {
        public static MColor ToMediaColor(this DColor color) {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static DColor ToDrawingColor(this MColor color) {
            return DColor.FromArgb(color.A, color.R, color.G, color.B);
        }
        /// <summary>
        /// Method to compute the centroid of a polygon. This does NOT work for a complex polygon.
        /// </summary>
        /// <param name="poly">points that define the polygon</param>
        /// <returns>centroid point, or PointF.Empty if something wrong</returns>
        public static PointF GetCentroid(List<PointF> poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
                return PointF.Empty;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new PointF(centerX / accumulatedArea, centerY / accumulatedArea);
        }
    }
}
