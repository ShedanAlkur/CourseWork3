using CourseWork3.Parser;
using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseWork3.Game
{
    class Generator<T> : ControlledObject<Generator<T>> where T: Projectile
    {
        public static new Dictionary<string, Action<Generator<T>, object>> ActionsForParser;

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
            ActionsForParser = new Dictionary<string, Action<Generator<T>, object>>
            {
                [Keywords.Set + Keywords.Sector] = (Generator<T> obj, object value) => obj.Sector = (float)value,
                [Keywords.Set + Keywords.SpawnDelay] = (Generator<T> obj, object value) => obj.SpawnDelay = (float)value,
                [Keywords.Set + Keywords.SpawnCount] = (Generator<T> obj, object value) => obj.SpawnCount = (int)value,
                [Keywords.Set + Keywords.Angle] = (Generator<T> obj, object value) => obj.Angle = (float)value,
                [Keywords.Set + Keywords.Sprite] = (Generator<T> obj, object value) => throw new NotImplementedException(),
                [Keywords.Set + Keywords.Projectile] = (Generator<T> obj, object value) => obj.ProjPattern = (Pattern<Projectile>)value,

                [Keywords.Increase + Keywords.Sector] = (Generator<T> obj, object value) => obj.Sector = (float)value,
                [Keywords.Increase + Keywords.SpawnDelay] = (Generator<T> obj, object value) => obj.SpawnDelay = (float)value,
                [Keywords.Increase + Keywords.SpawnCount] = (Generator<T> obj, object value) => obj.SpawnCount = (int)value,
                [Keywords.Increase + Keywords.Angle] = (Generator<T> obj, object value) => obj.Angle = (float)value,

                [Keywords.Clear + Keywords.Sprite] = (Generator<T> obj, object value) => throw new NotImplementedException(),
                [Keywords.Clear + Keywords.Projectile] = (Generator<T> obj, object value) => obj.ProjPattern = null,
            };
            ControlledObject<Generator<T>>.ActionsForParser.ToList().ForEach(x =>
                ActionsForParser.Add(x.Key, x.Value));
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
