using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FGLegacyTools.HarmonyPathces;

namespace FGLegacyTools
{
    [BepInPlugin(GUID, DisplayName, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal readonly struct BuildInfo
        {
            public readonly string Version;
            public readonly string Config;
            public readonly string Commit;
            public readonly DateTime BuildDate;

            internal BuildInfo(string version, string commit, string build_date, string conf)
            {
                Version = version;
                Commit = commit;
                Config = conf;

                if (long.TryParse(build_date, out long unixTimestamp))
                    BuildDate = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
            }

            public override readonly string ToString() => $"v{Version}";
            internal readonly string GetCommit(int l) => Commit.Length > l ? Commit[..l] : Commit;
        }

        internal static new ManualLogSource Log;
        internal static Harmony Harmony;
        internal static BuildInfo BuildDetails;

        public override void Load()
        {
            Log = base.Log;

            var assembly = Assembly.GetExecutingAssembly();

            var version = FileVersionInfo.GetVersionInfo(assembly.Location);
            var commit = version.ProductVersion.Split('+');
            var metadata = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToList();
            var cfg = assembly.GetCustomAttributes<AssemblyConfigurationAttribute>().ToList()[0].Configuration;

            var buildDate = metadata.FirstOrDefault(x => x.Key == "BuildDate")?.Value;

            BuildDetails = new BuildInfo(MyPluginInfo.PLUGIN_VERSION, commit.Length > 1 ? commit[1] : "...", buildDate, cfg);

            Harmony = new($"{GUID}.test");
            Harmony.PatchAll(typeof(HarmonyPatches));

            ClassInjector.RegisterTypeInIl2Cpp<Utility>();

            Log.LogMessage($"---");
            Log.LogMessage($"{DisplayName} - {BuildDetails} (#{BuildDetails.GetCommit(12)})");
            Log.LogMessage($"Compiled at: {BuildDetails.BuildDate}");
            Log.LogMessage($"---");
        }
    }
}
