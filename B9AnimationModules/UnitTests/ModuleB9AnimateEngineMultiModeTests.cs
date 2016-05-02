#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Testing;

namespace B9AnimationModules.UnitTests
{
    public class ModuleB9AnimateEngineMultiModeTests : B9AnimateTestBase<ModuleB9AnimateEngineMultiMode>
    {
        private ModuleEnginesFX primaryEngine;
        private ModuleEnginesFX secondaryEngine;
        private MultiModeEngine multiModeEngine;

        public override void TestStartUp()
        {
            base.TestStartUp();
            
            part.isControlSource = true;

            primaryEngine = AddPartModule<ModuleEnginesFX>();
            secondaryEngine = AddPartModule<ModuleEnginesFX>();
            multiModeEngine = AddPartModule<MultiModeEngine>();

            primaryEngine.engineID = "primary";
            secondaryEngine.engineID = "secondary";
            multiModeEngine.primaryEngineID = primaryEngine.engineID;
            multiModeEngine.secondaryEngineID = secondaryEngine.engineID;
            multiModeEngine.OnStart(PartModule.StartState.None);
        }

        [TestInfo("TestFindMultiModeModule")]
        public void TestFindMultiModeModule()
        {
            assertEquals("Finds the MultiModeEngine", module.FindMultiModeModule(), multiModeEngine);
        }

        [TestInfo("TestGetThrottleSetting")]
        public void TestGetThrottleSetting()
        {
            part.Modules.Remove(multiModeEngine);
            module.OnStart(PartModule.StartState.None);

            assertEquals("It gives zero when the multi mode engine cannot be found", module.GetThrottleSetting(), 0f);

            part.Modules.Add(multiModeEngine);
            module.OnStart(PartModule.StartState.None);

            multiModeEngine.runningPrimary = true;
            float throttle = 0.12345f;
            primaryEngine.currentThrottle = throttle;

            assertEquals("It gives the correct throttle normally", module.GetThrottleSetting(), throttle);
        }

        [TestInfo("TestGetMachNumber")]
        public void TestGetMachNumber()
        {
            float mach = 1.2345f;
            part.machNumber = mach;
            assertEquals("Mach number matches part", module.GetMachNumber(), mach);
        }

        [TestInfo("TestTargetAnimationState")]
        public void TestTargetAnimationState()
        {
            float shutdownState = 0.12345f;
            module.shutdownState = shutdownState;
            primaryEngine.EngineIgnited = false;
            secondaryEngine.EngineIgnited = false;

            assertEquals("It gives shutdown state when no engine is ignited", module.GetTargetAnimationState(), shutdownState);

            part.Modules.Remove(multiModeEngine);
            module.OnStart(PartModule.StartState.None);
            primaryEngine.EngineIgnited = true;

            assertEquals("It gives the shutdown state when multiModeEngine is absent", module.GetTargetAnimationState(), shutdownState);

            part.Modules.Add(multiModeEngine);
            module.OnStart(PartModule.StartState.None);

            module.throttleCurvePrimary = new FloatCurve();
            module.throttleCurvePrimary.Add(0.0f, 0.2f, 0.1f, 0.1f);
            module.throttleCurvePrimary.Add(1.0f, 0.3f, 0.1f, 0.1f);
            
            multiModeEngine.runningPrimary = true;
            primaryEngine.EngineIgnited = true;
            primaryEngine.thrustPercentage = 100f;

            module.machCurvePrimary = new FloatCurve();
            module.machCurvePrimary.Add(0.0f, 1.0f); // Make sure it's not doing anything with mach curve here
            module.idleState = 0.2468f;
            module.idleThreshold = 0.01f;
            primaryEngine.currentThrottle = 0.005f;
            assertEquals("It gives the idle state below idle throttle threshold", module.GetTargetAnimationState(), 0.2468f);

            module.machCurvePrimary = new FloatCurve();
            module.machCurvePrimary.Add(0.0f, 0.0f);
            primaryEngine.currentThrottle = 0.5f;
            assertEquals("It reads the throttle correctly at 50% thrust", module.GetTargetAnimationState(), 0.25f);

            primaryEngine.currentThrottle = 1.0f;
            assertEquals("It reads the throttle correctly at 100% thrust", module.GetTargetAnimationState(), 0.3f);

            module.throttleCurvePrimary = new FloatCurve();
            module.throttleCurvePrimary.Add(0.0f, 0.0f);
            module.machCurvePrimary = new FloatCurve();
            module.machCurvePrimary.Add(0.0f, 0.2f, 0.2f, 0.2f);
            module.machCurvePrimary.Add(2.0f, 0.4f, 0.2f, 0.2f);

            primaryEngine.currentThrottle = 1.0f;
            part.machNumber = 0.0f;
            assertEquals("It reads the mach number curve correctly at mach 0", module.GetTargetAnimationState(), 0.2f);

            part.machNumber = 1.0f;
            assertEquals("It reads the mach number curve correctly at mach 1", module.GetTargetAnimationState(), 0.3f);
            
            module.throttleCurvePrimary = new FloatCurve();
            module.throttleCurvePrimary.Add(0.0f, 2.0f);
            module.machCurvePrimary = new FloatCurve();
            module.machCurvePrimary.Add(0.0f, 3.0f);
            assertEquals("State is clamped above 1", module.GetTargetAnimationState(), 1.0f);

            module.throttleCurvePrimary = new FloatCurve();
            module.throttleCurvePrimary.Add(0.0f, -2.0f);
            module.machCurvePrimary = new FloatCurve();
            module.machCurvePrimary.Add(0.0f, -3.0f);
            assertEquals("State is clamped below 0", module.GetTargetAnimationState(), 0.0f);

            multiModeEngine.runningPrimary = false;
            primaryEngine.EngineIgnited = false;
            secondaryEngine.EngineIgnited = true;
            secondaryEngine.thrustPercentage = 100f;

            module.throttleCurveSecondary = new FloatCurve();
            module.throttleCurveSecondary.Add(0.0f, 0.0f, 0.4f, 0.4f);
            module.throttleCurveSecondary.Add(1.0f, 0.4f, 0.4f, 0.4f);
            module.machCurveSecondary = new FloatCurve();
            module.machCurveSecondary.Add(0.0f, 0.3f, 0.2f, 0.2f);
            module.machCurveSecondary.Add(1.0f, 0.5f, 0.2f, 0.2f);
            secondaryEngine.currentThrottle = 0.25f;
            part.machNumber = 0.5f;
            assertEquals("Secondary mode works properly", module.GetTargetAnimationState(), 0.5f);
        }
    }
}

#endif