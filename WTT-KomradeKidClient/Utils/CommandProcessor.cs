#if !UNITY_EDITOR
using EFT.UI;

namespace GameBoyEmulator.Utils
{
    public class CommandProcessor
    {
        public void RegisterCommandProcessor()
        {
            ConsoleScreen.Processor.RegisterCommand("clear", delegate
            {
                MonoBehaviourSingleton<PreloaderUI>.Instance.Console.Clear();
            });
        }
    }
}
#endif
