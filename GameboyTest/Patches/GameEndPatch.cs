using EFT;
using GameBoyEmulator.Utils;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityGB;

namespace SimpleCommandUtils.Patches
{
    internal class GameEndedPatch : ModulePatch
    {
        private static Type _targetClassType;

        protected override MethodBase GetTargetMethod()
        {
            _targetClassType = PatchConstants.EftTypes.Single(targetClass =>
                !targetClass.IsInterface &&
                !targetClass.IsNested &&
                targetClass.GetMethods().Any(method => method.Name == "OfflineRaidEnded") &&
                targetClass.GetMethods().Any(method => method.Name == "ReceiveInsurancePrices")
            );

            return AccessTools.Method(_targetClassType.GetTypeInfo(), "OfflineRaidEnded");
        }

        [PatchPostfix]
        public static void Postfix()
        {
            SaveAllActiveEmulators();

        }

        public static void SaveAllActiveEmulators()
        {
            // Find all active instances of DefaultEmulatorManager in the scene
            DefaultEmulatorManager[] emulators = GameObject.FindObjectsOfType<DefaultEmulatorManager>();

            foreach (var emulator in emulators)
            {
                // Check if the emulator is on
                if (emulator._emulatorOn)
                {
                    // Call the Save method
                    emulator.Emulator.Save();
                    Console.WriteLine("Saved emulator!");
                }
            }
        }

    }
}