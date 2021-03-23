using System;
namespace Magician.Common.Style
{
    interface IMagicianControl
    {
        BaseStyle.EPresetStyle PresetStyle { get; set; }
    }
}