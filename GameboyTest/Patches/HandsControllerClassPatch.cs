#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace GameBoyEmulator.Patches
{
    internal class HandsControllerClassPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(HandsControllerClass).GetMethod("method_41", BindingFlags.Instance | BindingFlags.Public);
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