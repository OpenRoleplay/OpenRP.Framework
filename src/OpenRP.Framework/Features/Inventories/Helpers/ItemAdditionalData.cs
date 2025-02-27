using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Helpers
{
    public class ItemAdditionalData
    {
        // Personalized Additional Data
        public static readonly HashSet<string> _PersonalizedAdditionalData = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "DEFAULT_WALLET",
            "WEARING"
        };

        // Properties
        private string custom_data { get; set; }
        private Dictionary<string, string> custom_data_dictionary { get; set; }

        // Constructor
        private ItemAdditionalData(string custom_data)
        {
            this.custom_data = custom_data;
            this.custom_data_dictionary = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(this.custom_data))
            {
                string[] custom_data_array;
                if (custom_data.Contains(";"))
                {
                    custom_data_array = custom_data.Split(";");
                }
                else
                {
                    custom_data_array = new[] { this.custom_data };
                }

                foreach (string data in custom_data_array)
                {
                    string[] seperate_values = data.Split("=");

                    this.custom_data_dictionary.Add(seperate_values[0].TrimStart('['), seperate_values[1].TrimEnd(']'));
                }
            }
        }

        // Functions
        public Dictionary<string, string> GetAllCustomData()
        {
            return custom_data_dictionary;
        }

        public void SetBoolean(string key, bool value)
        {
            if (value)
            {
                this.custom_data_dictionary[key.ToUpper()] = "1";
            }
            else
            {
                this.custom_data_dictionary.Remove(key.ToUpper());
            }
        }

        public void SetString(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                this.custom_data_dictionary[key.ToUpper()] = value;
            }
            else
            {
                this.custom_data_dictionary.Remove(key.ToUpper());
            }
        }

        public string GetString(string key)
        {
            if (this.custom_data_dictionary.TryGetValue(key.ToUpper(), out string value))
            {
                return value;
            }
            return null;
        }

        public int? GetInt(string key)
        {
            if (int.TryParse(this.GetString(key.ToUpper()), out int value))
            {
                return value;
            }
            return null;
        }

        public ulong? GetUlong(string key)
        {
            if (ulong.TryParse(this.GetString(key.ToUpper()), out ulong value))
            {
                return value;
            }
            return null;
        }

        public bool? GetBoolean(string key)
        {
            try
            {
                return Convert.ToBoolean(this.GetInt(key.ToUpper()));
            }
            catch (Exception)
            {

            }
            return null;
        }

        // Static Functions
        public static ItemAdditionalData Parse(string custom_data)
        {
            ItemAdditionalData parser = new ItemAdditionalData(custom_data);
            return parser;
        }

        public static bool Equals(string customDataOne, string customDataTwo)
        {
            ItemAdditionalData additionalDataOne = ItemAdditionalData.Parse(customDataOne);
            Dictionary<string, string> customDataDictOne = additionalDataOne.GetAllCustomData();

            ItemAdditionalData additionalDataTwo = ItemAdditionalData.Parse(customDataTwo);
            Dictionary<string, string> customDataDictTwo = additionalDataOne.GetAllCustomData();

            List<string> keysFound = new List<string>();
            keysFound.AddRange(customDataDictOne.Keys);
            keysFound.AddRange(customDataDictTwo.Keys);

            foreach (string key in keysFound)
            {
                // If the key is in the skip list, it does not matter for the equals check, skip.
                if (_PersonalizedAdditionalData.Contains(key))
                {
                    continue;
                }

                if (additionalDataOne.GetString(key) != additionalDataTwo.GetString(key))
                {
                    return false;
                }
            }

            return true;
        }

        public static ItemAdditionalData RemovePersonalizedData(ItemAdditionalData original)
        {
            // Get the full dictionary from the original additional data.
            Dictionary<string, string> allData = original.GetAllCustomData();

            // Filter out any keys that are in the personalized set.
            var filteredData = allData
                .Where(kvp => !_PersonalizedAdditionalData.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Rebuild the custom data string in the same format.
            // (Assumes the format: "[KEY=VALUE];[KEY=VALUE];..." sorted by key.)
            var sortedData = filteredData.OrderBy(kvp => kvp.Key);
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var kvp in sortedData)
            {
                if (!first)
                {
                    sb.Append(";");
                }
                sb.Append($"[{kvp.Key.ToUpper()}={kvp.Value}]");
                first = false;
            }

            string newCustomData = sb.ToString();

            return ItemAdditionalData.Parse(newCustomData);
        }

        public static ItemAdditionalData Combine(string primaryDataString, string secondaryDataString)
        {
            // Parse both additional data strings.
            ItemAdditionalData primaryData = ItemAdditionalData.Parse(primaryDataString);
            ItemAdditionalData secondaryData = ItemAdditionalData.Parse(secondaryDataString);

            // Retrieve the underlying dictionaries.
            Dictionary<string, string> primaryDict = primaryData.GetAllCustomData();
            Dictionary<string, string> secondaryDict = secondaryData.GetAllCustomData();

            // Start with the primary dictionary.
            Dictionary<string, string> combined = new Dictionary<string, string>(primaryDict);

            // Add keys from the secondary dictionary only if they don't exist in primary.
            foreach (var kvp in secondaryDict)
            {
                if (!combined.ContainsKey(kvp.Key))
                {
                    combined[kvp.Key] = kvp.Value;
                }
            }

            // Rebuild the custom data string in the expected format: "[KEY=VALUE];[KEY=VALUE];..."
            // Sort keys for consistency and convert keys to uppercase.
            var sortedData = combined.OrderBy(kvp => kvp.Key);
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var kvp in sortedData)
            {
                if (!first)
                {
                    sb.Append(";");
                }
                sb.Append($"[{kvp.Key.ToUpper()}={kvp.Value}]");
                first = false;
            }

            string newCustomData = sb.ToString();
            return ItemAdditionalData.Parse(newCustomData);
        }

        public override string ToString()
        {
            bool firstValue = true;
            StringBuilder newCustomDataString = new StringBuilder();

            this.custom_data_dictionary = this.custom_data_dictionary.OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Value);

            foreach (KeyValuePair<string, string> key_value in this.custom_data_dictionary)
            {
                if (!firstValue)
                {
                    newCustomDataString.Append(";");
                }
                newCustomDataString.Append(string.Format("[{0}={1}]", key_value.Key.ToUpper(), key_value.Value));
                firstValue = false;
            }

            this.custom_data = newCustomDataString.ToString();

            return this.custom_data;
        }
    }
}
