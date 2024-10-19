#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System.Reflection;
using GameBoyEmulator.CustomEFTTypes;

namespace GameBoyEmulator.Patches
{
    internal class TryProceedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("TryProceed", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool PatchPrefix(Player __instance, Item item, Callback<IHandsController> completeCallback, bool scheduled = true)
        {
            if (item is CustomUsableItem customItem)
            {
                Player.Class1111 @class = new Player.Class1111();
                @class.completeCallback = completeCallback;
                @class.player_0 = GameBoyEmulator.player;
                GameBoyEmulator.player.StopBlindFire();
                GameBoyEmulator.player.method_62();
                Player.Class1112 class2 = new Player.Class1112();
                class2.class1111_0 = @class;
                TryProceedPatch.ProceedCustomUsableItem(item as CustomUsableItem, class2.class1111_0.completeCallback, scheduled);
                return false;
            }

            return true;
        }
        public static void ProceedCustomUsableItem(CustomUsableItem item, Callback<IHandsController> completeCallback, bool scheduled = true)
        {
            Player.Class1113 @class = new Player.Class1113();
            @class.completeCallback = completeCallback;

            if (GameBoyEmulator.player is ClientPlayer)
            {
                GameBoyEmulator.player.Proceed<ClientCustomUsableItemController>(item, new Callback<GInterface141>(@class.method_0), scheduled);
                return;
            }
            GameBoyEmulator.player.Proceed<CustomUsableItemController>(item, new Callback<GInterface141>(@class.method_1), scheduled);
        }

    }
}
#endif