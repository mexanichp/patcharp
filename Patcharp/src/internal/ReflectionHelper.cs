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
        //private readonly IEnumerable _stack;

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
            var objects = new Stack<Tuple<ExpandoObject, object>>(1);
            objects.Push(new Tuple<ExpandoObject, object>(patchOp, objToPatch));

            while (objects.Count > 0)
            {
                var objFromStack = objects.Pop();
                var keyValueObject = new Dictionary<string, object>(objFromStack.Item1, StringComparer.OrdinalIgnoreCase);

                // Iterate through the object's properties and set values for them.
                foreach (var propValue in keyValueObject)
                {
                    var patchOpValue = propValue.Value;
                    var refObjProp = objFromStack.Item2.GetType().GetRuntimeProperty(propValue.Key);
                    var typeInfo = refObjProp?.PropertyType.GetTypeInfo();
                    
                    switch (patchOpValue)
                    {
                        case ExpandoObject eoStruct when typeInfo != null && typeInfo.IsValueType:
                            var refObjValue = refObjProp.GetValue(objFromStack.Item2);
                            if (refObjValue == null)
                            {
                                refObjValue = Activator.CreateInstance(refObjProp.PropertyType);
                                refObjProp.SetValue(objFromStack.Item2, refObjValue);
                                break;
                            }

                            var tuple = new Tuple<ExpandoObject, object>(eoStruct, refObjValue);
                            objects.Push(tuple);
                            refObjProp.SetValue(objFromStack.Item2, tuple.Item2);
                            break;

                        case ExpandoObject expandoObject when refObjProp != null:
                            refObjValue = refObjProp.GetValue(objFromStack.Item2);
                            if (refObjValue == null)
                            {
                                refObjValue = Activator.CreateInstance(refObjProp.PropertyType);
                                refObjProp.SetValue(objFromStack.Item2, refObjValue);
                                break;
                            }

                            objects.Push(new Tuple<ExpandoObject, object>(expandoObject, refObjValue));
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
                                refObjProp.SetValue(objFromStack.Item2, Enum.ToObject(refObjProp.PropertyType, patchOpValue));
                            }
                            catch
                            {
                                // ignored
                            }

                            break;

                        case string enumStr when typeInfo != null && typeInfo.IsEnum:
                            try
                            {
                                refObjProp.SetValue(objFromStack.Item2, Enum.Parse(refObjProp.PropertyType, enumStr));
                            }
                            catch
                            {
                                // ignored
                            }

                            break;

                        case var any when refObjProp != null && any != null:
                            if (refObjProp.PropertyType == patchOpValue.GetType())
                            {
                                refObjProp.SetValue(objFromStack.Item2, patchOpValue);
                            }

                            if (!TypeDescriptor.GetConverter(refObjProp.PropertyType).CanConvertFrom(patchOpValue.GetType()))
                            {
                                break;
                            }

                            refObjProp.SetValue(objFromStack.Item2, Convert.ChangeType(TypeDescriptor.GetConverter(refObjProp.PropertyType).ConvertFrom(propValue.Value), refObjProp.PropertyType));
                            break;

                        case null when refObjProp != null:
                            refObjProp.SetValue(objFromStack.Item2, patchOpValue);
                            break;
                    }
                }
            }
        }

        //private void StackPush<T>(ExpandoObject patchOp, ref T obj)
        //{
        //    Stack<Tuple<ExpandoObject, T>> stack = (_stack as Stack<Tuple<ExpandoObject, T>>) ?? new Stack<Tuple<ExpandoObject, T>>(1);
        //    stack.Push(new Tuple<ExpandoObject, T>(patchOp, obj));
        //}
    }
}