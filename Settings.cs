using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.ModOptions;
using UnityEngine;

namespace CoordDisplay;

public class Settings : ModSettings
{
    public static readonly ModSettingBool ModEnabled = new(true)
    {
        description = "Show the tower and mouse position while placing."
    };

    public static readonly ModSettingBool LogPositions = new(false)
    {
        description = "Log a tower's position when selecting it."
    };

    public static readonly ModSettingHotkey MoveTextHotkey = new(KeyCode.None)
    {
        description = "Set the text position to the current mouse position"
    };

    public static readonly ModSettingInt X = new(700)
    {
        description = "The horizontal text position in pixels"
    };

    public static readonly ModSettingInt Y = new(30)
    {
        description = "The vertical text position in pixels"
    };
}