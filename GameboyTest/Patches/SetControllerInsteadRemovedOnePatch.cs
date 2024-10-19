#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using GameBoyEmulator.CustomEFTTypes;

namespace GameBoyEmulator.Patches
{
    internal class SetControllerInsteadRemovedOnePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("SetControllerInsteadRemovedOne", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool PatchPrefix(Player __instance, Item removingItem, Callback callback)
        {
            Player.Class1116 @class = new Player.Class1116();
            @class.player_0 = GameBoyEmulator.player;
            @class.callback = callback;

            var equipment = __instance.InventoryControllerClass.Inventory.Equipment;
            var containedItem = equipment.GetSlot(EquipmentSlot.Scabbard)?.ContainedItem;

            if (containedItem is CustomUsableItem customUsableItem && removingItem != containedItem)
            {
                __instance.Proceed<CustomUsableItemController>(containedItem, new Callback<GInterface141>(@class.method_2), false);
                return false;
            }

            return true;
        }
    }
}
#endif