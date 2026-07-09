using AOSharp.Core;
using AOSharp.Core.UI;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Shared
{
    public abstract class BuddyBaseConfig<T> where T : new()
    {
        public abstract string FileName { get; }
        private static string _savePath;

        public T Load(string pluginDir)
        {
            try
            {
                _savePath = $"{pluginDir}\\{FileName}.json";

                if (File.Exists(_savePath))
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(_savePath));
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_savePath));
                    T defaults = LoadDefaults;
                    if (defaults != null && FileName != null)
                        File.WriteAllText(_savePath, JsonConvert.SerializeObject(defaults, Formatting.Indented));
                    return defaults;
                }
            }
            catch (Exception ex)
            {
                Chat.WriteLine(ex.Message);
                return default;
            }
        }


        public void Save()
        {
            try
            {
                File.WriteAllText(_savePath, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Chat.WriteLine(ex.Message);
            }
        }

        public static T LoadConfig(string playerSettingsPath)
        {
            BuddyBaseConfig<T> instance = Activator.CreateInstance<T>() as BuddyBaseConfig<T>;
            return instance.Load(playerSettingsPath);
        }

        public abstract T LoadDefaults { get; }
    }
}