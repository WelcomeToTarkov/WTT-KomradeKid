#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System.Reflection;
using GameBoyEmulator.CustomEFTTypes;


namespace GameBoyEmulator.Patches
{
    internal class SetHandsUsableItemPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod(nameof(Player.SetInHandsUsableItem), BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool PatchPrefix(Player __instance, ref Item item, ref Callback<GInterface141> callback)
        {
            if (item is CustomUsableItem)
            {
                __instance.Proceed<CustomUsableItemController>(item, callback, true);
                return false;
            }

            return true;
        }
    }
}
#endif