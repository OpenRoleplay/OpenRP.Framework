using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Chat.Extensions
{
    public static partial class StringExtensions
    {
        public static string Muffle(this string text, float percentage)
        {
            char[] char_array = text.ToCharArray();

            Random random_percentage = new Random();

            for (int i = 0; i < char_array.Length; i++)
            {
                if (char_array[i] != ' ')
                {
                    if (random_percentage.Next(0, 100) <= percentage)
                    {
                        char_array[i] = '.';
                    }
                }
            }

            return new string(char_array);
        }

        public static string Muffle(this string text, float distance, float acceptable_distance = 5.0f, float max_distance = 7.0f)
        {
            if (distance >= acceptable_distance)
            {
                float percentage = (30 / (max_distance - acceptable_distance) * (distance - acceptable_distance));

                return text.Muffle(percentage);
            }
            return text;
        }
    }
}
