using System;
using System.Linq;

namespace B9AnimationModules
{
    public class ModuleB9AnimateThrottle : ModuleB9AnimateBase
    {
        private IEngineStatus engine;

        [KSPField]
        public string engineID;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            engine = FindEngine();
        }

        public override float GetTargetAnimationState() => GetThrottleSetting();

        public IEngineStatus FindEngine()
        {
            if (String.IsNullOrEmpty(engineID))
            {
                IEngineStatus engine = part.FindModuleImplementing<IEngineStatus>();

                if (engine == null)
                    LogError("Cannot find engine module");

                return engine;
            }
            else
            {
                var engine = part.Modules.OfType<IEngineStatus>().FirstOrDefault(module => module.engineName == engineID);
                
                if (engine == null)
                    LogError("Cannot find engine module with engineID '" + engineID + "'");

                return engine;
            }
        }

        public float GetThrottleSetting() => engine?.throttleSetting ?? 0f;
    }
}
