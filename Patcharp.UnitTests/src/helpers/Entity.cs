using System;

namespace Patcharp.UnitTests.Helpers
{
    public class Entity
    {
        public Guid Id { get; set; }

        public string ValueString { get; set; }

        public int ValueInt { get; set; }

        public long ValueLong { get; set; }

        public bool ValueBool { get; set; }

        public Entity ValueThis { get; set; }

        public EntityStruct StructThis { get; set; }
    }

    public struct EntityStruct
    {
        public EntityEnum ValueEnum { get; set; }

        public float ValueFloat { get; set; }
    }

    public enum EntityEnum
    {
        First,
        Second,
        Third
    }
}