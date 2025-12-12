using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace shooter
{
    public class EnemiesGenerator
    {
        private EnemyType type;
        private double posX;
        private double posY;
        public List<Obstacles> obstacles = new List<Obstacles>();
        public EnemyType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        public double PosX
        {
            get
            {
                return this.posX;
            }

            set
            {
                this.posX = value;
            }
        }

        public double PosY
        {
            get
            {
                return this.posY;
            }

            set
            {
                this.posY = value;
            }
        }


        public EnemiesGenerator(EnemyType type, double posX, double posY, Canvas canva)
        {
            this.Type = type;
            this.posX = posX;
            this.posY = posY;

        
            SpawnEnemies(canva, posX, posY, type);
        }

        public void SpawnEnemies(Canvas canvas, double X, double Y, EnemyType type)
        {
            Enemy enemy = new Enemy(X, Y, type, 40, 40);
            canvas.Children.Add(enemy.Sprite);
            enemy.UpdatePosition();
        }
    }
}
