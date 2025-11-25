#if !UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using Object = UnityEngine.Object;

namespace GameBoyEmulator.Patches
{
    internal class GameEndedPatch : ModulePatch
    {
        private static Type _targetClassType;

        protected override MethodBase GetTargetMethod()
        {
            _targetClassType = PatchConstants.EftTypes.Single(targetClass =>
                !targetClass.IsInterface &&
                !targetClass.IsNested &&
                targetClass.GetMethods().Any(method => method.Name == "LocalRaidEnded") &&
                targetClass.GetMethods().Any(method => method.Name == "ReceiveInsurancePrices")
            );

            return AccessTools.Method(_targetClassType.GetTypeInfo(), "LocalRaidEnded");
        }

        [PatchPostfix]
        public static void Postfix()
        {
            SaveAllActiveEmulators();

        }

        public static void SaveAllActiveEmulators()
        {
            // Find all active instances of DefaultEmulatorManager in the scene
            DefaultEmulatorManager[] emulators = Object.FindObjectsOfType<DefaultEmulatorManager>();

            foreach (var emulator in emulators)
            {
                // Check if the emulator is on
                if (emulator.emulatorOn)
                {
                    // Call the Save method
                    emulator.Emulator.Save();
                    Console.WriteLine("Saved emulator!");
                }
            }
        }

    }
}
#endif