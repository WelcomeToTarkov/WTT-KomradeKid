#if !UNITY_EDITOR
using Comfort.Common;
using EFT.InventoryLogic;
using EFT;
using SPT.Reflection.Patching;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using GameBoyEmulator.CustomEFTTypes;
using EFT.AssetsManager;
using EFT.InputSystem;
using System;
using GameBoyEmulator.Utils;
using EFT.UI;
using System.Linq;


namespace GameBoyEmulator.Patches
{
    internal class TranslateCommandHideoutPatch : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            return typeof(HideoutPlayerOwner).GetMethod(nameof(HideoutPlayerOwner.TranslateCommand));
        }

        [PatchPrefix]
        public static bool PatchPrefix(HideoutPlayerOwner __instance, ECommand command, ref InputNode.ETranslateResult __result)
        {
            HideoutPlayer hideoutPlayer = __instance.HideoutPlayer;
            
            if (hideoutPlayer.HandsController != null && hideoutPlayer.HandsController is CustomUsableItemController)
            {
                CustomUsableItemController customUsableItemController = hideoutPlayer.HandsController as CustomUsableItemController;

                if (command == ECommand.ExamineWeapon)
                {
                    customUsableItemController.ExamineWeapon();                
                }
                if (command == ECommand.ToggleAlternativeShooting)
                {
                    customUsableItemController.ToggleAim();
                }
                if (command == ECommand.Escape)
                {
                    hideoutPlayer.SetEmptyHands(new Callback<GInterface137>(method_2));
                }
            }

            if (command >= ECommand.SelectFastSlot4 && command <= ECommand.SelectFastSlot0)
            {
                EBoundItem boundItem = command switch
                {
                    ECommand.SelectFastSlot4 => EBoundItem.Item4,
                    ECommand.SelectFastSlot5 => EBoundItem.Item5,
                    ECommand.SelectFastSlot6 => EBoundItem.Item6,
                    ECommand.SelectFastSlot7 => EBoundItem.Item7,
                    ECommand.SelectFastSlot8 => EBoundItem.Item8,
                    ECommand.SelectFastSlot9 => EBoundItem.Item9,
                    ECommand.SelectFastSlot0 => EBoundItem.Item10,
                    _ => throw new ArgumentOutOfRangeException() 
                };

                Item boundItemObj = hideoutPlayer.Inventory.FastAccess.GetBoundItem(boundItem);

                if (boundItemObj is CustomUsableItem)
                {
                    ProceedItemGameBoy(hideoutPlayer, boundItemObj);
                    return false;
                }
                return true;
            }
            return true;
        }

        public static async void ProceedItemGameBoy(HideoutPlayer hideoutPlayer, Item boundItemObj)
        {
            await HideoutPlayer.smethod_2(hideoutPlayer.Profile, JobPriority.Immediate);
            InventoryScreenQuickAccessPanel inventoryScreenQuickAccessPanel = Singleton<CommonUI>.Instance.EftBattleUIScreen.QuickAccessPanel;
            inventoryScreenQuickAccessPanel.Show(hideoutPlayer.InventoryControllerClass, ItemUiContext.Instance);
            inventoryScreenQuickAccessPanel.AnimatedShow(true);
            hideoutPlayer.SetItemInHands(boundItemObj, new Callback<IHandsController>(method_131));
        }
        public static void method_131(Result<IHandsController> result)
        {
            if (result.Failed)
            {
                ConsoleScreen.LogError(result.Error);
            }
        }
        public static void method_2(Result<GInterface137> callback)
        {
            Complete(null);
        }
        public static void Complete(string error)
        {

        }
    }
}
#endif