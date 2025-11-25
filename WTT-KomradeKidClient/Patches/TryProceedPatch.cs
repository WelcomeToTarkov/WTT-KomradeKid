#if !UNITY_EDITOR
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System.Reflection;
using GameBoyEmulator.CustomEFTData;

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
                Player.Class1344 @class = new Player.Class1344
                {
                    completeCallback = completeCallback,
                    player_0 = KomradeClient.Player
                };
                __instance.StopBlindFire();
                __instance.RemoveLeftHandItem();
                __instance.method_126();
                if (!__instance.InventoryController.IsAtReachablePlace(item))
                {
                    __instance.SetFirstAvailableItem(@class.completeCallback);
                    return false;
                }
                
                Player.MedsController medsController = __instance.HandsController as Player.MedsController;
                if (medsController != null && !__instance.IsAI)
                {
                    medsController.SetOnUsedCallback(new Callback<IOnHandsUseCallback>(Player.Class1318.class1318_0.method_24));
                }

                Player.Class1345 class2 = new Player.Class1345();
                class2.class1344_0 = @class;
                ProceedCustomUsableItem(customItem, class2.class1344_0.completeCallback, scheduled);
                return false;
            }

            return true;
        }

        public static void ProceedCustomUsableItem(CustomUsableItem item, Callback<IHandsController> completeCallback, bool scheduled = true)
        {
            Player.Class1346 @class = new Player.Class1346();
            {
                completeCallback = completeCallback;
            };

            if (KomradeClient.Player is ClientPlayer)
            {
                KomradeClient.Player.Proceed<ClientCustomUsableItemController>(item, @class.method_0, scheduled);
                return;
            }
            KomradeClient.Player.Proceed<CustomUsableItemController>(item, @class.method_1, scheduled);
        }

    }
}
#endif