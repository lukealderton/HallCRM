using System.ComponentModel.DataAnnotations;

namespace CRM.Primitives.Common.Enums
{
    /// <summary>
    /// Used to tell the app where a notification should link to (e.g. this id is a conversation id).
    /// </summary>
    public enum ItemType : Int32
    {
        [Display(Name = "None")]
        None = 0,
    }
}
