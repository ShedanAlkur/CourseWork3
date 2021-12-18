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

        private float sector;
        public float Sector 
        { 
            get => sector;
            set => sector = value;
        }

        private float spawnDelay;
        public float SpawnDelay { get => spawnDelay; set { spawnDelay = value; CurrentSpawnDelay = value; } }

        private int spawnCount;
        public int SpawnCount 
        {
            get => spawnCount; 
            set => spawnCount = value;
        }

        public float Angle = -MathHelper.PiOver2;

        public float RotationSpeed;

        public float RotationAcceleration;

        public Pattern<Projectile> ProjPattern;


        public float CurrentSpawnDelay;


        static Generator()
        {
            ActionsForParser = new Dictionary<string, Action<Generator, object>>
            {
                [Keywords.Set + Keywords.Sector] = (Generator obj, object value) => obj.Sector = MathHelper.DegreesToRadians((float)value),
                [Keywords.Set + Keywords.SpawnDelay] = (Generator obj, object value) => obj.SpawnDelay = (float)value,
                [Keywords.Set + Keywords.SpawnCount] = (Generator obj, object value) => obj.SpawnCount = (int)value,
                [Keywords.Set + Keywords.Angle] = (Generator obj, object value) => obj.Angle = MathHelper.DegreesToRadians((float)value) - MathHelper.PiOver2,
                [Keywords.Set + Keywords.Sprite] = (Generator obj, object value) => throw new NotImplementedException(),
                [Keywords.Set + Keywords.Projectile] = (Generator obj, object value) => obj.ProjPattern = (Pattern<Projectile>)value,
                [Keywords.Set + Keywords.RotationSpeed] = (Generator obj, object value) => obj.RotationSpeed = MathHelper.DegreesToRadians((float)value),
                [Keywords.Set + Keywords.RotationAcceleration] = (Generator obj, object value) => obj.RotationAcceleration = MathHelper.DegreesToRadians((float)value),

                [Keywords.Increase + Keywords.Sector] = (Generator obj, object value) => obj.Sector = MathHelper.DegreesToRadians((float)value),
                [Keywords.Increase + Keywords.SpawnDelay] = (Generator obj, object value) => obj.SpawnDelay = (float)value,
                [Keywords.Increase + Keywords.SpawnCount] = (Generator obj, object value) => obj.SpawnCount = (int)value,
                [Keywords.Increase + Keywords.Angle] = (Generator obj, object value) => obj.Angle = MathHelper.DegreesToRadians((float)value),
                [Keywords.Increase + Keywords.RotationSpeed] = (Generator obj, object value) => obj.RotationSpeed += MathHelper.DegreesToRadians((float)value),
                [Keywords.Increase + Keywords.RotationAcceleration] = (Generator obj, object value) => obj.RotationAcceleration += MathHelper.DegreesToRadians((float)value),

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
            if (IsPaused) return;

            Angle += RotationSpeed * elapsedTime;
            RotationSpeed += RotationAcceleration * elapsedTime;

            CurrentSpawnDelay += elapsedTime;
            while (CurrentSpawnDelay >= SpawnDelay)
            {
                CurrentSpawnDelay -= SpawnDelay;

                if (ProjPattern == null || SpawnDelay == 0 || Sector == 0) continue;
                float sectorBetweenProj = Sector / SpawnCount;

                float angle1 = Angle - Sector / 2f + sectorBetweenProj / 2f;
                float angle2 = Angle + Sector / 2f;

                for (float spawnAngle = angle1; spawnAngle <= angle2; spawnAngle += sectorBetweenProj)
                {
                    GameMain.World.Add(new Projectile(ProjPattern, Position, spawnAngle, CurrentRuntime, isEnemyGenerator));
                }
            }
        }
    }
}
