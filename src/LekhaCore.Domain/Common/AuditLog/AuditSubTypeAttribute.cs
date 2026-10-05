namespace LekhaCore.Core.Domain.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class AuditSubTypeAttribute : Attribute
    {
        public string Name { get; }

        public AuditSubTypeAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Audit subtype cannot be null or empty.", nameof(name));
            }

            Name = name;
        }
    }
}
