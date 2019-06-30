using System;
using System.Dynamic;
using Newtonsoft.Json;
using Patcharp.Internal;

namespace Patcharp
{
    public class Patcharp : IPatcharp
    {
        private readonly ReflectionHelper _reflectionHelper;

        public Patcharp()
        {
            _reflectionHelper = new ReflectionHelper();
        }

        public T ApplyPatchOperation<T>(T item, string jsonBody)
        {
            dynamic request;

            try
            {
                request = JsonConvert.DeserializeObject<ExpandoObject>(jsonBody);
            }
            catch (JsonReaderException e)
            {
                throw new ArgumentException("JSON argument has invalid format.", nameof(jsonBody), e);
            }

            _reflectionHelper.ApplyTo(request, ref item);

            return item;
        }
    }
}