#if !UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

namespace GameBoyEmulator.Patches
{
    internal class IsAtBindablePlacePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(InventoryControllerClass).GetMethod("IsAtBindablePlace", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool Prefix(InventoryControllerClass __instance, Item item, ref bool __result)
        {
            if (item is CustomUsableItem)
            {
                InventoryControllerClass.Class2093 @class = new InventoryControllerClass.Class2093();
                @class.inventoryControllerClass = __instance;

                if (item.CurrentAddress != null && !(item.Parent is GClass2782))
                {
                    ItemAddress currentAddress = item.Parent.Container.ParentItem.CurrentAddress;
                    @class.parentSlot = (((currentAddress != null) ? currentAddress.Container : null) as Slot);
                    LootItemClass lootItemClass = item as LootItemClass;
                    __result = Inventory.FastAccessSlots
                        .Select(new Func<EquipmentSlot, Slot>(@class.method_0))
                        .Any(new Func<Slot, bool>(@class.method_1))
                        && (lootItemClass == null || !lootItemClass.MissingVitalParts.Any<Slot>())
                        && __instance.Examined(item)
                        && (item is Weapon || item is GrenadeClass || item.GetItemComponent<KnifeComponent>() != null ||
                            item is MedsClass || item is FoodClass || item is GClass2749 || item is GClass2747 ||
                            item is RecodableItemClass || item is CustomUsableItem);

                    return false;
                }
            }

            return true;
        }
    }

}
#endif