using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Netsky_Engine.World
{
    //this class is identical to ListOfObject3Ds but doesn't add this as object parent when adding
    public class BlindList3D : ListOfObject3Ds
    {
        public override void Add(Object3D so)
        {
            Object3D parentt = so.parent;
            base.Add(so);
            so.parent = parentt;
        }
    }
}
