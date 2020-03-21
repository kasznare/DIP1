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

        public _Room()
        {
            
        }

        public List<_Line> lines { get; set; }
       
        public _Room DeepCopy() {
            List<_Line> newLines = new List<_Line>();
            foreach (_Line line in lines)
            {
                newLines.Add(line.DeepCopy());
            }
            return new _Room(newLines);
        }

        public List<_Point> GetBoundaryPointsSorted()
        {
            return new List<_Point>();
        }
    }
}
