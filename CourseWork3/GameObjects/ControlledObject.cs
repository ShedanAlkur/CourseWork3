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
        public static Dictionary<string, Action<T, object>> ActionsForParser;

        public readonly Pattern<T> Pattern;

        public int CurrentIndex;
        public float CurrentRuntime;
        public float MaxRuntime;
        public float CurrentPauseTime;
        public bool IsSelectedRuntimeCommand;

        static ControlledObject()
        {
            ActionsForParser = new Dictionary<string, Action<T, object>>
            {
                [Keywords.Set + Keywords.PositionX] = (T obj, object value) => obj.Position.X = (float)value,
                [Keywords.Set + Keywords.PositionY] = (T obj, object value) => obj.Position.Y = (float)value,
                [Keywords.Set + Keywords.VelocityScalar] = (T obj, object value) => obj.VelocityScalar = (float)value,
                [Keywords.Set + Keywords.VelocityAngle] = (T obj, object value) => obj.VelocityAngle = (float)value,
                [Keywords.Set + Keywords.AccelerationScalar] = (T obj, object value) => obj.AccelerationScalar = (float)value,
                [Keywords.Set + Keywords.AccelerationAngle] = (T obj, object value) => obj.AccelerationAngle = (float)value,

                [Keywords.Increase + Keywords.PositionX] = (T obj, object value) => obj.Position.X += (float)value,
                [Keywords.Increase + Keywords.PositionY] = (T obj, object value) => obj.Position.Y += (float)value,
                [Keywords.Increase + Keywords.VelocityScalar] = (T obj, object value) => obj.VelocityScalar += (float)value,
                [Keywords.Increase + Keywords.VelocityAngle] = (T obj, object value) => obj.VelocityAngle += (float)value,
                [Keywords.Increase + Keywords.AccelerationScalar] = (T obj, object value) => obj.AccelerationScalar += (float)value,
                [Keywords.Increase + Keywords.AccelerationAngle] = (T obj, object value) => obj.AccelerationAngle += (float)value,

                [Keywords.Pause] = (T obj, object value) => { obj.CurrentPauseTime = (float)value; },
                [Keywords.Destroy] = (T obj, object value) => { obj.Terminated = true; },
                [Keywords.Runtime] = (T obj, object value) =>
                {
                    obj.CurrentRuntime = 0;
                    obj.MaxRuntime = (float)value;
                    obj.IsSelectedRuntimeCommand = true;
                },

                [Keywords.VelocityToPoint] = (T obj, object value) => obj.VelocityAngle = (obj.Position - (Vector2)value).GetAngle(),
                [Keywords.velocityToPlayer] = (T obj, object value) =>
                    obj.VelocityAngle = (obj.Position - GameMain.World.Player.Position).GetAngle(),
                [Keywords.PointRotation] = (T obj, object value) => obj.PointRotation((Vector2)value),
            };
        }

        public void PointRotation(Vector2 center)
        {
            
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
