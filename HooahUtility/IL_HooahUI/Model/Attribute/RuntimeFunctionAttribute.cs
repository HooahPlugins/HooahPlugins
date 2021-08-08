namespace HooahUtility.Model.Attribute
{
    // todo: create easy multi option function attribute for less hassle. 
    public class RuntimeFunctionAttribute : System.Attribute
    {
        public string name;

        public RuntimeFunctionAttribute(string name)
        {
            this.name = name;
        }
    }
}
