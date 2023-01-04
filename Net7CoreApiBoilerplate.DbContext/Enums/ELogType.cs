using Net7CoreApiBoilerplate.Utility.Attributes;

namespace Net7CoreApiBoilerplate.DbContext.Enums
{
    public enum ELogType
    {
        [EnumDescription("Unknown", 0)]
        Unknown = 0,

        [EnumDescription("New blog added", 0)]
        BlogAdded = 1,
        [EnumDescription("Blog updated", 0)]
        BlogUpdated = 2,

        // etc...
    }
}
