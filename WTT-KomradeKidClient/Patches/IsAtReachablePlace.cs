#if !UNITY_EDITOR
using System.Linq;
using System.Reflection;
using EFT.InventoryLogic;
using GameBoyEmulator.CustomEFTData;
using SPT.Reflection.Patching;

namespace GameBoyEmulator.Patches;

internal class IsAtReachablePlace : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(InventoryController).GetMethod("IsAtReachablePlace", BindingFlags.Instance | BindingFlags.Public);
    }

    [PatchPostfix]
    public static void Postfix(InventoryController __instance, Item item, ref bool __result)
    {
        if (item is CustomUsableItem usableItem)
        {
            
            if (usableItem.CurrentAddress == null)
            {
                return;
            }
            EFT.InventoryLogic.IContainer container = usableItem.Parent.Container;
            if (__instance.Inventory.Stash == null || container != __instance.Inventory.Stash.Grid)
            {
                CompoundItem compoundItem = item as CompoundItem;
                if ((compoundItem == null || !compoundItem.MissingVitalParts.Any<Slot>()) && __instance.Inventory.GetItemsInSlots(Inventory.BindAvailableSlotsExtended).Contains(item) && __instance.Examined(item))
                {
                    __result = true;
                }
            }
        }
    }
}
#endif