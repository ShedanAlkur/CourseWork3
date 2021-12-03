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
                [$"setPositionX".ToLower()] = (T obj, object value) => obj.Position.X = (float)value,
                [$"incPositionX".ToLower()] = (T obj, object value) => obj.Position.X += (float)value,

                [$"setPositionY".ToLower()] = (T obj, object value) => obj.Position.Y = (float)value,
                [$"incPositionY".ToLower()] = (T obj, object value) => obj.Position.Y += (float)value,

                [$"setVelocity".ToLower()] = (T obj, object value) => obj.VelocityScalar = (float)value,
                [$"incVelocity".ToLower()] = (T obj, object value) => obj.VelocityScalar += (float)value,

                [$"setVelocityAngle".ToLower()] = (T obj, object value) => obj.VelocityAngle = (float)value,
                [$"incVelocityAngle".ToLower()] = (T obj, object value) => obj.VelocityAngle += (float)value,

                [$"setAcceleration".ToLower()] = (T obj, object value) => obj.AccelerationScalar = (float)value,
                [$"incAcceleration".ToLower()] = (T obj, object value) => obj.AccelerationScalar += (float)value,

                [$"setAccelerationAngle".ToLower()] = (T obj, object value) => obj.AccelerationAngle = (float)value,
                [$"setAccelerationAngle".ToLower()] = (T obj, object value) => obj.AccelerationAngle += (float)value,
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
