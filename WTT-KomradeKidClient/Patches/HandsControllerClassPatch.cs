#if !UNITY_EDITOR
using EFT;
using SPT.Reflection.Patching;
using System.Reflection;
using GameBoyEmulator.CustomEFTData;

namespace GameBoyEmulator.Patches
{
    internal class HandsControllerClassPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(HandsControllerClass).GetMethod("method_49", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool Prefix(ref PlayerAnimator.EWeaponAnimationType __result, HandsControllerClass __instance)
        {
            if (__instance.ItemInHands is CustomUsableItem)
            {
                __result = PlayerAnimator.EWeaponAnimationType.Pistol;
                return false;
            }

            return true;
        }
    }
}
#endif