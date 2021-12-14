using CourseWork3;
using CourseWork3.GraphicsOpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    abstract class GameObject : IRenderable, IUpdateable
    {
        const float AngleAccuracy = MathHelper.Pi / 180 * 3; // ~3 градуса

        public Vector2 Position;

        private Vector2 velocity;

        public Vector2 Velocity
        {
            get => velocity;
            set
            {
                velocity = value;
                velocityScalar = velocity.LengthFast;
                velocityAngle = velocity.GetAngle() + MathHelper.PiOver2;
            }
        }

        private float velocityScalar;
        public float VelocityScalar
        {
            get => velocityScalar;
            set
            {
                velocityScalar = value;
                velocity = Vector2Ext.ByAngle(velocityAngle) * velocityScalar;
            }
        }

        private float velocityAngle;
        public float VelocityAngle
        {
            get => velocityAngle;
            set
            {
                velocityAngle = value;
                velocity = Vector2Ext.ByAngle(velocityAngle) * velocityScalar;
            }
        }


        public float AccelerationScalar;

        private float accelerationAngle;
        public float AccelerationAngle
        {
            get => accelerationAngle;
            set 
            {
                accelerationAngle = value;
                accelerationAngleApproximatelyEqualPi = AnglesApproximatelyEqualCheck(accelerationAngle, MathHelper.Pi);
            }
        }

        private bool accelerationAngleApproximatelyEqualPi = false;

        public bool Terminated;


        public GameObject(Vector2 position)
        {
            this.Position = position;
        }

        public virtual void Update(float elapsedTime)
        {
            if (Velocity != Vector2.Zero)
            {
                // Обновление позиции от скорости
                Position += Velocity * elapsedTime;
            }
            if (AccelerationScalar != 0)
            {
                if (accelerationAngleApproximatelyEqualPi)
                {
                    // Обновление позиции от ускорения
                    Vector2 acceleration = -AccelerationScalar * Velocity.Normalized();
                    Position += acceleration * elapsedTime * elapsedTime / 2;
                    // Обновление скорости от ускорения
                    Vector2 temp = acceleration * elapsedTime;
                    Velocity += temp;

                    // Если из-за ускорения скорость стала противоположно направлена, то сонаправить ускорение скорости
                    if (AnglesApproximatelyEqualCheck((temp).GetAngle(), -Velocity.GetAngle()))
                    {
                        AccelerationAngle = 0;
                    }
                }
                else
                {
                    // Обновление позиции от ускорения
                    Vector2 acceleration = AccelerationScalar * Vector2Ext.ByAngle(AccelerationAngle + velocityAngle);
                    Position += acceleration * elapsedTime * elapsedTime / 2;
                    // Обновление скорости от ускорения
                    Velocity += acceleration * elapsedTime;
                }

            }
        }

        public virtual void Draw() { }

        public virtual void OnCollision(GameObject gameObject) { }

        private static bool AnglesApproximatelyEqualCheck(float angle1, float angle2)
            => (MathF.Abs(angle1 - angle2) <= AngleAccuracy);
    }
}
