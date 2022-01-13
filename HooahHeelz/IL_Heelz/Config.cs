using BepInEx.Configuration;
using Heelz;

namespace Util
{
    public static class ConfigUtility
    {
        public const string HeelzConfigName = "Heelz";
        public static bool CanLog => HeelzPlugin.VerboseMode.Value;

        public static void Initialize(ConfigFile Config)
        {
            HeelzPlugin.LoadDevXML = Config.Bind(HeelzConfigName, "Load Developer XML", false,
                new ConfigDescription(
                    "Make Heelz Plugin load heel_manifest.xml file from game root folder. Useful for developing heels. Useless for most of users."));
            HeelzPlugin.VerboseMode = Config.Bind(HeelzConfigName, "Verbose Mode", false,
                new ConfigDescription("Print Everything"));
        }
    }
}
