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

        #region Setup

        public override void OnStart(PartModule.StartState state)
        {
            Animation[] anims = FindAnimations(animationName);
            animStates = SetupAnimations(anims, animationName);
        }

        #endregion

        #region Update

        protected virtual void Update()
        {
            float target = GetTargetAnimationState();
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

        #region Overrideable Methods

        public virtual Animation[] FindAnimations(string name)
        {
            Animation[] anims = part.FindModelAnimators(name);
            if (anims.Length == 0)
                Debug.LogError("Error: Cannot find animation named '" + name + "' on part " + part.name);

            return anims;
        }

        public virtual AnimationState[] SetupAnimations(Animation[] anims, string name)
        {
            List<AnimationState> states = new List<AnimationState>();
            for (int i = 0; i < anims.Length; i++)
            {
                Animation anim = anims[i];
                AnimationState animState = anim?[name];
                if (animState == null)
                    continue;
                animState.speed = 0;
                animState.enabled = true;
                animState.layer = layer;
                anim.Play(name);
                states.Add(animState);
            }

            return states.ToArray();
        }

        public virtual float GetTargetAnimationState() => 0f;

        public virtual float HandleResponseSpeed(float target) => Mathf.Lerp(animationState, target, responseSpeed * 25f * DeltaTime);
        
        public virtual float DeltaTime => TimeWarp.fixedDeltaTime;

        #endregion

        #region Debug

        public void LogError(string message)
        {
            string str = "Error on module " + moduleName;
            if (part?.partInfo != null)
                str += " on part " + part.partInfo.name;
            str += ": ";
            str += message;
            Debug.LogError(str);
        }

        #endregion
    }
}
