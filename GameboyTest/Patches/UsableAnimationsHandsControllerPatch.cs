#if !UNITY_EDITOR
using Comfort.Common;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace GameBoyEmulator.Patches
{
    internal class UsableAnimationsHandsControllerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GClass2389).GetMethod("smethod_0", BindingFlags.Static | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool Prefix(ref GInterface241 __result, Item item)
        {
            if (item is CustomUsableItem)
            {
                __result = new GClass2392();
                return false;
            }
            return true;
        }
    }
}
#endif