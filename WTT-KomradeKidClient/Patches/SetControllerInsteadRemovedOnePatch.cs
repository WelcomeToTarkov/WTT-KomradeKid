#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System.Reflection;
using GameBoyEmulator.CustomEFTData;
using HarmonyLib;

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
            Player.Class1346 @class = new Player.Class1346();
            var equipment = __instance.InventoryController.Inventory.Equipment;
            var containedItem = equipment.GetSlot(EquipmentSlot.Scabbard)?.ContainedItem;

            if (containedItem is CustomUsableItem && removingItem != containedItem)
            {
                __instance.Proceed<CustomUsableItemController>(containedItem, new Callback<GInterface202>(@class.method_0), false);
                return false;
            }

            return true;
        }
    }
}
#endif