using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class World : IUpdateable
    {
        List<GameObject> gameObjects;

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

        protected World()
        {
            gameObjects = new List<GameObject>();
        }

        public Player Player { get; protected set; }
        LevelPattern pattern;
        public int CurrentIndex;
        public float CurrentDelaytime;
        public float MaxDelaytime;

        public void Update(float elapsedTime)
        {
            CurrentDelaytime += elapsedTime;
            if (CurrentDelaytime >= MaxDelaytime)
                pattern.Invoke(ref CurrentIndex);

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
