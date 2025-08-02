using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using FG.Common;
using FG.Common.CMS;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Mediatonic.Tools.ParsingUtils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using ThatOneRandom3AMProject.HarmonyPathces;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using Debug = UnityEngine.Debug;

namespace ThatOneRandom3AMProject
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
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

            public override readonly string ToString() => $"v{Version} (#{GetCommit()})";
            readonly string GetCommit() => Commit.Length > 6 ? Commit[..6] : Commit;
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

            Harmony = new("flz.test");
            Harmony.PatchAll(typeof(HarmonyPatches));

            ClassInjector.RegisterTypeInIl2Cpp<Utility>();
        }
    }
}
