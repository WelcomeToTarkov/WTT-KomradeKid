#if !UNITY_EDITOR
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

namespace GameBoyEmulator.Patches
{
    internal class GetWeaponAnimationTypePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("GetWeaponAnimationType", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool Prefix(Player __instance, Player.AbstractHandsController handsController, ref PlayerAnimator.EWeaponAnimationType __result)
        {
            if (handsController == null || handsController.Item == null || handsController is Player.EmptyHandsController)
            {
                return true; 
            }

            Item item = handsController.Item;

            if (item is CustomUsableItem)
            {
                __result = PlayerAnimator.EWeaponAnimationType.Pistol;
                return false;
            }
            return true;
        }
    }
}
#endif