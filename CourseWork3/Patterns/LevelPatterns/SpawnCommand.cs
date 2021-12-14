using CourseWork3.Game;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class SpawnCommand : ILevelCommand
    {
        Pattern<Enemy> enemyPattern;
        Vector2 position;

        public SpawnCommand(Pattern<Enemy> enemyPattern, Vector2 position)
        {
            this.enemyPattern = enemyPattern;
            this.position = position;
        }

        public void Invoke()
        {
            GameMain.World.Add(new Enemy(enemyPattern, position));
        }

        public override string ToString()
        {
            return $"Spawn at {position}";
        }
    }
}
