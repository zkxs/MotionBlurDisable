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
        public override string Version => "1.1.0";
        public override string Link => "https://github.com/zkxs/MotionBlurDisable";

        private static bool _first_trigger = false;

        public override void OnEngineInit()
        {
            // disable future motion blurs by patching the setup method
            Harmony harmony = new Harmony("net.michaelripley.MotionBlurDisable");
            MethodInfo originalMethod = AccessTools.DeclaredMethod(typeof(CameraInitializer), nameof(CameraInitializer.SetPostProcessing), new Type[] { typeof(Camera), typeof(bool), typeof(bool), typeof(bool) });
            if (originalMethod == null)
            {
                Error("Could not find CameraInitializer.SetPostProcessing(Camera, bool, bool, bool)");
                return;
            }
            MethodInfo replacementMethod = AccessTools.DeclaredMethod(typeof(MotionBlurDisable), nameof(SetPostProcessingPostfix));
            harmony.Patch(originalMethod, postfix: new HarmonyMethod(replacementMethod));
            Msg("Hook installed successfully");

            // disable prexisting motion blurs by searching for all matching Unity components
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
                    Warn($"failed to disable a prexisting motion blur: {e}");
                }
            }
            Msg($"disabled {count} prexisting motion blurs");
        }

        private static void SetPostProcessingPostfix(Camera c, bool enabled, bool motionBlur, bool screenspaceReflections)
        {
            try
            {
                c.GetComponent<PostProcessLayer>().defaultProfile.GetSetting<MotionBlur>().enabled.value = false;

                if (!_first_trigger)
                {
                    _first_trigger = true;
                    Msg("Hook triggered! Everything worked!");
                }
            }
            catch (Exception e)
            {
                Warn($"failed to disable a new motion blur: {e}");
            }
        }
    }
}
