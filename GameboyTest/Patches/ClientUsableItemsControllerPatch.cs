#if !UNITY_EDITOR
using Comfort.Common;
using EFT.InventoryLogic;
using EFT;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;

namespace GameBoyEmulator.Patches
{
    internal class ClientUsableItemControllerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ClientUsableItemController).GetMethod("smethod_10", BindingFlags.Static | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool Prefix(ref Task<ClientUsableItemController> __result, ClientPlayer player, string itemId)
        {
            Item item = string.IsNullOrEmpty(itemId) ? null : player.InventoryControllerClass.FindItem(itemId);

            if (item is CustomUsableItem customItem)
            {
                __result = Player.UsableItemController.smethod_6<ClientUsableItemController>(player, item);
                return false;
            }

            return true;
        }

    }
}
#endif