#if !UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

internal class IsAtReachablePlace : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(InventoryControllerClass).GetMethod("IsAtReachablePlace", BindingFlags.Instance | BindingFlags.Public);
    }

    [PatchPrefix]
    public static bool Prefix(InventoryControllerClass __instance, Item item, ref bool __result)
    {
        if (item is CustomUsableItem)
        {
            if (item.CurrentAddress == null)
            {
                __result = false;
                return false;
            }

            EFT.InventoryLogic.IContainer container = item.Parent.Container;
            LootItemClass lootItemClass;
            __result = (__instance.Inventory.Stash == null || container != __instance.Inventory.Stash.Grid)
                        && ((lootItemClass = (item as LootItemClass)) == null || !lootItemClass.MissingVitalParts.Any<Slot>())
                        && __instance.Inventory.GetItemsInSlots(Inventory.BindAvailableSlotsExtended).Contains(item)
                        && __instance.Examined(item)
                        && (item is Weapon || item is GrenadeClass || item.GetItemComponent<KnifeComponent>() != null
                            || item is MedsClass || item is FoodClass || item is GClass2749
                            || item is GClass2747 || item is RecodableItemClass
                            || item is CustomUsableItem);

            return false;
        }

        return true;
    }
}
#endif