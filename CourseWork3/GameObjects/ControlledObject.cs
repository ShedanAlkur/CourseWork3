using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class ControlledObject<T> : GameObject where T : ControlledObject<T>
    {
        public static Dictionary<string, Action<T, object>> ParserMethods = new Dictionary<string, Action<T, object>>
        {
            
        };

        public readonly Pattern<T> Pattern;

        public int CurrentIndex;
        public float CurrentRuntime;
        public float MaxRuntime;

        static ControlledObject()
        {
            ParserMethods = new Dictionary<string, Action<T, object>>
            {
                [$"set-position".ToLower()] = (T obj, object value) => obj.Position = (Vector2)value,
                [$"inc-position".ToLower()] = (T obj, object value) => obj.Position += (Vector2)value,

                [$"set-velocity".ToLower()] = (T obj, object value) => obj.Position = (Vector2)value,
                [$"inc-velocity".ToLower()] = (T obj, object value) => obj.Position += (Vector2)value,

                [$"set-velocityScalar".ToLower()] = (T obj, object value) => obj.VelocityScalar = (float)value,
                [$"inc-velocityScalar".ToLower()] = (T obj, object value) => obj.VelocityScalar += (float)value,

                [$"set-velocityAngle".ToLower()] = (T obj, object value) => obj.VelocityAngle = (float)value,
                [$"inc-velocityAngle".ToLower()] = (T obj, object value) => obj.VelocityAngle += (float)value,

                [$"set-accelerationScalar".ToLower()] = (T obj, object value) => obj.AccelerationScalar = (float)value,
                [$"inc-accelerationScalar".ToLower()] = (T obj, object value) => obj.AccelerationScalar += (float)value,

                [$"set-accelerationAngle".ToLower()] = (T obj, object value) => obj.AccelerationAngle = (float)value,
                [$"inc-accelerationAngle".ToLower()] = (T obj, object value) => obj.AccelerationAngle += (float)value,
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
                Pattern.Invoke((T)this);

            base.Update(elapsedTime);
        }
    }
}
