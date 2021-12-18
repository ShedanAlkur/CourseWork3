using CourseWork3.Game;
using OpenTK;
using System.Drawing;

namespace CourseWork3.GameObjects
{
    class ItemBomb : Item
    {
        public ItemBomb(Vector2 position) : base(position)
        {
        }

        protected override Color Color => Color.Green;

        protected override void Use()
        {
            base.Use();
            GameMain.Stats.BombCount++;
        }
    }
}
