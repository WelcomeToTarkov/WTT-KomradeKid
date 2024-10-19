using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using EFT.UI;
using UnityGB;

public class DefaultSaveMemory : ISaveMemory
{
    public void Save(string name, byte[] data)
    {
        if (data == null)
        {
            ConsoleScreen.LogWarning($"Save operation aborted: data for '{name}' is null.");
            return;
        }

        string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string path = System.IO.Path.Combine(pluginPath, "Saves", name + ".sav");

        try
        {
            Directory.CreateDirectory(Path.Combine(pluginPath, "Saves")); // Ensure the directory exists
            File.WriteAllBytes(path, data);
            ConsoleScreen.Log($"Successfully saved data for '{name}' at '{path}'. Size: {data.Length} bytes.");
        }
        catch (System.Exception e)
        {
            ConsoleScreen.LogError($"Couldn't save save file for '{name}'.");
            ConsoleScreen.Log(e.Message);
        }
    }

    public byte[] Load(string name)
    {
        string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string path = System.IO.Path.Combine(pluginPath, "Saves", name + ".sav");

        if (!File.Exists(path))
        {
            ConsoleScreen.LogWarning($"No save file could be found for '{name}' at '{path}'.");
            return null;
        }

        byte[] data = null;
        try
        {
            data = File.ReadAllBytes(path);
            ConsoleScreen.Log($"Successfully loaded data for '{name}' from '{path}'. Size: {data.Length} bytes.");
        }
        catch (System.Exception e)
        {
            ConsoleScreen.LogError($"Couldn't load save file for '{name}'.");
            ConsoleScreen.Log(e.Message);
        }

        return data;
    }
}
