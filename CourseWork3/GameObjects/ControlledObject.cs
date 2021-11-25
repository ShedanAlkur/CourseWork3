using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class ControlledObject<T> : GameObject where T : ControlledObject<T>
    {
        public static Dictionary<string, Action<T, object>> PropertiesMethods = new Dictionary<string, Action<T, object>>
        {
            
        };

        public readonly Pattern<T> Pattern;

        public int CurrentIndex;
        public float CurrentRuntime;
        public float MaxRuntime;

        static ControlledObject()
        {
            PropertiesMethods = new Dictionary<string, Action<T, object>>
            {
                [$"set-position".ToLower()] = (T obj, object value) => obj.Position = (Vector2)value,
                [$"inc-position".ToLower()] = (T obj, object value) => obj.Position += (Vector2)value,
                [$"set-velocity".ToLower()] = (T obj, object value) => obj.Velocity = (Vector2)value,
                [$"inc-velocity".ToLower()] = (T obj, object value) => obj.Velocity += (Vector2)value,
            };
        }

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
