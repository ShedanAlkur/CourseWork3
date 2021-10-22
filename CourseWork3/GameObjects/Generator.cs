using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class Generator<T> : ControlledObject<Generator<T>> where T: Projectile
    {
        public GameObject Owner;
        public float Sector;
        public float SpawnDelay;
        public int SpawnCount;
        public float Angle;
        public bool SyncProj;
        public Pattern<Projectile> ProjPattern;

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
            // TODO: Добавить стрельбу

            base.Update(elapsedTime);
        }
    }
}
