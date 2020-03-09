using System;

namespace UIWPF.Model {
    public class _Geometry {
        public string Name { get; set; }
        public int Number { get; set; }
        public Guid Guid { get; set; }

        public _Geometry()
        {
            Name = "";
            Number = -1;
            Guid = Guid.NewGuid();

        }
        public override string ToString()
        {
            return Name;
        }
    }
}