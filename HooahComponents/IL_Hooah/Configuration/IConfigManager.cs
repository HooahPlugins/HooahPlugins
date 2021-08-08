namespace HooahComponents.Configuration
{
    public interface IConfigManager
    {
        byte[] SerializeConfig();
        void DeserializeConfig(byte[] msgPackBytes);
    }
}