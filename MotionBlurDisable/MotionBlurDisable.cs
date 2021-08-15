using HarmonyLib;
using NeosModLoader;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MotionBlurDisable
{
    public class MotionBlurDisable : NeosMod
    {
        public override string Name => "MotionBlurDisable";
        public override string Author => "runtime";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/zkxs/MotionBlurDisable";

        private static bool _first_trigger = false;

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.michaelripley.MotionBlurDisable");
            MethodInfo originalMethod = AccessTools.DeclaredMethod(typeof(CameraInitializer), nameof(CameraInitializer.SetPostProcessing), new Type[] { typeof(Camera), typeof(bool), typeof(bool), typeof(bool) });
            if (originalMethod == null)
            {
                Error("Could not find CameraInitializer.SetPostProcessing(Camera, bool, bool, bool)");
                return;
            }
            MethodInfo replacementMethod = AccessTools.DeclaredMethod(typeof(MotionBlurDisable), nameof(SetPostProcessing));
            harmony.Patch(originalMethod, prefix: new HarmonyMethod(replacementMethod));
            Msg("Hook installed successfully");

            // disable prexisting motion blurs
            PostProcessLayer[] components = Resources.FindObjectsOfTypeAll<PostProcessLayer>();
            int count = 0;
            foreach (PostProcessLayer component in components)
            {
                try
                {
                    MotionBlur motionBlur = component.defaultProfile.GetSetting<MotionBlur>();
                    if (motionBlur != null)
                    {
                        motionBlur.enabled.value = false;
                        count += 1;
                    }
                }
                catch(Exception e)
                {
                    Warn($"failed to disable a motion blur: {e}");
                }
            }
            Msg($"disabled {count} prexisting motion blurs");
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
                Msg("Hook triggered! Everything worked!");
            }

            return false; // skip original method
        }
    }
}
