using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2P
{
    public class ScoreManager
    {
        private int _score;

        public ScoreManager()
        {
            _score = 0;
        }

        public void AddPoints(int points)
        {
            _score += points;
        }

        public int GetScore()
        {
            return _score;
        }

        public void Reset()
        {
            _score = 0;
        }
    }

}
