using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIWPF.Model {
    public class _Model {
        private List<_Room> modelStorage { get; set; }

        public bool AddRoom(_Room r)
        {
            bool success = false;
            modelStorage.Add(r);
            return success;
        }

        public bool RemoveRoom(_Room r)
        {
            bool success = false;
            modelStorage.Remove(r);
            return success;
        }
    }
}
