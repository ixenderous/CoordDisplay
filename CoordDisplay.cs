using MelonLoader;
using BTD_Mod_Helper;
using CoordDisplay;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(CoordDisplay.CoordDisplay), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace CoordDisplay;

public class CoordDisplay : BloonsTD6Mod
{
    private GameObject canvasGO;
    private TextMeshProUGUI coordText;

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!Settings.ModEnabled || InGame.instance == null || InGame.instance.inputManager == null)
            return;
        
        if (coordText == null)
            CreateUI();

        var input = InGame.instance.inputManager;
        var placementModel = input.placementModel;

        if (placementModel == null)
        {
            if (coordText != null) coordText.enabled = false;
            return;
        }

        var towerPos = input.entityPositionWorld;
        var mousePos = Input.mousePosition;

        coordText.text =
            $"Mouse: ({mousePos.x}, {mousePos.y})\n" +
            $"Tower: ({towerPos.x}, {towerPos.y})";

        coordText.rectTransform.position = Input.mousePosition + new Vector3(0, 40, 0);
        coordText.enabled = true;
    }

    private void CreateUI()
    {
        canvasGO = new GameObject("CoordCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        Object.DontDestroyOnLoad(canvasGO);

        var textGO = new GameObject("CoordText");
        textGO.transform.SetParent(canvasGO.transform);
        coordText = textGO.AddComponent<TextMeshProUGUI>();
        coordText.fontSize = 20;
        coordText.alignment = TextAlignmentOptions.Center;
        coordText.color = Color.black;
        coordText.enableWordWrapping = false;
        coordText.overflowMode = TextOverflowModes.Overflow;
        
        coordText.fontStyle = FontStyles.Bold;

        coordText.enabled = false;
    }

    public override void OnTowerSelected(Tower tower)
    {
        base.OnTowerSelected(tower);

        if (!Settings.ModEnabled) return;

        var pos = tower.Position;
        ModHelper.Msg<CoordDisplay>($"({pos.X}, {pos.Y})");
    }
}