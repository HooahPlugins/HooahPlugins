namespace HooahUtility.Model.Attribute
{
    public class PropertyRangeAttribute : System.Attribute
    {
        public float min;
        public float max;

        public PropertyRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}