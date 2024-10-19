using EFT;
using EFT.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameBoyEmulator.Utils
{
    public class InventoryControllerAccessor
    {
        public static InventoryControllerClass GetInventoryControllerClass(ItemUiContext _itemUIContext)
        {
            Type contextType = _itemUIContext.GetType();
            FieldInfo inventoryControllerField = contextType.GetField("inventoryControllerClass", BindingFlags.NonPublic | BindingFlags.Instance);

            if (inventoryControllerField != null)
            {
                return inventoryControllerField.GetValue(_itemUIContext) as InventoryControllerClass;
            }

            return null;
        }

    }
    internal class CommonUtils
    {
        public static Transform FindChildRecursive(Transform parent, string childName)
        {
            return parent.Cast<Transform>()
                            .FirstOrDefault(t => t.name.Equals(childName, StringComparison.OrdinalIgnoreCase))
                            ?? parent.Cast<Transform>()
                                    .Select(t => FindChildRecursive(t, childName))
                                    .FirstOrDefault(t => t != null);
        }

        public static Transform FindDeepChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                Transform result = FindDeepChild(child, childName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static GameObject GetGameBoyEmulatorObject()
        {
            Player player = GameBoyEmulator.player;
            if (player == null)
            {
                return null;
            }

            CustomUsableItemController customUsableItemController = player.HandsController as CustomUsableItemController;

            if (customUsableItemController == null)
            {
                return null;
            }
            GameObject controllerObject = customUsableItemController.ControllerGameObject;
            if (controllerObject == null)
            {
                return null;
            }

            WeaponPrefab weaponPrefab = controllerObject?.GetComponent<WeaponPrefab>();
            if (weaponPrefab == null)
            {
                return null;
            }

            Transform emulatorTransform = FindDeepChild(controllerObject.transform, "gameboy_emulator");
            if (emulatorTransform == null)
            {
                return null;
            }

            GameObject emulatorGameobject = emulatorTransform.gameObject;

            return emulatorGameobject;

        }
        public static void AddEntryToResourcesDictionary(string key, object value)
        {
            Type type = typeof(CacheResourcesPopAbstractClass);
            FieldInfo dictionaryField = type.GetField("dictionary_0", BindingFlags.NonPublic | BindingFlags.Static);
            if (dictionaryField != null)
            {
                var dictionary = dictionaryField.GetValue(null) as Dictionary<string, object>;
                if (dictionary != null)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, value);
#if DEBUG
                        Console.WriteLine($"Successfully added {key} to resources dictionary!");
#endif
                    }
                }
            }
        }

        public static void CreateSlotSprites()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePathPrefix = "GameBoyEmulator.Images.slots.";

            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.StartsWith(resourcePathPrefix) && resourceName.EndsWith(".png"))
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null)
                        {
                            Debug.LogWarning($"Resource {resourceName} not found.");
                            continue;
                        }

                        byte[] imageData = new byte[stream.Length];
                        stream.Read(imageData, 0, imageData.Length);

                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite sprite = Sprite.Create(texture,
                                new Rect(0, 0, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                100.0f);

                            string imageName = resourceName.Replace(resourcePathPrefix, "").Replace(".png", "");
                            string key = $"Slots/{imageName}";

                            AddEntryToResourcesDictionary(key, sprite);
                        }
                        else
                        {
                            ConsoleScreen.LogError($"Failed to load image data for {resourceName}");
                        }
                    }
                }
            }
        }
    
    
    }
}
