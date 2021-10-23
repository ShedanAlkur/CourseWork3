using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    partial class GameMain
    {
        public class GameCamera : IUpdateable
        {
            public enum MovementType
            {
                Instant,
                Linear,
                QuadraticInOut,
                CubicInOut,
                QuarticOut,
                BounceOut,
            }

            private Vector2 position;
            public Vector2 Position
            {
                get => position;
                set
                {
                    position = positionFrom = positionGoto = value;
                    movementType = MovementType.Instant;
                    currentMovementTime = 0;
                    maxMovementTime = 0;
                }
            }

            public float Rotation;
            public float Scale;

            private Vector2 positionFrom;
            private Vector2 positionGoto;
            private float currentMovementTime;
            private float maxMovementTime;

            private MovementType movementType;

            public GameCamera(Vector2 position, MovementType tweenType = MovementType.Instant, float scale = 1.0f, float rotation = 0)
            {
                this.position = position;

                this.positionGoto = position;

                this.movementType = tweenType;

                this.Scale = scale;
                this.Rotation = rotation;
            }

            public Vector2 ToWorld(Vector2 screenCoord)
            {
                screenCoord /= Scale;
                Vector2 dx = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));
                Vector2 dy = new Vector2(MathF.Sin(-Rotation), MathF.Cos(Rotation));

                return position + dx * screenCoord.X + dy * screenCoord.Y;
            }

            public void MoveTo(Vector2 positionGoto, MovementType movementType, float movementTime)
            {
                this.positionFrom = this.position;
                this.positionGoto = positionGoto;
                this.position = positionGoto;
                this.movementType = movementType;
                currentMovementTime = 0;
                maxMovementTime = movementTime;
            }

            public void Update(float elapsedTime)
            {
                if (currentMovementTime < maxMovementTime)
                {
                    currentMovementTime += elapsedTime;

                    position = positionFrom + (positionGoto - positionFrom)
                        * TweenValue(currentMovementTime / maxMovementTime);
                }
                else
                {
                    position = positionGoto;
                }
            }

            private float TweenValue(float t)
            {
                switch (movementType)
                {
                    case MovementType.Linear:
                        return t;
                    case MovementType.QuadraticInOut:
                        return ((t * t) / ((2 * t * t) - (2 * t) + 1));
                    case MovementType.CubicInOut:
                        return ((t * t * t) / ((3 * t * t) - (3 * t) + 1));
                    case MovementType.QuarticOut:
                        return (-((t - 1) * (t - 1) * (t - 1) * (t - 1)) + 1);
                    case MovementType.BounceOut:
                        float p = 0.3f;
                        return ((float)MathF.Pow(2, -10 * t) * (float)MathF.Sin((t - p / 4) * (2 * MathF.PI) / p) + 1);
                    case MovementType.Instant:
                        return 1;
                    default:
                        throw new ArgumentException($"Недопустимый параметр {nameof(t)}", nameof(t));
                }
            }
        }
    }
}
