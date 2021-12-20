using CourseWork3.GameObjects;
using CourseWork3.Patterns;
using OpenTK;
using System.Collections.Generic;

namespace CourseWork3.Game
{
    class World : IUpdateable
    {
        public List<GameObject> gameObjects;

        private static World instance;
        public static World Instance
        {
            get
            {
                if (instance == null)
                    instance = new World();
                return instance;
            }
        }

        public Player Player { get; protected set; }
        public LevelPattern Pattern;
        public int CurrentIndex;
        public float CurrentPausetime;
        public float MaxPausetime;

        public static readonly Vector2 DefaultPlayerPosition = new Vector2(0, -200);
        public static readonly Vector2 Size = new Vector2(480, 480 * 1.2f);
        public static readonly Vector2 Center = new Vector2(0, 0);
        public static readonly Vector2 TopLeftPoint = new Vector2(Center.X + -Size.X / 2f, Center.Y + Size.Y / 2);
        public static readonly Vector2 BottomRightPoint = new Vector2(Center.X + Size.X / 2, Center.Y + -Size.Y / 2);

        protected World()
        {
            gameObjects = new List<GameObject>();
        }

        public void InitPlayer()
        {
            if (Player != null) return;
            Player = new Player(DefaultPlayerPosition);
            this.Add(Player);
        }

        public void Add(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        public void Update(float elapsedTime)
        {
            CurrentPausetime += elapsedTime;
            if (Pattern != null && CurrentPausetime >= MaxPausetime)
                Pattern.Invoke();

            DoCollision();

            int i = 0;
            while (i < gameObjects.Count)
            {
                if (gameObjects[i].Terminated)
                    gameObjects.Remove(gameObjects[i]);
                else
                    gameObjects[i++].Update(elapsedTime);
            }
        }

        public void Render()
        {
            int i = 0;
            while (i < gameObjects.Count)
            {
                if (!gameObjects[i].Terminated)
                    gameObjects[i].Draw();
                i++;
            }
        }

        protected void DoCollision()
        {
            for (int i = 0; i < gameObjects.Count - 1; i++)
                for (int j = i + 1; j < gameObjects.Count; j++)
                    if (gameObjects[i].GetType() != gameObjects[j].GetType())
                    {
                        gameObjects[i].OnCollision(gameObjects[j]);
                        gameObjects[j].OnCollision(gameObjects[i]);
                    }
        }
    }
}
