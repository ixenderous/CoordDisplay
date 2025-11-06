using MelonLoader;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using CoordDisplay;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(CoordDisplay.CoordDisplay), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace CoordDisplay
{
    public class CoordDisplay : BloonsTD6Mod
    {
        private bool isInGame = false;

        private GameObject canvasGO;
        private TextMeshProUGUI coordText;

        // Cached shared font and material from the game UI
        private static TMP_FontAsset sharedFont;
        private static Material sharedFontMaterial;

        public override void OnMatchStart()
        {
            if (sharedFont == null)
                StealText();
            
            isInGame = true;
        }

        public override void OnMatchEnd()
        {
            base.OnMatchEnd();

            isInGame = false;
            DestroyUI();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!Settings.ModEnabled || !isInGame || InGame.instance?.inputManager == null)
                return;

            CreateUI();

            if (Settings.MoveTextHotkey.JustPressed())
                SetTextPosition();

            UpdateText();
        }

        private void CreateUI()
        {
            if (canvasGO != null)
                return; // already created

            canvasGO = new GameObject("CoordCanvas");
            Object.DontDestroyOnLoad(canvasGO);

            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var textGO = new GameObject("CoordText");
            textGO.transform.SetParent(canvasGO.transform, false);

            coordText = textGO.AddComponent<TextMeshProUGUI>();
            coordText.fontSize = 20;
            coordText.alignment = TextAlignmentOptions.Center;
            coordText.enableWordWrapping = false;
            coordText.overflowMode = TextOverflowModes.Overflow;
            coordText.fontStyle = FontStyles.Bold;
            coordText.color = Color.white;

            // Apply stolen font setup if available
            if (sharedFont != null)
            {
                coordText.font = sharedFont;
                coordText.fontMaterial = sharedFontMaterial;
            }
            else
            {
                // ModHelper.Msg<CoordDisplay>("No shared font found yet — using default TMP font.");
            }

            coordText.enabled = false;
        }

        private void DestroyUI()
        {
            if (canvasGO != null)
            {
                Object.Destroy(canvasGO);
                canvasGO = null;
                coordText = null;
            }
        }

        public override void OnTowerSelected(Tower tower)
        {
            base.OnTowerSelected(tower);

            if (!Settings.LogPositions)
                return;

            var pos = tower.Position;
            ModHelper.Msg<CoordDisplay>($"{tower.towerModel.name} selected at ({pos.X}, {pos.Y})");
        }

        private void StealText()
        {
            var ui = InGame.instance?.GetInGameUI();
            if (ui == null)
            {
                // ModHelper.Msg<CoordDisplay>("UI not found");
                return;
            }

            var tmp = ui.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp == null)
            {
                // ModHelper.Msg<CoordDisplay>("Couldn't find any TMP component in UI");
                return;
            }

            sharedFont = tmp.font;
            sharedFontMaterial = tmp.fontMaterial;

            // ModHelper.Msg<CoordDisplay>($"Stole TMP font: {sharedFont?.name ?? "null"}");
            // ModHelper.Msg<CoordDisplay>($"Material: {sharedFontMaterial?.name ?? "null"}");
        }

        private void SetTextPosition()
        {
            var mousePos = Input.mousePosition;
            Settings.X.SetValueAndSave((long)mousePos.x);
            Settings.Y.SetValueAndSave((long)mousePos.y);

            // ModHelper.Msg<CoordDisplay>($"Moved text to X: {Settings.X.GetValue()} Y: {Settings.Y.GetValue()}");
        }

        private void UpdateText()
        {
            var input = InGame.instance.inputManager;
            var placementModel = input.placementModel;
            if (placementModel == null)
            {
                coordText.enabled = false;
                return;
            }

            var towerPos = input.entityPositionWorld;
            var mousePos = Input.mousePosition;
            
            coordText.text =
                $"Mouse: ({mousePos.x}, {mousePos.y})\n" +
                $"Tower: ({towerPos.x}, {towerPos.y})";

            coordText.rectTransform.position = new Vector3(Settings.X, Settings.Y, 0);
            coordText.enabled = true;
        }
    }
}
