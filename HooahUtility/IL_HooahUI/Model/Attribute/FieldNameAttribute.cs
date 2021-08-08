namespace HooahUtility.Model.Attribute
{
    // todo: implement globalization config..
    public class FieldNameAttribute : System.Attribute
    {
        public string name;

        public FieldNameAttribute(string name)
        {
            this.name = name;
        }
    }
}
