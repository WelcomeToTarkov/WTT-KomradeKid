#if !UNITY_EDITOR
using Comfort.Common;
using CommonAssets.Scripts.Game;
using EFT;
using EFT.Console.Core;
using EFT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using SPT.Reflection.Patching;

namespace GameBoyEmulator.Utils
{
    public class CommandProcessor
    {
        public void RegisterCommandProcessor()
        {
            ConsoleScreen.Processor.RegisterCommand("clear", delegate ()
            {
                MonoBehaviourSingleton<PreloaderUI>.Instance.Console.Clear();
            });
#if DEBUG
            ConsoleScreen.Processor.RegisterCommand("StartGameBoyEmulator", delegate ()
            {
                GameBoyEmulatorUI.Instance.Show();
            });

            ConsoleScreen.Processor.RegisterCommand("CloseGameBoyEmulator", delegate ()
            {
                GameBoyEmulatorUI.Instance.Hide();
            });
            ConsoleScreen.Processor.RegisterCommand("KillGameBoyEmulator", delegate ()
            {
                GameBoyEmulatorUI.Instance.DestroyUIElements();
            });
#endif
        }
    }
}
#endif
