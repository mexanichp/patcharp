# patcharp
:fire: Patch C# objects at runtime

[![Build status](https://ci.appveyor.com/api/projects/status/76n5b5d5p4symvyl?svg=true)](https://ci.appveyor.com/project/mexanichp/patcharp)

A library intended to help with patch operations on DTO objects. 

## Usage
```csharp
var _patcharp = new Patcharp();
var json = $"{{\"ValueInt\":\"{value}\"}}";
var entity = new Entity();

// Apply patch operation of ValueInt field to Entity.
_patcharp.ApplyPatchOperation(entity, json); 
```


## Testing
There is an intent to cover all possible situations with value types, reference types, nested and complex objects. 
Unit tests perform via NUnit framework.


## Distribution
This library will be available in NuGet feed once it's completely tested and ready to be consumed.


## Contribution
There are a lot of work to be done on performance improvements, unit testing, finding bugs. 
Feel free to create a new issue or help with optimization.

Please send pull requests to add improvements you've come up with.
