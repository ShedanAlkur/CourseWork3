using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class ControlledObject<T> : GameObject where T : ControlledObject<T>
    {
        public readonly Pattern<T> Pattern;

        public int CurrentIndex;
        public float CurrentRuntime;
        public float MaxRuntime;

        public ControlledObject(Pattern<T> pattern) : base()
        {
            CurrentIndex = 0;
            CurrentRuntime = 0;
            MaxRuntime = 0;
            this.Pattern = pattern;
        }

        public override void Update(float elapsedTime)
        {
            CurrentRuntime += elapsedTime;
            if (CurrentRuntime >= MaxRuntime)
                Pattern.Invoke(this);

            base.Update(elapsedTime);
        }
    }
}
