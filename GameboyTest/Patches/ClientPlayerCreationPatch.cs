﻿#if !UNITY_EDITOR
using Comfort.Common;
using EFT.InventoryLogic;
using EFT;
using SPT.Reflection.Patching;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using GameBoyEmulator.CustomEFTTypes;


namespace GameBoyEmulator.Patches
{
#if DEBUG

    internal class ClientPlayerCreationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ClientPlayer).GetMethod(nameof(ClientPlayer.method_147));
        }

        [PatchPrefix]
        public static bool PatchPrefix(ClientPlayer __instance, Profile profile, MongoID firstId, Quaternion rotation, bool isAlive, EHandsControllerType type, bool isInSpawnOperation, string itemId, byte[] healthState, bool isInPronePose, float poseLevel, bool isStationaryWeapon, Vector2 stationaryRotation, Quaternion playerStationaryRotation, int animationVariant, Player.EVoipState voipState, bool isInBufferZone, int bufferZoneUsageTimeLeft, bool leftStance, Callback callback)
        {
            return true;
        }
}
#endif

}
#endif