using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Patcharp.UnitTests")]
namespace Patcharp.Internal
{
    internal class ReflectionHelper
    {
        public virtual void ApplyTo<T>(ExpandoObject patchOp, ref T objToPatch)
        {
            if (patchOp == null)
            {
                throw new ArgumentNullException(nameof(patchOp));
            }

            if (objToPatch == null)
            {
                throw new ArgumentNullException(nameof(objToPatch));
            }

            // Stack of items to patch.
            var objects = new Stack<ModelHolder>(1);
            objects.Push(new ModelHolder(patchOp, objToPatch));

            while (objects.Count > 0)
            {
                var objFromStack = objects.Pop();
                var keyValueObject = new Dictionary<string, object>(objFromStack.PatchOp, StringComparer.OrdinalIgnoreCase);

                // Iterate through the object's properties and set values for them.
                foreach (var propValue in keyValueObject)
                {
                    var patchOpValue = propValue.Value;
                    var refObjProp = objFromStack.ObjToPatch.GetType().GetRuntimeProperty(propValue.Key);
                    var typeInfo = refObjProp?.PropertyType.GetTypeInfo();
                    
                    switch (patchOpValue)
                    {
                        case ExpandoObject eoStruct when typeInfo != null && typeInfo.IsValueType:
                            var refObjValue = refObjProp.GetValue(objFromStack.ObjToPatch);
                            var modelHolder = new ModelHolder(eoStruct, refObjValue);
                            modelHolder.ModelChanged += (sender, args) => refObjProp.SetValue(objFromStack.ObjToPatch, args.ObjToPatch);
                            objects.Push(modelHolder);
                            break;

                        case ExpandoObject expandoObject when refObjProp != null:
                            refObjValue = refObjProp.GetValue(objFromStack.ObjToPatch);
                            if (refObjValue == null)
                            {
                                refObjValue = Activator.CreateInstance(refObjProp.PropertyType);
                                refObjProp.SetValue(objFromStack.ObjToPatch, refObjValue);
                                break;
                            }

                            objects.Push(new ModelHolder(expandoObject, refObjValue));
                            break;

                        case int _ when typeInfo != null && typeInfo.IsEnum:
                        case uint _ when typeInfo != null && typeInfo.IsEnum:
                        case short _ when typeInfo != null && typeInfo.IsEnum:
                        case ushort _ when typeInfo != null && typeInfo.IsEnum:
                        case long _ when typeInfo != null && typeInfo.IsEnum:
                        case ulong _ when typeInfo != null && typeInfo.IsEnum:
                        case byte _ when typeInfo != null && typeInfo.IsEnum:
                        case sbyte _ when typeInfo != null && typeInfo.IsEnum:
                        case char _ when typeInfo != null && typeInfo.IsEnum:
                            try
                            {
                                refObjProp.SetValue(objFromStack.ObjToPatch, Enum.ToObject(refObjProp.PropertyType, patchOpValue));
                            }
                            catch
                            {
                                // ignored
                            }

                            break;

                        case string enumStr when typeInfo != null && typeInfo.IsEnum:
                            try
                            {
                                refObjProp.SetValue(objFromStack.ObjToPatch, Enum.Parse(refObjProp.PropertyType, enumStr));
                            }
                            catch
                            {
                                // ignored
                            }

                            break;

                        case IEnumerable enumerable when typeInfo != null && typeInfo.IsAssignableFrom(enumerable.GetType().GetTypeInfo()):
                            refObjProp.SetValue(objFromStack.ObjToPatch, enumerable);
                            break;

                        case List<object> list when typeInfo != null && typeInfo.IsArray:

                            // This will be a future implementation to handle array specific changes.
                            throw new NotImplementedException();

                        case var any when refObjProp != null && any != null:
                            if (refObjProp.PropertyType == patchOpValue.GetType())
                            {
                                refObjProp.SetValue(objFromStack.ObjToPatch, patchOpValue);
                            }

                            if (!TypeDescriptor.GetConverter(refObjProp.PropertyType).CanConvertFrom(patchOpValue.GetType()))
                            {
                                break;
                            }

                            refObjProp.SetValue(objFromStack.ObjToPatch, Convert.ChangeType(TypeDescriptor.GetConverter(refObjProp.PropertyType).ConvertFrom(propValue.Value), refObjProp.PropertyType));
                            break;

                        case null when refObjProp != null:
                            refObjProp.SetValue(objFromStack.ObjToPatch, patchOpValue);
                            break;
                    }
                }

                objFromStack.OnHolderIsUpdated();
            }
        }
    }
}