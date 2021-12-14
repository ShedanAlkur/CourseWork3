using CourseWork3.Parser;
using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseWork3.Game
{
    class Generator : ControlledObject<Generator>
    {
        public static new Dictionary<string, Action<Generator, object>> ActionsForParser;

        private bool isEnemyGenerator;

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
            ActionsForParser = new Dictionary<string, Action<Generator, object>>
            {
                [Keywords.Set + Keywords.Sector] = (Generator obj, object value) => obj.Sector = (float)value,
                [Keywords.Set + Keywords.SpawnDelay] = (Generator obj, object value) => obj.SpawnDelay = (float)value,
                [Keywords.Set + Keywords.SpawnCount] = (Generator obj, object value) => obj.SpawnCount = (int)value,
                [Keywords.Set + Keywords.Angle] = (Generator obj, object value) => obj.Angle = (float)value - MathHelper.PiOver2,
                [Keywords.Set + Keywords.Sprite] = (Generator obj, object value) => throw new NotImplementedException(),
                [Keywords.Set + Keywords.Projectile] = (Generator obj, object value) => obj.ProjPattern = (Pattern<Projectile>)value,
                [Keywords.Set + Keywords.RotationSpeed] = (Generator obj, object value) => obj.RotationSpeed = (float)value,

                [Keywords.Increase + Keywords.Sector] = (Generator obj, object value) => obj.Sector = (float)value,
                [Keywords.Increase + Keywords.SpawnDelay] = (Generator obj, object value) => obj.SpawnDelay = (float)value,
                [Keywords.Increase + Keywords.SpawnCount] = (Generator obj, object value) => obj.SpawnCount = (int)value,
                [Keywords.Increase + Keywords.Angle] = (Generator obj, object value) => obj.Angle = (float)value,
                [Keywords.Set + Keywords.RotationSpeed] = (Generator obj, object value) => obj.RotationSpeed += (float)value,

                [Keywords.Clear + Keywords.Sprite] = (Generator obj, object value) => throw new NotImplementedException(),
                [Keywords.Clear + Keywords.Projectile] = (Generator obj, object value) => obj.ProjPattern = null,

                [Keywords.AimToPlayer] = (Generator obj, object value) => obj.Angle = (GameMain.World.Player.Position - obj.Position).GetAngle(),
            };
            ControlledObject<Generator>.ActionsForParser.ToList().ForEach(x =>
                ActionsForParser.Add(x.Key, x.Value));
        }

        public Generator(Pattern<Generator> pattern, GameObject owner) : base(pattern, owner.Position)
        {
            this.Owner = owner;
            isEnemyGenerator = !(owner is Player);
        }
        public Generator(Pattern<Generator> pattern, Vector2 position, bool isEnemyGenerator) : base(pattern, position)
        {
            this.isEnemyGenerator = isEnemyGenerator;
        }

        public override void Update(float elapsedTime)
        {
            if (Owner != null && Owner.Terminated) this.Terminated = true;

            base.Update(elapsedTime);
            this.Position = Owner.Position;

            Angle += RotationSpeed * elapsedTime;

            CurrentSpawnDelay += elapsedTime;
            while (CurrentSpawnDelay >= SpawnDelay)
            {
                CurrentSpawnDelay -= SpawnDelay;

                float sectorBetweenProj = Sector / SpawnCount;

                for (float spawnAngle = Angle - Sector / 2 + (((SpawnCount & 1) == 0) ? sectorBetweenProj / 2f : 0);
                    spawnAngle <= Angle + Sector / 2; spawnAngle += sectorBetweenProj)
                {
                    GameMain.World.Add(new Projectile(ProjPattern, Position, spawnAngle, isEnemyGenerator));
                }
            }
        }
    }
}
