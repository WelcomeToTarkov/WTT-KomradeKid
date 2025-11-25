using System;
using System.IO;
using System.Reflection;
#if !UNITY_EDITOR
using EFT.UI;
#endif
using UnityGB;

public class DefaultSaveMemory : ISaveMemory
{
    public void Save(string name, byte[] data)
    {
        if (data == null)
        {
            Console.WriteLine($"Save operation aborted: data for '{name}' is null.");
            return;
        }

        string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string path = Path.Combine(pluginPath ?? throw new InvalidOperationException(), "Saves", name + ".sav");

        try
        {
            Directory.CreateDirectory(Path.Combine(pluginPath, "Saves")); // Ensure the directory exists
            File.WriteAllBytes(path, data);
            Console.WriteLine($"Successfully saved data for '{name}' at '{path}'. Size: {data.Length} bytes.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't save save file for '{name}'.");
            Console.WriteLine(e.Message);
        }
    }

    public byte[] Load(string name)
    {
        string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string path = Path.Combine(pluginPath ?? throw new InvalidOperationException(), "Saves", name + ".sav");

        if (!File.Exists(path))
        {
            Console.WriteLine($"No save file could be found for '{name}' at '{path}'.");
            return null;
        }

        byte[] data = null;
        try
        {
            data = File.ReadAllBytes(path);
            Console.WriteLine($"Successfully loaded data for '{name}' from '{path}'. Size: {data.Length} bytes.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't load save file for '{name}'.");
            Console.WriteLine(e.Message);
        }

        return data;
    }
}
