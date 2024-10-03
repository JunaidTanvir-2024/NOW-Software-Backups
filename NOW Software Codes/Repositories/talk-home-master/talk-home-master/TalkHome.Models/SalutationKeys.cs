using System.Collections.Generic;

namespace TalkHome.Models
{
    /// <summary>
    /// Provides a list of keys for salutations.
    /// </summary>
    public class SalutationKeys
    {
        public static List<string> Keys { get; set; }

        static SalutationKeys()
        {
            Keys = new List<string>();
            Keys.Add("Mr");
            Keys.Add("Miss");
            Keys.Add("Mrs");
            Keys.Add("Ms");
            Keys.Add("Dr");
        }
    }
}
