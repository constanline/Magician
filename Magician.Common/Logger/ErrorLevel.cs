using System.ComponentModel;

namespace Magician.Common.Logger
{
    [Description("异常/错误严重级别")]
    public enum ErrorLevel
    {
        [Description("致命的")]
        Fatal = 4,
        [Description("高")]
        High = 3,
        [Description("普通")]
        Standard = 2,
        [Description("低")]
        Low = 1
    }    
}
