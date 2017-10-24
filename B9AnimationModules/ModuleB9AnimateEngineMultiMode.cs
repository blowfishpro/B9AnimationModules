using System;
using UnityEngine;

namespace B9AnimationModules
{
    public class ModuleB9AnimateEngineMultiMode : ModuleB9AnimateBase
    {
        private MultiModeEngine multiMode;

        [KSPField]
        public FloatCurve throttleCurvePrimary = new FloatCurve();

        [KSPField]
        public FloatCurve throttleCurveSecondary = new FloatCurve();

        [KSPField]
        public FloatCurve machCurvePrimary = new FloatCurve();
        
        [KSPField]
        public FloatCurve machCurveSecondary = new FloatCurve();

        [KSPField]
        public float shutdownState = 0f;

        [KSPField]
        public float idleState = 1f;

        [KSPField]
        public float idleThreshold = 0.01f;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            multiMode = FindMultiModeModule();
            if (multiMode == null)
                LogError("Cannot find MultiModeEngine");
        }

        public override float GetTargetAnimationState()
        {
            float state;

            if (multiMode == null || !multiMode.isOperational)
            {
                state = shutdownState;
            }
            else if (GetThrottleSetting() <= idleThreshold)
            {
                state = idleState;
            }
            else if (multiMode.runningPrimary)
            {
                state = throttleCurvePrimary.Evaluate(GetThrottleSetting());
                state += machCurvePrimary.Evaluate(GetMachNumber());
            }
            else
            {
                state = throttleCurveSecondary.Evaluate(GetThrottleSetting());
                state += machCurveSecondary.Evaluate(GetMachNumber());
            }

            state = Mathf.Clamp(state, 0f, 1f);

            return state;
        }

        public MultiModeEngine FindMultiModeModule()
        {
            return part.FindModuleImplementing<MultiModeEngine>();
        }

        public float GetThrottleSetting()
        {
            return multiMode?.throttleSetting ?? 0f;
        }

        public float GetMachNumber()
        {
            return (float)part.machNumber;
        }
    }
}
