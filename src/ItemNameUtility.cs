using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AwakeningItemTool
{
    public class ItemNameUtility
    {
        private static Dictionary<string, string> iidToNameDictionary;

        public static void Initialize()
        {
            iidToNameDictionary = new Dictionary<string, string>();
            ReadIidToNameFileContents();
        }

        public static string GetNameForItemByIid(string iid)
        {
            string result;
            if (iidToNameDictionary.TryGetValue(iid, out result))
            {
                return result;
            }

            return iid;
        }

        private static void ReadIidToNameFileContents()
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AwakeningItemTool.Resources.IIDtoName.txt"));
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] lineParts = line.Split(',');
                iidToNameDictionary.Add(lineParts[0], lineParts[1]);
            }
        }
    }
}
