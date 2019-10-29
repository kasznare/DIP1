using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Annotations;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace WindowsFormsApp1.Utilities {
    public static class Utils {
        public static MColor ToMediaColor(this DColor color) {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static DColor ToDrawingColor(this MColor color) {
            return DColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public class SmartList<T> : List<T>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
