#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Testing;

namespace B9AnimationModules.UnitTests
{
    public abstract class B9AnimateTestBase<T> : UnitTest where T : ModuleB9AnimateBase
    {
        protected GameObject gameObject;
        protected GameObject gameObject2;
        protected Part part;
        protected T module;

        public override void TestStartUp()
        {
            base.TestStartUp();

            gameObject = new GameObject("part");
            gameObject2 = new GameObject("model");
            gameObject2.transform.parent = gameObject.transform;
            part = gameObject.AddComponent<Part>();
            part.enabled = false;
            module = AddPartModule<T>();
        }

        protected U AddPartModule<U>() where U : PartModule
        {
            U module = gameObject.AddComponent<U>();
            module.enabled = false;
            part.Modules.Add(module);
            return module;
        }

        public override void TestTearDown()
        {
            base.TestTearDown();

            UnityEngine.Object.Destroy(gameObject);
            UnityEngine.Object.Destroy(gameObject2);
        }
    }
}

#endif
