using System;

namespace Diploma2.Model {
    public class _GeometryBase {
        public string Name { get; set; }
        public int Number { get; set; }
        public Guid Guid { get; set; }

        public _GeometryBase()
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