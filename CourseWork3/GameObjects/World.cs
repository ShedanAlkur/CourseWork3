using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

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

        static readonly Vector2 DefaultPlayerPosition = new Vector2(0, -200);

        public Player Player { get; protected set; }
        public LevelPattern Pattern;
        public int CurrentIndex;
        public float CurrentPausetime;
        public float MaxPausetime;

        protected World()
        {
            gameObjects = new List<GameObject>();
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
            if (CurrentPausetime >= MaxPausetime)
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
                    gameObjects[i++].Draw();
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
