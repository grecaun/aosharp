using System;
using System.Collections.Generic;
using System.IO;
using AOSharp.Common.Unmanaged.DataTypes;
using AOSharp.Common.Unmanaged.Interfaces;
using AOSharp.Core.UI;
using Newtonsoft.Json;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public class HandlerSettings
        {
            private readonly string _groupName;
            private readonly string _savePath;
            private List<string> _variables = new List<string>();
            private Dictionary<string, string> _storedValues;
            private Dictionary<string, string> _newValues = new Dictionary<string, string>();

            public Variant this[string name]
            {
                get
                {
                    if (!_variables.Contains(name))
                        return null;

                    return DistributedValue.GetDValue($"{_groupName}__{name}", false);
                }

                set
                {
                    DistributedValue.SetDValue($"{_groupName}__{name}", value);
                }
            }

            public HandlerSettings(string groupName)
            {
                _groupName = groupName;
                _savePath = $"{Preferences.GetCharacterPath()}\\AOSharp\\{groupName}.config";
                Load();
            }

            private void AddVariable(string name)
            {
                _variables.Add(name);
                _newValues[name] = this[name]?.ToString();
            }

            public void AddVariable(string name, int value)
            {
                DistributedValue.Create($"{_groupName}__{name}", value);
                AddVariable(name);
            }

            public void AddVariable(string name, float value)
            {
                DistributedValue.Create($"{_groupName}__{name}", value);
                AddVariable(name);
            }

            public void AddVariable(string name, bool value)
            {
                DistributedValue.Create($"{_groupName}__{name}", value);
                AddVariable(name);
            }

            public void DeleteVariable(string name)
            {
                _variables.Remove(name);
            }

            public void Save()
            {
                Dictionary<string, string> values = new Dictionary<string, string>();

                foreach (string variable in _variables)
                {
                    values.Add(variable, this[variable].ToString());
                }

                new FileInfo(_savePath).Directory.Create();
                File.WriteAllText(_savePath, JsonConvert.SerializeObject(values, Formatting.Indented));
            }


            public void Load()
            {
                try
                {
                    _storedValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_savePath));

                    if (_storedValues == null)
                        throw new Exception("JSON Object is null. Likely due to corrupt config file.");
                }
                catch
                {
                    _storedValues = new Dictionary<string, string>();
                }
            }

            public void Prune()
            {
                if (_storedValues == null) return;

                if (_storedValues.ContainsKey("Version_Number") && _storedValues["Version_Number"] != _newValues["Version_Number"])
                {
                    Chat.WriteLine("Version change detected, pruning settings...");

                    foreach (var key in new List<string>(_storedValues.Keys))
                        if (!_newValues.ContainsKey(key))
                        {
                            Chat.WriteLine($"Removing old key: {key}");
                            _storedValues.Remove(key);
                        }

                    foreach (var kvp in _newValues)
                        if (!_storedValues.ContainsKey(kvp.Key))
                        {
                            _storedValues.Add(kvp.Key, kvp.Value);
                            Chat.WriteLine($"Added new key : {kvp.Key}");
                        }

                    _storedValues["Version_Number"] = _newValues["Version_Number"];
                }

              

                File.WriteAllText(_savePath, JsonConvert.SerializeObject(_storedValues, Formatting.Indented));

                foreach (var kvp in _storedValues)
                    DistributedValue.SetDValue($"{_groupName}__{kvp.Key}", Variant.LoadFromString(kvp.Value));
            }
        }
    }
}
