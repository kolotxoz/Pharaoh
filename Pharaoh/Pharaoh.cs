using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Pharaoh;

[BepInPlugin("p1xel8ted.Pharaoh.Pharaoh", "Pharaoh", "0.1.0")]
[HarmonyPatch]
public class Pharaoh : BaseUnityPlugin
{
    private static Harmony _hi;
    private static ManualLogSource _log;

    public void Awake()
    {
        _hi = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        _log = new ManualLogSource("Log");
        BepInEx.Logging.Logger.Sources.Add(_log);
        _log.LogInfo($"Plugin Pharaoh is loaded!");
    }

    public void OnDestroy()
    {
        _hi?.UnpatchSelf();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionManager), nameof(OptionManager.GetAvailableResolutions))]
    public static void OptionManager_GetAvailableResolutions(ref OptionManager __instance, ref IEnumerable<Resolution> __result)
    {
        var res = new Resolution
        {
            height = Display.main.systemHeight,
            width = Display.main.systemWidth,
            refreshRate = Screen.resolutions.Max(a => a.refreshRate)
        };
        __result = __result.Prepend(res);
    }

    private void LateUpdate()
    {
        var graphics = GlobalAccessor.PlayerOptions.Graphics;
        var aspectRatioFitters = FindObjectsOfType<AspectRatioFitter>();
        foreach (var a in aspectRatioFitters)
        {
            a.aspectRatio = (float) graphics.Width / (float) graphics.Height;
        }
    }
}