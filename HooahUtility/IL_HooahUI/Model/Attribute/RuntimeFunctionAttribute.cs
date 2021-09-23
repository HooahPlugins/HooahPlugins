namespace HooahUtility.Model.Attribute
{
    public class RuntimeFunctionAttribute : System.Attribute
    {
        public string name;

        public RuntimeFunctionAttribute(string name)
        {
            this.name = name;
        }
    }
}
