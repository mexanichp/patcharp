using System;
using System.Dynamic;
using Newtonsoft.Json;
using NUnit.Framework;
using Patcharp.Internal;
using Patcharp.UnitTests.Helpers;

namespace Patcharp.UnitTests
{
    [TestFixture]
    public class PatcharpServiceTests
    {
        private IPatcharp _patcharpService;
        private ReflectionHelper _reflectionHelper;

        [SetUp]
        public void SetUp()
        {
            _patcharpService = new Patcharp();
            _reflectionHelper = new ReflectionHelper();
        }

        [Test]
        public void ApplyTo_InvokedWithInvalidJson_ShouldThrowArgumentException()
        {
            // Arrange
            var invalidJson = "{\"test\":{not valid format}}";

            // Act

            // Assert
            Assert.Throws<ArgumentException>(() => _patcharpService.ApplyPatchOperation(new Entity(), invalidJson));
        }

        [Test]
        public void ApplyTo_SetNewObjectToNullProperty_NestedPropertyShouldNotBeNull()
        {
            // Arrange
            dynamic incomingRequest = new ExpandoObject();
            incomingRequest.Id = Guid.NewGuid();
            incomingRequest.ValueBool = true;
            incomingRequest.ValueInt = 19;
            incomingRequest.ValueString = "not null";

            incomingRequest.ValueThis = new ExpandoObject();
            incomingRequest.ValueThis.Id = Guid.NewGuid();

            var originObject = new Entity
            {
                Id = Guid.Empty,
                ValueBool = false,
                ValueInt = 20,
                ValueString = null,
                ValueThis = null
            };

            // Act
            _reflectionHelper.ApplyTo((ExpandoObject)incomingRequest, ref originObject);

            // Assert
            Assert.IsNotNull(originObject.ValueThis);
        }

        [Test]
        public void ApplyTo_SetChangedObjectToObjectProperty_NestedObjectPropertyShouldBeUpdated()
        {
            // Arrange
            dynamic incomingRequest = new ExpandoObject();
            incomingRequest.Id = Guid.NewGuid();
            incomingRequest.ValueBool = true;
            incomingRequest.ValueInt = 19;
            incomingRequest.ValueString = "not null";

            incomingRequest.ValueThis = new ExpandoObject();
            var newGuid = incomingRequest.ValueThis.Id = Guid.NewGuid();
            var oldGuid = Guid.NewGuid();

            var originObject = new Entity
            {
                Id = Guid.Empty,
                ValueBool = false,
                ValueInt = 20,
                ValueString = null,
                ValueThis = new Entity
                {
                    Id = oldGuid
                }
            };

            // Act
            _reflectionHelper.ApplyTo<Entity>((ExpandoObject)incomingRequest, ref originObject);

            // Assert
            Assert.AreEqual(newGuid, originObject.ValueThis.Id);
        }

        [Test]
        public void ApplyPatchOperation_SetNullToObjectProperty_PropertyShouldBeNull()
        {
            // Arrange
            var incomingRequest = new Entity
            {
                Id = Guid.Empty,
                ValueBool = false,
                ValueInt = 20,
                ValueString = null,
                ValueThis = null
            };

            var originObject = new Entity
            {
                Id = Guid.Empty,
                ValueBool = false,
                ValueInt = 20,
                ValueString = null,
                ValueThis = new Entity
                {
                    Id = Guid.NewGuid()
                }
            };

            // Act
            _patcharpService.ApplyPatchOperation(originObject, JsonConvert.SerializeObject(incomingRequest));

            // Assert
            Assert.IsNull(originObject.ValueThis);
        }

        [Test]
        public void ApplyPatchOperation_SetChangedStructToObjectProperty_NestedStructPropertyShouldBeUpdated()
        {
            // Arrange
            var incomingRequest = new Entity
            {
                StructThis = new EntityStruct
                {
                    ValueEnum = EntityEnum.Third
                }
            };

            var originObject = new Entity
            {
                Id = Guid.Empty,
                ValueBool = false,
                ValueInt = 20,
                ValueString = null,
                ValueThis = new Entity
                {
                    Id = Guid.NewGuid()
                },
                StructThis = new EntityStruct
                {
                    ValueEnum = EntityEnum.Second,
                    ValueFloat = 24.555f
                }
            };

            // Act
            _patcharpService.ApplyPatchOperation(originObject, JsonConvert.SerializeObject(incomingRequest));

            // Assert
            Assert.AreEqual(24.555f, originObject.StructThis.ValueFloat);
            Assert.AreEqual(EntityEnum.Third, originObject.StructThis.ValueEnum);
        }
    }
}