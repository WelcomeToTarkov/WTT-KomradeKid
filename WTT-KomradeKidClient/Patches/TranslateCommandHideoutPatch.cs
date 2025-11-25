#if !UNITY_EDITOR
using Comfort.Common;
using EFT.InventoryLogic;
using EFT;
using SPT.Reflection.Patching;
using System.Reflection;
using EFT.InputSystem;
using System;
using EFT.UI;
using GameBoyEmulator.CustomEFTData;
using System.Collections.Generic;
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
            
            if (hideoutPlayer.HandsController != null && hideoutPlayer.HandsController is CustomUsableItemController customUsableItemController)
            {
                if (customUsableItemController != null)
                {
                    switch (command)
                    {
                        case ECommand.ExamineWeapon:
                            customUsableItemController.ExamineWeapon();
                            break;
                        case ECommand.ToggleAlternativeShooting:
                            customUsableItemController.ToggleAim();
                            __result = InputNode.ETranslateResult.Block;
                            return false;
                        case ECommand.Escape:
                            hideoutPlayer.SetEmptyHands(method_2);
                            break;
                    }
                }
            }

            if (command is >= ECommand.SelectFastSlot4 and <= ECommand.SelectFastSlot0)
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
            }
            return true;
        }

        private static async void ProceedItemGameBoy(HideoutPlayer hideoutPlayer, Item boundItemObj)
        {
            ConsoleScreen.LogWarning($"[GameBoy] Original item ID: {boundItemObj.Id}");
            
            // DON'T wait for smethod_2 - this orphans the item!
            // Instead, clone it BEFORE calling smethod_2
            CustomUsableItem clonedItem = (CustomUsableItem)boundItemObj.CloneItemWithSameId();
            if (clonedItem == null)
            {
                ConsoleScreen.LogError($"[GameBoy] Failed to clone item");
                return;
            }
            
            ConsoleScreen.LogWarning($"[GameBoy] Cloned item ID: {clonedItem.Id}");

            await HideoutPlayer.smethod_2(hideoutPlayer.Profile, JobPriorityClass.Immediate);
            
            ConsoleScreen.LogWarning($"[GameBoy] After smethod_2, using cloned item");

            InventoryScreenQuickAccessPanel inventoryScreenQuickAccessPanel = Singleton<CommonUI>.Instance.EftBattleUIScreen.QuickAccessPanel;
            inventoryScreenQuickAccessPanel.Show(hideoutPlayer.InventoryController, ItemUiContext.Instance);
            inventoryScreenQuickAccessPanel.AnimatedShow(true);
            
            if (clonedItem != null && clonedItem.CheckAction(null).Succeeded && !hideoutPlayer.InventoryController.IsChangingWeapon && (!hideoutPlayer.IsInBufferZone || hideoutPlayer.CanManipulateWithHandsInBufferZone))
            {
                TryProceedPatch.ProceedCustomUsableItem(clonedItem, method_131, true);
                return;
            }
            hideoutPlayer.SetItemInHands(clonedItem, method_131);
        }

        private static void method_131(Result<IHandsController> result)
        {
            if (result.Failed)
            {
                ConsoleScreen.LogError($"[GameBoy] SetItemInHands failed: {result.Error}");
            }
            else
            {
                ConsoleScreen.LogWarning($"[GameBoy] SetItemInHands succeeded");
            }
        }

        private static void method_2(Result<GInterface198> callback)
        {
            Complete(null);
        }

        private static void Complete(string error)
        {

        }
    }
}
#endif