using System;
using System.Collections.Generic;
using System.Text;
using CourseWork3;
using CourseWork3.GraphicsOpenGL;
using OpenTK;

namespace CourseWork3.Game
{
    abstract class GameObject : IRenderable, IUpdateable
    {
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

        public float AccelerationAngle;

        public bool Terminated;
        public virtual void Update(float elapsedTime)
        {
            if (Velocity != Vector2.Zero)
            {
                // Обновление позиции от скорости
                Position += Velocity * elapsedTime;
            }
            if (AccelerationScalar != 0)
            {
                if(AccelerationAngle == MathHelper.Pi)
                {
                    // Обновление позиции от ускорения
                    Vector2 acceleration = -AccelerationScalar * Velocity.Normalized();
                    Position += acceleration * elapsedTime * elapsedTime / 2;
                    // Обновление скорости от ускорения
                    Velocity += acceleration * elapsedTime;
                    // Если из-за ускорения скорость стала противоположно направлена, то сонаправить ускорение скорости

                    if ((acceleration * elapsedTime).GetAngle() == -Velocity.GetAngle())
                    {
                        Console.WriteLine($"value1 {acceleration * elapsedTime}");
                        Console.WriteLine($"value2 {Velocity}");
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
    }
}
