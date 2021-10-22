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
        public Vector2 Velocity;
        public float VelocityAngle;
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
                // Обновление позиции от ускорения
                Vector2 acceleration = AccelerationScalar * Vector2Ext.ByAngle(VelocityAngle);
                Position += acceleration * elapsedTime * elapsedTime / 2;
                // Обновление скорости от ускорения
                Velocity += acceleration * elapsedTime;
                // Обновление угла скорости
                VelocityAngle = Velocity.GetAngle();
                // Если из-за ускорения скорость стала противоположно направлена, то сонаправить ускорение скорости
                if (AccelerationAngle == MathHelper.Pi && (acceleration * elapsedTime).LengthSquared >= Velocity.LengthSquared) 
                    AccelerationAngle = 0;
            }
        }

        public virtual void Draw() { }

        public virtual void OnCollision(GameObject gameObject) { }
    }
}
