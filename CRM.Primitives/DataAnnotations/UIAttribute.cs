namespace CRM.Primitives.DataAnnotations
{
    /// <summary>
    /// UI color/display information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class UIAttribute : Attribute
    {
        public UIAttribute()
        {
            ColorHex = "#666";
            ColorClass = "";
        }

        public string ColorClass { get; set; }
        public string ColorHex { get; set; }
        public string? GroupName { get; set; }
        public string? IconName { get; set; }
    }
}
