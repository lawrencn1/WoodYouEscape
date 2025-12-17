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
        private EnemyType _type;
        private double _posX;
        private double _posY;
        public List<Obstacles> obstacles = new List<Obstacles>();
        public EnemyType Type
        {
            get
            {
                return this._type;
            }

            set
            {
                this._type = value;
            }
        }

        public double PosX
        {
            get
            {
                return this._posX;
            }

            set
            {
                this._posX = value;
            }
        }

        public double PosY
        {
            get
            {
                return this._posY;
            }

            set
            {
                this._posY = value;
            }
        }


        public EnemiesGenerator(EnemyType type, double posX, double posY, Canvas canva)
        {
            this.Type = type;
            this._posX = posX;
            this._posY = posY;

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
