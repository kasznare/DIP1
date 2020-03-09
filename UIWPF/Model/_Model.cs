using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIWPF.Model {
    public class _Model {
        private ObservableCollection<_Room> modelStorage { get; set; }

        public bool AddRoom(_Room r) {
            bool success = false;
            modelStorage.Add(r);
            return success;
        }

        public bool RemoveRoom(_Room r) {
            bool success = false;
            modelStorage.Remove(r);
            return success;
        }

        public _Model() {
            modelStorage = new ObservableCollection<_Room>();
        }

        public _Model(List<_Room> newRooms) {
            modelStorage = new ObservableCollection<_Room>(newRooms);
        }

        public _Model DeepCopy() {
            List<_Room> newRooms = new List<_Room>();
            foreach (_Room room in modelStorage) {
                newRooms.Add(room.DeepCopy());
            }
            return new _Model(newRooms);
        }

        public void MoveLine(int distance, _Line lineToMove) {
            List<_Room> roomsThatCare = new List<_Room>(); //these rooms might need to change
            roomsThatCare = modelStorage.Where(i => i.getLines.Contains(lineToMove)).ToList();
            if (!roomsThatCare.Any()) throw new Exception("LineIsMissing");

            _Line movedLine = lineToMove.DeepCopy();
            _Point moveVector = new _Point(10, 10);
            movedLine.Move(moveVector);

            _Line l1 = new _Line(lineToMove.StartPoint, lineToMove.StartPoint.Move(moveVector));
            _Line l2 = new _Line(lineToMove.EndPoint, lineToMove.EndPoint.Move(moveVector));

            foreach (_Room room in modelStorage)
            {
                foreach (_Line line in room.getLines)
                {
                    if (line.IsTheSame(l1)) l1 = line;
                    if (line.IsTheSame(l2)) l2 = line;
                }
            }

            foreach (_Room room in roomsThatCare)
            {
                room.getLines.Remove(lineToMove);
                foreach (_Line getLine in room.getLines)
                {
                    _Point p = getLine.ConnectsPoint(lineToMove);
                    if (p!=null && p==lineToMove.StartPoint) room.getLines.Add(l1);
                    if (p!=null && p==lineToMove.EndPoint) room.getLines.Add(l2);
                    
                }
                List<_Line> l = room.getLines.Where(i=> i.Connects(lineToMove)).ToList();
                
                room.getLines.Add(movedLine); //this detached the model
            }
//at this point we only need to solve
//1. same line already existed before - fully or partially
//2. new point was not needed
   
            //we still need to find if there are any rooms just touching the line.

            foreach (_Room room in modelStorage)
            {
                
            }

        }
    }
}
