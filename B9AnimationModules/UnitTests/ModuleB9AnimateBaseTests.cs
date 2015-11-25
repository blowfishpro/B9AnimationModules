#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Testing;

namespace B9AnimationModules.UnitTests
{
    public class ModuleB9AnimateBaseTestable : ModuleB9AnimateBase
    {
        public float targetAnimationState = 0f;
        public float deltaTime = 0.1f;

        public override float TargetAnimationState()
        {
            return targetAnimationState;
        }

        public override float GetDeltaTime()
        {
            return deltaTime;
        }
    }

    public class ModuleB9AnimateBaseTests : B9AnimateTestBase<ModuleB9AnimateBaseTestable>
    {
        [TestInfo("TestHandleResponseSpeed")]
        public void TestHandleResponseSpeed()
        {
            module.responseSpeed = 0.01f;
            module.deltaTime = 0.5f;
            module.animationState = 0f;
            assertEquals("Correctly handles response speed", module.HandleResponseSpeed(1f), 0.125f);

            module.responseSpeed = 0.01f;
            module.deltaTime = 10f;
            module.animationState = 0f;
            assertEquals("Clamps animation state to 1", module.HandleResponseSpeed(1f), 1f);

            module.responseSpeed = 0.01f;
            module.deltaTime = 10f;
            module.animationState = 1f;
            assertEquals("Clamps animation state to 0", module.HandleResponseSpeed(0f), 0f);
        }
    }
}

#endif