namespace HooahUtility.Model.Attribute
{
    public class FieldNameAttribute : System.Attribute
    {
        public string name;

        public FieldNameAttribute(string name)
        {
            this.name = name;
        }
    }
}
