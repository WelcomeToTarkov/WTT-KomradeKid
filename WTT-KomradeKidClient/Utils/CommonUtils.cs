#if !UNITY_EDITOR
using EFT;
using EFT.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EFT.InventoryLogic;
using GameBoyEmulator.CustomEFTData;
using UnityEngine;
namespace GameBoyEmulator.Utils
{
    public abstract class InventoryControllerAccessor
    {
        public static InventoryController GetInventoryControllerClass(ItemUiContext __itemUIContext)
        {
            Type contextType = __itemUIContext.GetType();
            FieldInfo inventoryControllerField = contextType.GetField("inventoryController_0", BindingFlags.NonPublic | BindingFlags.Instance);

            if (inventoryControllerField != null)
            {
                return inventoryControllerField.GetValue(__itemUIContext) as InventoryController;
            }
            return null;
        }

    }
    internal abstract class CommonUtils
    {
        public static Transform FindChildRecursive(Transform parent, string childName)
        {
            return parent.Cast<Transform>()
                            .FirstOrDefault(t => t.name.Equals(childName, StringComparison.OrdinalIgnoreCase))
                            ?? parent.Cast<Transform>()
                                    .Select(t => FindChildRecursive(t, childName))
                                    .FirstOrDefault(t => t);
        }

        private static Transform FindDeepChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                Transform result = FindDeepChild(child, childName);
                if (result)
                {
                    return result;
                }
            }
            return null;
        }

        public static GameObject GetGameBoyEmulatorObject(CustomUsableItemController controller = null)
        {
            // If controller is passed, use it directly (preferred)
            if (controller != null)
            {
                GameObject controllerObject = controller.ControllerGameObject;
        
                if (controllerObject == null)
                {
#if DEBUG
                    Console.WriteLine("[GameBoy] ControllerGameObject is null");
#endif
                    return null;
                }

                Transform emulatorTransform = FindDeepChild(controllerObject.transform, "gameboy_emulator");
        
                if (emulatorTransform == null)
                {
#if DEBUG
                    Console.WriteLine("[GameBoy] Emulator transform not found");
#endif
                    return null;
                }

                return emulatorTransform.gameObject;
            }

            // Fallback: Look it up from player (for other use cases)
            Player player = KomradeClient.Player;

#if DEBUG
            Console.WriteLine($"[GameBoy] Player object found: {player != null}");
#endif

            if (!player)
            {
#if DEBUG
                Console.WriteLine("[GameBoy] Player object is null. Returning null.");
#endif
                return null;
            }

            CustomUsableItemController customUsableItemController = player.HandsController as CustomUsableItemController;

#if DEBUG
            Console.WriteLine($"[GameBoy] HandsController cast result: {customUsableItemController != null}");
            if (customUsableItemController == null && player.HandsController != null)
            {
                Console.WriteLine($"[GameBoy] HandsController type: {player.HandsController.GetType().FullName}");
            }
#endif

            if (!customUsableItemController)
            {
                return null;
            }

            return GetGameBoyEmulatorObject(customUsableItemController); // Recursive call with controller
        }
    }
}
#endif
