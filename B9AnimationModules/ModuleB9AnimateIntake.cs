using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace B9AnimationModules
{
    public class ModuleB9AnimateIntake : ModuleB9AnimateBase
    {
        [KSPField]
        public float intakeClosedState = 0f;

        [KSPField]
        public FloatCurve machCurve = new FloatCurve();

        private ModuleResourceIntake intake;

        public ModuleResourceIntake FindIntake()
        {
            return part.FindModuleImplementing<ModuleResourceIntake>();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            intake = FindIntake();

            if (machCurve == null)
            {
                Debug.LogError("ERROR: ModuleB9AnimateIntake on part " + part.name + ": machCurve is null!");
                machCurve = new FloatCurve();
                machCurve.Add(0f, 0f);
            }
        }

        public float GetMachNumber()
        {
            return (float)part.machNumber;
        }

        public override float GetTargetAnimationState()
        {
            if (intake && !intake.intakeEnabled)
                return intakeClosedState;
            
            float state = machCurve.Evaluate(GetMachNumber());
            state = Mathf.Clamp(state, 0f, 1f);
            return state;
        }
    }
}
