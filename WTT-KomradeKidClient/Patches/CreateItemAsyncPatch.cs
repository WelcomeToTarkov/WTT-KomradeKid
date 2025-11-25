#if !UNITY_EDITOR
using EFT.AssetsManager;
using EFT.CameraControl;
using EFT.InventoryLogic;
using EFT;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using GameBoyEmulator.CustomEFTData;
using SPT.Reflection.Patching;

namespace GameBoyEmulator.Patches
{
internal class CreateItemAsyncPatch : ModulePatch
{
protected override MethodBase GetTargetMethod()
{
return typeof(PoolManagerClass).GetMethod("CreateItemAsync", BindingFlags.Instance | BindingFlags.Public);
}

    [PatchPrefix]
    public static bool PatchPrefix(PoolManagerClass __instance, Item item, ECameraType cameraType, [CanBeNull] IPlayer player, bool isAnimated, GDelegate62 yield, CancellationToken ct, ref Task<GameObject> __result)
    {
        if (item == null)
        {
            return true;
        }

        if (item is CustomUsableItem || item is GameBoyCartridge || item is GameBoyAccessory)
        {
            __result = CustomCreateItemAsync(__instance, item, cameraType, player, isAnimated, yield, ct);
            return false;
        }
        return true;
    }
    
    private static async Task<GameObject> CustomCreateItemAsync(PoolManagerClass __instance, Item item, ECameraType cameraType, [CanBeNull] IPlayer player, bool isAnimated, GDelegate62 yield, CancellationToken ct)
    {
        // Updated to use Class1455 instead of Class1318
        PoolManagerClass.Class1455 @class = new PoolManagerClass.Class1455();
        PoolManagerClass.PoolsCategory poolCategory = PoolManagerClass.PoolsCategory.Raid;

        // Updated field name from dictionary_2 to Dictionary_2 (capital D)
        var dictionaryField = typeof(PoolManagerClass).GetField("Dictionary_2", BindingFlags.NonPublic | BindingFlags.Instance);
        if (dictionaryField != null)
        {
            var dictionary = (Dictionary<PoolManagerClass.PoolsCategory, CancellationToken>)dictionaryField.GetValue(__instance);
            if (dictionary.TryGetValue(PoolManagerClass.PoolsCategory.Raid, out var token))
            {
                ct = CancellationTokenSource.CreateLinkedTokenSource(token, ct).Token;
            }
        }

        if (ct.IsCancellationRequested)
        {
            return null;
        }

        @class.itemGameObject = __instance.method_2(item.Prefab, poolCategory);
        if (@class.itemGameObject == null)
        {
            PoolManagerClass.Logger.LogError($"Failed to create GameObject for item: {item}", Array.Empty<object>());
            return null;
        }

        // Updated cancellation registration to use method_0 instead of direct Destroy
        var cancellationTokenRegistration = ct.Register(new Action(@class.method_0));

        if (@class.itemGameObject.scene.buildIndex != -1 && @class.itemGameObject.transform.parent == null && Application.isPlaying)
        {
            UnityEngine.Object.DontDestroyOnLoad(@class.itemGameObject);
        }
        @class.itemGameObject.transform.localScale = Vector3.one;

        await yield(null);

        if (ct.IsCancellationRequested)
        {
            return null;
        }

        AssetPoolObject component = @class.itemGameObject.GetComponent<AssetPoolObject>();

        if (component == null)
        {
            PoolManagerClass.Logger.LogError($"No AssetPoolObject found for item: {item}", Array.Empty<object>());
            return null;
        }

        Transform weaponHierarchy = null;
        bool flag = item is CustomUsableItem;
        if (flag)
        {
            weaponHierarchy = (component as WeaponPrefab)?.Hierarchy.transform;
        }

        await yield(null);

        if (ct.IsCancellationRequested)
        {
            return null;
        }

        // Updated from GClass2981 to GClass3248 (but check actual decompiled code for exact class name)
        GClass3248 collection = item as GClass3248;
        bool flag2 = false;
        if (collection != null && !(item is CylinderMagazineItemClass))
        {
            flag2 = await __instance.method_4(collection, flag, cameraType, player, isAnimated, @class.itemGameObject, component, weaponHierarchy, ct, yield);
        }

        if (ct.IsCancellationRequested)
        {
            return null;
        }

        if (flag)
        {
            WeaponPrefab weaponPrefab = component as WeaponPrefab;
            if (weaponPrefab != null)
            {
                weaponPrefab.Init(player, player != null);
                if (flag2 && player != null)
                {
                    weaponPrefab.RebindAnimator(player);
                }
            }
        }
        else
        {
            if (item is GameBoyCartridge gameboyCartridge)
            {
                gameboyCartridge.ApplyStickerTexture(@class.itemGameObject);
                @class.itemGameObject.name = @class.itemGameObject.name.Replace("(Clone)", string.Empty);
            }

            if (item is GameBoyAccessory)
            {
                @class.itemGameObject.name = @class.itemGameObject.name.Replace("(Clone)", string.Empty);
            }
        }

        if (ct.IsCancellationRequested)
        {
            return null;
        }

        GInterface236[] components = @class.itemGameObject.GetComponents<GInterface236>();
        foreach (var comp in components)
        {
            comp.Init(item, isAnimated);
        }

        Mod mod;
        if ((mod = (item as Mod)) != null && mod.IsAnimated)
        {
            @class.itemGameObject.name = @class.itemGameObject.name.Replace("(Clone)", string.Empty);
        }

        cancellationTokenRegistration.Dispose();
        return @class.itemGameObject;
    }
}
}
#endif