using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIWPF.Model {
    public class _Room:_Geometry {
        public _Room(List<_Line> newLines)
        {
            lines = newLines;
        }

        private List<_Line> lines { get; set; }
        public List<_Line> getLines => lines;
        //public void AddLines(List<_Line> LinesToAdd)
        //{
        //    foreach (_Line line in LinesToAdd)
        //    {
        //        lines.Add(line);
        //    }
        //}

        //public void AddLine(_Line LineToAdd)
        //{
        //    lines.Add(LineToAdd);
        //}
        public _RoomType Type { get; set; }

        public _Room DeepCopy() {
            List<_Line> newLines = new List<_Line>();
            foreach (_Line line in lines)
            {
                newLines.Add(line.DeepCopy());
            }
            return new _Room(newLines);
        }
    }
}
