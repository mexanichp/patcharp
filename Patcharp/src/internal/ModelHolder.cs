using System;
using System.Dynamic;

namespace Patcharp.Internal
{
    public class ModelHolder
    {
        public event EventHandler<ModelHolderArgs> ModelChanged;

        private readonly ExpandoObject _patchOp;
        private object _objToPatch;

        public ModelHolder(ExpandoObject patchOp, object objToPatch)
        {
            _patchOp = patchOp;
            _objToPatch = objToPatch;
        }

        public void UpdateObjToPatch(object updatedObj)
        {
            _objToPatch = updatedObj;
            ModelChanged?.Invoke(this, new ModelHolderArgs(_patchOp, _objToPatch));
        }
    }
}