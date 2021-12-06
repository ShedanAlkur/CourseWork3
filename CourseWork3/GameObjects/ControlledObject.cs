using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;
using CourseWork3.Parser;

namespace CourseWork3.Game
{
    class ControlledObject<T> : GameObject where T : ControlledObject<T>
    {
        public static Dictionary<string, Dictionary<string, Action<T, object>>> ParserActionByTwoCommand;
        public static Dictionary<string, Action<T, object>> ParserActionByOneCommand;

        public readonly Pattern<T> Pattern;

        public int CurrentIndex;
        public float CurrentRuntime;
        public float MaxRuntime;
        public float CurrentPauseTime;

        static ControlledObject()
        {
            ParserActionByTwoCommand = new Dictionary<string, Dictionary<string, Action<T, object>>>
            {
                [Keywords.Set] = new Dictionary<string, Action<T, object>>
                {
                    [Keywords.PositionX] = (T obj, object value) => obj.Position.X = (float)value,
                    [Keywords.PositionY] = (T obj, object value) => obj.Position.Y = (float)value,
                    [Keywords.VelocityScalar] = (T obj, object value) => obj.VelocityScalar = (float)value,
                    [Keywords.VelocityAngle] = (T obj, object value) => obj.VelocityAngle = (float)value,
                    [Keywords.AccelerationScalar] = (T obj, object value) => obj.AccelerationScalar = (float)value,
                    [Keywords.AccelerationAngle] = (T obj, object value) => obj.AccelerationAngle = (float)value,
                },
                [Keywords.Increase] = new Dictionary<string, Action<T, object>>
                {
                    [Keywords.PositionX] = (T obj, object value) => obj.Position.X += (float)value,
                    [Keywords.PositionY] = (T obj, object value) => obj.Position.Y += (float)value,
                    [Keywords.VelocityScalar] = (T obj, object value) => obj.VelocityScalar += (float)value,
                    [Keywords.VelocityAngle] = (T obj, object value) => obj.VelocityAngle += (float)value,
                    [Keywords.AccelerationScalar] = (T obj, object value) => obj.AccelerationScalar += (float)value,
                    [Keywords.AccelerationAngle] = (T obj, object value) => obj.AccelerationAngle += (float)value,
                },
            };

            ParserActionByOneCommand = new Dictionary<string, Action<T, object>>
            {
                [Keywords.Pause] = (T obj, object value) => { obj.CurrentPauseTime = (float)value; },
                [Keywords.Runtime] = (T obj, object value) => { obj.CurrentRuntime = 0; obj.MaxRuntime = (float)value; }

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
            if (CurrentPauseTime > 0)
            {
                CurrentPauseTime -= elapsedTime;
                if (CurrentPauseTime > 0) return;
                else 
                { 
                    elapsedTime = -CurrentPauseTime;
                    CurrentPauseTime = 0;
                }
            }

            CurrentRuntime += elapsedTime;
            if (CurrentRuntime >= MaxRuntime)
                Pattern.Invoke((T)this);

            base.Update(elapsedTime);
        }
    }
}
