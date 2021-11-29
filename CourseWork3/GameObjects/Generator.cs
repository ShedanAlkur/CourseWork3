using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class Generator<T> : ControlledObject<Generator<T>> where T: Projectile
    {
        public static new Dictionary<string, Action<Generator<T>, object>> ParserMethods 
            = new Dictionary<string, Action<Generator<T>, object>>
        {

        };

        public GameObject Owner;
        public float Sector;
        public float SpawnDelay;
        public int SpawnCount;
        public float Angle;
        public float RotationSpeed;
        public Pattern<Projectile> ProjPattern;

        public float CurrentSpawnDelay;

        static Generator()
        {
            ParserMethods = new Dictionary<string, Action<Generator<T>, object>>
            {
                [$"set-Sector".ToLower()] = (Generator<T> obj, object value) => obj.Sector = (float)value,
                [$"inc-Sector".ToLower()] = (Generator<T> obj, object value) => obj.Sector += (float)value,

                [$"set-SpawnDelay".ToLower()] = (Generator<T> obj, object value) => obj.SpawnDelay = (float)value,
                [$"inc-SpawnDelay".ToLower()] = (Generator<T> obj, object value) => obj.SpawnDelay += (float)value,

                [$"set-SpawnCount".ToLower()] = (Generator<T> obj, object value) => obj.SpawnCount = (int)value,
                [$"inc-SpawnCount".ToLower()] = (Generator<T> obj, object value) => obj.SpawnCount += (int)value,

                [$"set-Angle".ToLower()] = (Generator<T> obj, object value) => obj.Angle = (float)value,
                [$"inc-Angle".ToLower()] = (Generator<T> obj, object value) => obj.Angle += (float)value,

                [$"set-RotationSpeed".ToLower()] = (Generator<T> obj, object value) => obj.RotationSpeed = (float)value,
                [$"inc-RotationSpeed".ToLower()] = (Generator<T> obj, object value) => obj.RotationSpeed += (float)value,

                [$"set-ProjPattern".ToLower()] = (Generator<T> obj, object value) => obj.ProjPattern = (Pattern<Projectile>)value,
                [$"clear-ProjPattern".ToLower()] = (Generator<T> obj, object value) => obj.ProjPattern = null,
            };
        }

        public Generator(Pattern<Generator<T>> pattern, GameObject owner) : base(pattern)
        {
            this.Owner = owner;
            this.Position = owner.Position;
        }
        public Generator(Pattern<Generator<T>> pattern, Vector2 position) : base(pattern)
        {
            this.Position = position;
        }

        public override void Update(float elapsedTime)
        {
            if (Owner.Terminated) this.Terminated = true;
            base.Update(elapsedTime);
            this.Position = Owner.Position;

            CurrentSpawnDelay += elapsedTime;

            while (CurrentSpawnDelay >= SpawnDelay)
            {
                CurrentSpawnDelay -= SpawnDelay;

                float angleDelay = Sector / SpawnCount;
                for (float spawnAngle = Angle - Sector / 2; spawnAngle <= Angle + Sector / 2; spawnAngle+=angleDelay)
                {
                    // TODO: spawn projectiles
                }
            }
        }
    }
}
