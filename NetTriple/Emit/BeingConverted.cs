using System.Collections.Generic;

namespace NetTriple.Emit
{
    /// <summary>
    /// In order to guard against perpetual recursion in case of
    /// circular references
    /// </summary>
    public static class BeingConverted
    {
        private static readonly List<string> ConvertingList = new List<string>();

        public static bool IsConverting(string subject)
        {
            return ConvertingList.Contains(subject);
        }

        public static void Converting(string subject)
        {
            ConvertingList.Add(subject);
        }

        public static void Finished(string subject)
        {
            ConvertingList.Remove(subject);
        }
    }
}
