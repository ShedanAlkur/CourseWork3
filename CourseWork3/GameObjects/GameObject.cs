using CourseWork3.Game;
using OpenTK;
using System;

namespace CourseWork3.GameObjects
{
    abstract class GameObject : IRenderable, IUpdateable
    {
        const float AngleAccuracy = MathHelper.Pi / 180 * 6;

        public Vector2 Position;

        private Vector2 velocity;

        public Vector2 Velocity
        {
            get => velocity;
            set
            {
                velocity = value;
                velocityScalar = velocity.LengthFast;
                if (velocity != Vector2.Zero) velocityAngle = velocity.GetAngle();
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
                while (accelerationAngle < 0) accelerationAngle += MathHelper.TwoPi;
                while (accelerationAngle > MathHelper.TwoPi) accelerationAngle -= MathHelper.TwoPi;
                accelerationAngleApproximatelyEqualPi = AnglesApproximatelyEqualCheck(accelerationAngle, MathHelper.Pi);
            }
        }

        private bool accelerationAngleApproximatelyEqualPi = false;

        public bool Terminated;

        private float hitBoxSize;
        public virtual float HitBoxSize
        {
            get => hitBoxSize;
            set
            {
                hitBoxSize = value;
                halfHitBoxSize = hitBoxSize / 2f;
            }
        }

        private float halfHitBoxSize;
        public float HalfHitBoxSize
        {
            get => halfHitBoxSize;
        }

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
                    Vector2 acceleration = -AccelerationScalar * Velocity.FastNormilize(); // Использую свой нормализирующий метод, так как стандарный Vector2.Normilized() возвращает вектор (NaN, NaN), например, для вектора (0, -0).        
                    Position += acceleration * elapsedTime * elapsedTime / 2;
                    // Обновление скорости от ускорения
                    Vector2 temp = acceleration * elapsedTime;
                    Velocity += temp;

                    // Если из-за ускорения скорость стала противоположно направлена, то сонаправить ускорение скорости
                    if (AnglesApproximatelyEqualCheck((temp).GetAngle(), Velocity.GetAngle()))
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
            => (MathF.Abs((angle1) - (angle2)) <= AngleAccuracy);

        public bool SqrCollisionCheck(GameObject gameObject)
        {
            return
                this.Position.Y - this.HalfHitBoxSize < gameObject.Position.Y + gameObject.HalfHitBoxSize &&
                this.Position.Y + this.HalfHitBoxSize > gameObject.Position.Y - gameObject.HalfHitBoxSize &&
                this.Position.X + this.HalfHitBoxSize > gameObject.Position.X - gameObject.HalfHitBoxSize &&
                this.Position.X - this.HalfHitBoxSize < gameObject.Position.X + gameObject.HalfHitBoxSize;
        }
        public bool RoundCollisionCheck(GameObject gameObject)
        {

            return (this.Position - gameObject.Position).LengthSquared <=
                (this.halfHitBoxSize + gameObject.halfHitBoxSize).Sqr();
        }

        public bool WorldCollisionCheck()
        {
            return
                this.Position.Y - this.HalfHitBoxSize < World.TopLeftPoint.Y &&
                this.Position.Y + this.HalfHitBoxSize > World.BottomRightPoint.Y &&
                this.Position.X + this.HalfHitBoxSize > World.TopLeftPoint.X &&
                this.Position.X - this.HalfHitBoxSize < World.BottomRightPoint.X;
        }
    }
}
