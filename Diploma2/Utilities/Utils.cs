using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    }
}
