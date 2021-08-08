namespace HooahUtility.Model.Attribute
{
    public class NumberSpinnerAttribute : System.Attribute
    {
        public int min;
        public int max;
        public bool unlimited;

        public NumberSpinnerAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
            unlimited = false;
        }

        public NumberSpinnerAttribute()
        {
            unlimited = true;
        }
    }
}