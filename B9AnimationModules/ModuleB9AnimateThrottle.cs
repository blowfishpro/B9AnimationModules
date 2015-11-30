using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public override float TargetAnimationState()
        {
            return GetThrottleSetting();
        }

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
                for (int i = 0; i < part.Modules.Count; i++)
                {
                    if (!(part.Modules[i] is ModuleEngines)) continue;

                    ModuleEngines module = part.Modules[i] as ModuleEngines;

                    if (module.engineID != engineID) continue;

                    return module;
                }

                LogError("Cannot find engine module with engineID '" + engineID + "'");

                return null;
            }
        }

        public float GetThrottleSetting()
        {
            return engine?.throttleSetting ?? 0f;
        }
    }
}
