using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netsky_Engine
{
    //this class is identical to ListOfScreenObjects but doesn't add this as object parent when adding
    public class BlindList : ListOfScreenObjects
    {
        public override void Add(ScreenObject so)
        {
            ScreenObject parentt = so.parent;
            base.Add(so);
            so.parent = parentt;
        }
    }
}
