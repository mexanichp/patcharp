namespace Patcharp
{
    public interface IPatcharp
    {
        T ApplyPatchOperation<T>(T item, string jsonBody);
    }
}