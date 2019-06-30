using NUnit.Framework;
using Patcharp.UnitTests.Helpers;

namespace Patcharp.UnitTests
{
    [TestFixture(typeof(short))]
    [TestFixture(typeof(double))]
    [TestFixture(typeof(float))]
    [TestFixture(typeof(EntityEnum))]
    public class PatcharpConvertionTests<T>
    {
        private IPatcharp _patcharp;

        [SetUp]
        public void SetUp()
        {
            _patcharp = new Patcharp();
        }

        [DatapointSource]
        public short[] ArrayDouble2 = { short.MaxValue, short.MinValue };
        [DatapointSource]
        public double[] ArrayDouble1 = { double.MaxValue, double.MinValue };
        [DatapointSource]
        public float[] ArrayInt = { float.MinValue, float.MaxValue };


        [Theory]
        public void GivenShortEdgeValues_WhenTryingToPatchInt_ThenConvertsSuccessfully(T value)
        {
            // Arrange
            Assume.That(value is short);
            var json = $"{{\"ValueInt\":\"{value}\"}}";
            var entity = new Entity();

            // Act
            _patcharp.ApplyPatchOperation(entity, json);

            // Assert
            Assert.That(value, Is.EqualTo(entity.ValueInt));
        }

        [Theory]
        public void GivenFloatOrDoubleEdgeValues_WhenTryingToPatchInt_ThenThrowsArgumentException(T value)
        {
            // Arrange
            Assume.That(value is double || value is float);
            var json = $"{{\"ValueInt\":\"{value}\"}}";
            var entity = new Entity();

            // Act
            var action = new TestDelegate(() => _patcharp.ApplyPatchOperation(entity, json));

            // Assert
            Assert.That(action, Throws.ArgumentException);
        }

        [Theory]
        public void GivenEnumValues_WhenTryingToPatchEnum_ThenConvertsSuccessfully(T value)
        {
            // Arrange
            Assume.That(value is EntityEnum);
            var json = $"{{\"StructThis\":{{\"ValueEnum\":\"{value}\"}}}}";
            var entity = new Entity();

            // Act
            var result = _patcharp.ApplyPatchOperation(entity, json);

            // Assert
            Assert.That(result.StructThis.ValueEnum, Is.EqualTo(value));
        }
    }
}