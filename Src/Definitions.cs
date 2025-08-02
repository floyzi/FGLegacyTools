global using static Definitions;
using System.Collections.Generic;
using UnityEngine;
static class Definitions
{
    //if build is missing from this list a warning will be shown on gui
    internal static HashSet<string> KnownBuilds = 
    [
        "-buildVersion",
        "0.4.541",
        "0.4.640"
    ];

    internal const string GUID = "flz.random.project";
    internal const string DisplayName = "NAME WANTED!!";
}