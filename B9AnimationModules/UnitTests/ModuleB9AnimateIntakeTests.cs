#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Testing;

namespace B9AnimationModules.UnitTests
{
    public class ModuleB9AnimateIntakeTests : B9AnimateTestBase<ModuleB9AnimateIntake>
    {
        private ModuleResourceIntake intake;

        public override void TestStartUp()
        {
            base.TestStartUp();
            
            intake = AddPartModule<ModuleResourceIntake>();
        }

        [TestInfo("TestFindIntakeModule")]
        public void TestFindIntakeModule()
        {
            assertEquals("Finds the correct intake module", module.FindIntake(), intake);
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
            float state = 0.12345f;
            module.intakeClosedState = state;
            module.machCurve = new FloatCurve();
            module.machCurve.Add(0f, 0f);
            module.OnStart(PartModule.StartState.None);
            intake.intakeEnabled = false;
            assertEquals("State is correct when intake is disabled", module.GetTargetAnimationState(), state);

            module.machCurve = new FloatCurve();
            module.machCurve.Add(1f, 0f, 1f, 1f);
            module.machCurve.Add(2f, 1f, 1f, 1f);
            intake.intakeEnabled = true;
            part.machNumber = 1.5f;
            assertEquals("State is correct inside float curve range", module.GetTargetAnimationState(), 0.5f);

            module.machCurve = new FloatCurve();
            module.machCurve.Add(0f, 2f);
            part.machNumber = 1.5f;
            assertEquals("State is clamped to maximum of 1", module.GetTargetAnimationState(), 1f);

            module.machCurve = new FloatCurve();
            module.machCurve.Add(0f, -1f);
            part.machNumber = 1.5f;
            assertEquals("State is clamped to minimum of 0", module.GetTargetAnimationState(), 0f);
        }
    }
}

#endif
