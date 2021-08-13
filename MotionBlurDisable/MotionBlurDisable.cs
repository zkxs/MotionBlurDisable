using HarmonyLib;
using MelonLoader;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MotionBlurDisable
{
    public class MotionBlurDisable : MelonMod
    {
        private static bool _first_trigger = false;

        public override void OnApplicationStart()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("net.michaelripley.MotionBlurDisable");
            MethodInfo originalMethod = AccessTools.DeclaredMethod(typeof(CameraInitializer), nameof(CameraInitializer.SetPostProcessing), new Type[] { typeof(Camera), typeof(bool), typeof(bool), typeof(bool) });
            if (originalMethod == null)
            {
                MelonLogger.Error("Could not find CameraInitializer.SetPostProcessing(Camera, bool, bool, bool) setter");
                return;
            }
            MethodInfo replacementMethod = AccessTools.DeclaredMethod(typeof(MotionBlurDisable), nameof(SetPostProcessing));
            harmony.Patch(originalMethod, prefix: new HarmonyMethod(replacementMethod));
            MelonLogger.Msg("Hook installed successfully");
        }

        private static bool SetPostProcessing(Camera c, bool enabled, bool motionBlur, bool screenspaceReflections)
        {
            PostProcessLayer postProcessLayer = c.GetComponent<PostProcessLayer>();
            AmplifyOcclusionEffect amplifyOcclusionEffect = c.GetComponent<AmplifyOcclusionEffect>();
            postProcessLayer.enabled = enabled;
            if (amplifyOcclusionEffect != null)
            {
                amplifyOcclusionEffect.enabled = enabled;
            }
            if (enabled)
            {
                postProcessLayer.defaultProfile.GetSetting<MotionBlur>().enabled.value = false;
                postProcessLayer.defaultProfile.GetSetting<ScreenSpaceReflections>().enabled.value = screenspaceReflections;
            }

            if (!_first_trigger)
            {
                _first_trigger = true;
                MelonLogger.Msg("Hook triggered! Everything worked!");
            }

            return false; // skip original method
        }
    }
}
