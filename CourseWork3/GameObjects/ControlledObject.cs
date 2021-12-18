using CourseWork3.Game;
using CourseWork3.Parser;
using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CourseWork3.GameObjects
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
                [Keywords.Set + Keywords.VelocityAngle] = (T obj, object value) => obj.VelocityAngle = MathHelper.DegreesToRadians((float)value) - MathHelper.PiOver2,
                [Keywords.Set + Keywords.AccelerationScalar] = (T obj, object value) => obj.AccelerationScalar = (float)value,
                [Keywords.Set + Keywords.AccelerationAngle] = (T obj, object value) => obj.AccelerationAngle = MathHelper.DegreesToRadians((float)value),
                [Keywords.Set + Keywords.Hitbox] = (T obj, object value) => obj.HitBoxSize = (float)value,

                [Keywords.Increase + Keywords.PositionX] = (T obj, object value) => obj.Position.X += (float)value,
                [Keywords.Increase + Keywords.PositionY] = (T obj, object value) => obj.Position.Y += (float)value,
                [Keywords.Increase + Keywords.VelocityScalar] = (T obj, object value) => obj.VelocityScalar += (float)value,
                [Keywords.Increase + Keywords.VelocityAngle] = (T obj, object value) => obj.VelocityAngle += MathHelper.DegreesToRadians((float)value),
                [Keywords.Increase + Keywords.AccelerationScalar] = (T obj, object value) => obj.AccelerationScalar += (float)value,
                [Keywords.Increase + Keywords.AccelerationAngle] = (T obj, object value) => obj.AccelerationAngle += MathHelper.DegreesToRadians((float)value),
                [Keywords.Increase + Keywords.Hitbox] = (T obj, object value) => obj.HitBoxSize += (float)value,

                [Keywords.Pause] = (T obj, object value) => { obj.CurrentPauseTime = (float)value; obj.IsPaused = true; },
                [Keywords.Destroy] = (T obj, object value) => { obj.Terminated = true; },
                [Keywords.Runtime] = (T obj, object value) =>
                {
                    obj.CurrentRuntime = 0;
                    obj.MaxRuntime = (float)value;
                    obj.IsSelectedRuntimeCommand = true;
                },

                [Keywords.VelocityToPoint] = (T obj, object value) => obj.VelocityAngle = ((Vector2)value - obj.Position).GetAngle(),
                [Keywords.velocityToPlayer] = (T obj, object value) =>
                    obj.VelocityAngle = (GameMain.World.Player.Position - obj.Position).GetAngle(),
                [Keywords.PointRotation] = (T obj, object value) => obj.PointRotation((Vector2)value, true),
                [Keywords.PointCounterRotation] = (T obj, object value) => obj.PointRotation((Vector2)value, false),
            };
        }

        public void PointRotation(Vector2 center, bool clockwise)
        {
            float radius = (Position - center).Length;
            VelocityAngle = (center - Position).GetAngle() + ((clockwise) ? MathHelper.PiOver2 : -MathHelper.PiOver2);
            AccelerationAngle = (clockwise) ? -MathHelper.PiOver2 : +MathHelper.PiOver2;
            AccelerationScalar = VelocityScalar * VelocityScalar / radius;
        }

        public ControlledObject(Pattern<T> pattern, Vector2 position) : base(position)
        {
            CurrentIndex = 0;
            CurrentRuntime = 0;
            MaxRuntime = 0;
            this.Pattern = pattern;
        }

        public bool IsPaused;

        public override void Update(float elapsedTime)
        {
            if (CurrentPauseTime > 0)
            {
                CurrentPauseTime -= elapsedTime;
                if (CurrentPauseTime > 0)
                {
                    IsPaused = true;
                    return;
                }
                else
                {
                    elapsedTime = -CurrentPauseTime;
                    CurrentPauseTime = 0;
                }
            }
            IsPaused = false;

            CurrentRuntime += elapsedTime;
            if (CurrentRuntime >= MaxRuntime)
                Pattern.Invoke((T)this);

            base.Update(elapsedTime);
        }
    }
}
