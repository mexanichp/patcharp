using System;
using System.Dynamic;

namespace Patcharp.Internal
{
    public class ModelHolderArgs : EventArgs
    {
        public ExpandoObject PatchOp { get; private set; }

        public object ObjToPatch { get; private set; }

        public ModelHolderArgs(ExpandoObject patchOp, object objToPatch)
        {
            PatchOp = patchOp;
            ObjToPatch = objToPatch;
        }
    }
}