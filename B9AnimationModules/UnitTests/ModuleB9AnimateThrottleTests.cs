#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Testing;

namespace B9AnimationModules.UnitTests
{
    public class ModuleB9AnimateThrottleTests : B9AnimateTestBase<ModuleB9AnimateThrottle>
    {
        private ModuleEngines engineModule;

        public override void TestStartUp()
        {
            base.TestStartUp();

            engineModule = AddPartModule<ModuleEngines>();
        }

        [TestInfo("TestFindEngine")]
        public void TestFindEngine()
        {
            part.Modules.Remove(engineModule);

            assertEquals("Returns null if engine module is absent", (module.FindEngine() == null), true);

            module.engineID = "";
            part.Modules.Add(engineModule);
            engineModule.engineID = "Engine";

            assertEquals("It finds the engine with no engineID", module.FindEngine(), engineModule);

            module.engineID = "NotEngine";
            assertEquals("It does not find the engine when engineID mismatches", (module.FindEngine() == null), true);

            ModuleEngines engineModule2 = AddPartModule<ModuleEngines>();
            ModuleEngines engineModule3 = AddPartModule<ModuleEngines>();

            module.engineID = "CorrectEngine";
            engineModule.engineID = "FakeEngine";
            engineModule2.engineID = "CorrectEngine";
            engineModule3.engineID = "";

            assertEquals("It finds the correct engine module among many", module.FindEngine(), engineModule2);
        }

        [TestInfo("TestGetThrottleSetting")]
        public void TestGetThrottleSetting()
        {
            part.Modules.Remove(engineModule);
            module.OnStart(PartModule.StartState.None);

            assertEquals("It gives zero when the engine cannot be found", module.GetThrottleSetting(), 0f);

            part.Modules.Add(engineModule);
            module.engineID = "OddlySpecific";
            engineModule.engineID = "OddlySpecific";
            module.OnStart(PartModule.StartState.None);
            
            float throttle = 0.12345f;
            engineModule.currentThrottle = throttle;

            assertEquals("It gives the correct throttle normally", module.GetThrottleSetting(), throttle);
        }

        [TestInfo("TestTargetAnimationState")]
        public void TestTargetAnimationState()
        {
            module.OnStart(PartModule.StartState.None);

            float throttle = 0.12345f;
            engineModule.currentThrottle = throttle;

            assertEquals("It gives the correct throttle", module.GetTargetAnimationState(), throttle);
        }
    }
}

#endif