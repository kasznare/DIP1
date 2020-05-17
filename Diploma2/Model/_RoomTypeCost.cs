using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma2.Model
{
    public class _RoomTypeCost
    {
        public _RoomType type1 { get; set; }
        public _RoomType type2 { get; set; }
        public int Cost { get; set; }

        public _RoomTypeCost(_RoomType type1, _RoomType type2, int cost)
        {
            this.type1 = type1;
            this.type2 = type2;
            Cost = cost;
        }

        public _RoomTypeCost getInverse()
        {
            return new _RoomTypeCost(type2, type1, Cost);
        }
    }
}
