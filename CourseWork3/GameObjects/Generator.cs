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
        public static Dictionary<string, Dictionary<string, Action<Generator<T>, object>>> ParserActionByTwoCommand;

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
            ParserActionByTwoCommand = new Dictionary<string, Dictionary<string, Action<Generator<T>, object>>>
            {
                [Keywords.Set] = new Dictionary<string, Action<Generator<T>, object>>
                {
                    [Keywords.Sector] = (Generator<T> obj, object value) => obj.Sector = (float)value,
                    [Keywords.SpawnDelay] = (Generator<T> obj, object value) => obj.SpawnDelay = (float)value,
                    [Keywords.SpawnCount] = (Generator<T> obj, object value) => obj.SpawnCount = (int)value,
                    [Keywords.Angle] = (Generator<T> obj, object value) => obj.Angle = (float)value,
                    [Keywords.Sprite] = (Generator<T> obj, object value) => throw new NotImplementedException(),
                    [Keywords.Projectile] = (Generator<T> obj, object value) => obj.ProjPattern = (Pattern<Projectile>)value,
                },
                [Keywords.Increase] = new Dictionary<string, Action<Generator<T>, object>>
                {
                    [Keywords.Sector] = (Generator<T> obj, object value) => obj.Sector = (float)value,
                    [Keywords.SpawnDelay] = (Generator<T> obj, object value) => obj.SpawnDelay = (float)value,
                    [Keywords.SpawnCount] = (Generator<T> obj, object value) => obj.SpawnCount = (int)value,
                    [Keywords.Angle] = (Generator<T> obj, object value) => obj.Angle = (float)value,
                    [Keywords.Sprite] = (Generator<T> obj, object value) => throw new NotImplementedException(),
                    [Keywords.Projectile] = (Generator<T> obj, object value) => obj.ProjPattern = (Pattern<Projectile>)value,
                },
                [Keywords.Clear] = new Dictionary<string, Action<Generator<T>, object>>
                {
                    [Keywords.Sprite] = (Generator<T> obj, object value) => throw new NotImplementedException(),
                    [Keywords.Projectile] = (Generator<T> obj, object value) => obj.ProjPattern = null,
                },
            };
            ControlledObject<Generator<T>>.ParserActionByTwoCommand.ToList().ForEach(firstCommand =>
                firstCommand.Value.ToList().ForEach(secondCommand =>
                ParserActionByTwoCommand[firstCommand.Key].Add(secondCommand.Key, secondCommand.Value)));
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
