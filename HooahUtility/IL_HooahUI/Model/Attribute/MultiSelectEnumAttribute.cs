using System;

namespace HooahUtility.Model.Attribute
{
    // create multi select options.
    public class MultiSelectEnumAttribute : System.Attribute
    {
        public int max = 1;
        public MultiSelectEnumAttribute(int max = 1)
        {
            max = Math.Max(1, max);
        }
    }
}
