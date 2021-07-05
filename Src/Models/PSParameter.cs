namespace Hangfire.PowerShellExecutor.Models
{
    public class PSParameter
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public bool Protected { get; private set; }

        public PSParameter(string name, string value, bool isProtected)
        {
            Name = name;
            Value = value;
            Protected = isProtected;
        }

        public PSParameter(string name, string value)
        {
            Name = name;
            Value = value;
            Protected = false;
        }
    }
}