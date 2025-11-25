#if !UNITY_EDITOR
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System.Reflection;
using GameBoyEmulator.CustomEFTData;

namespace GameBoyEmulator.Patches
{
    internal class UsableAnimationsHandsControllerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GClass2970).GetMethod("smethod_0", BindingFlags.Static | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool Prefix(ref GInterface323 __result, Item item)
        {
            if (item is CustomUsableItem)
            {
                __result = new CustomUsableItemInterfaceClass();
                return false;
            }
            return true;
        }
    }
}
#endif