using System;
using System.Collections.Generic;
using UnityEngine;

namespace B9AnimationModules
{
    public abstract class ModuleB9AnimateBase : PartModule
    {
        #region KSPFields
        [KSPField]
        public string animationName = "animation";

        [KSPField]
        public float responseSpeed = 1f;

        [KSPField]
        public int layer = 1;

        [KSPField(isPersistant = true)]
        public float animationState = 0f;

        #endregion

        #region Protected Fields

        protected AnimationState[] animStates;

        #endregion

        #region Methods

        public virtual Animation[] FindAnimations(string name)
        {
            Animation[] anims = part.FindModelAnimators(name);
            if (anims.Length == 0)
                Debug.LogError("Error: Cannot find animation named '" + name + "' on part " + part.name);

            return anims;
        }

        public virtual AnimationState[] SetupAnimations(Animation[] anims)
        {
            AnimationState[] states = new AnimationState[anims.Length];
            for (int i = 0; i < anims.Length; i++)
            {
                Animation anim = anims[i];
                AnimationState animState = anim[anim.name];
                animState.speed = 0;
                animState.enabled = true;
                animState.layer = layer;
                anim.Play(anim.name);
                states[i] = animState;
            }

            return states;
        }

        public virtual float TargetAnimationState()
        {
            return 0f;
        }

        public virtual float HandleResponseSpeed(float target)
        {
            return Mathf.Lerp(animationState, target, responseSpeed * 25f * GetDeltaTime());
        }

        public virtual float GetDeltaTime()
        {
            return TimeWarp.fixedDeltaTime;
        }

        #endregion

        #region Setup

        public override void OnStart(PartModule.StartState state)
        {
            Animation[] anims = FindAnimations(animationName);
            animStates = SetupAnimations(anims);
        }

        #endregion

        #region Update

        protected virtual void Update()
        {
            float target = TargetAnimationState();
            animationState = HandleResponseSpeed(target);
            SetAnimationState(animationState);
        }

        protected virtual void SetAnimationState(float state)
        {
            for (int i = 0; i < animStates.Length; i++)
            {
                animStates[i].normalizedTime = animationState;
            }
        }

        #endregion

        #region Debug

        public void LogError(string message)
        {
            string str = "Module " + name;
            if (part?.partInfo != null)
                str += " on part " + part.partInfo.name;
            str += ": ";
            str += message;
            Debug.LogError(message);
        }

        #endregion
    }
}
