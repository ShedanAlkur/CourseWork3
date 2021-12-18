using CourseWork3.Game;
using CourseWork3.GameObjects;
using OpenTK;

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
