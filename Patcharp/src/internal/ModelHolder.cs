using System;
using System.Dynamic;

namespace Patcharp.Internal
{
    public class ModelHolder
    {
        public event EventHandler<ModelHolderArgs> ModelChanged;

        public ExpandoObject PatchOp { get; }
        public object ObjToPatch { get; }

        public ModelHolder(ExpandoObject patchOp, object objToPatch)
        {
            PatchOp = patchOp;
            ObjToPatch = objToPatch;
        }

        public void OnHolderIsUpdated()
        {
            ModelChanged?.Invoke(this, new ModelHolderArgs(PatchOp, ObjToPatch));
        }
    }
}