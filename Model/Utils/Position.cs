using System;
using System.Collections.Generic;

namespace Model.Utils
{
    public class Position
    {
        private int x;
        private int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int hashCode()
        {
            int hashX = (x != null ? x.GetHashCode() : 0);
            int hashY = (y != null ? y.GetHashCode() : 0);

            return (hashX + hashY) * hashY + hashX;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position))
            {
                return false;
            }

            var otherPair = (Position)obj;

            return ((this.x == otherPair.x || (this.x != null && otherPair.x != null && this.x.Equals(otherPair.x)))
                && (this.y == otherPair.y || (this.y != null && otherPair.y != null && this.y.Equals(otherPair.y))));
        }

        public static Position getRandomPosition(int seed)
        {
            var rnd = new Random(seed);
            int x = (int)(rnd.Next() * seed);
            int y = (int)(rnd.Next() * seed);

            return new Position(x, y);
        }

        public static double getDiagonalDistance(Position origin, Position destiny)
        {
            double res = 0;

            if (origin == null || destiny == null)
            {
                return res;
            }

            // Euclidean distance
            //return Math.sqrt(Math.pow((destiny.getX() - origin.getX()), 2) + Math.pow((destiny.getY() - origin.getY()), 2));

            // Diagonal distance
            return Math.Max(Math.Abs(origin.getX() - destiny.getX()), Math.Abs(origin.getY() - destiny.getY()));
        }

        public static int getHeuristic(Position origin, Position destiny, List<Position> future)
        {
            int xDiff = destiny.getX() - origin.getX();
            int yDiff = destiny.getY() - origin.getY();

            var possible = new Position(destiny.getX() + xDiff, destiny.getY() + yDiff);

            return (future.Contains(possible) ? 0 : 1);
        }

        public static Position getPseudoNearest(Position origin, List<Position> destinies)
        {
            Position res = null;
            var distance = Double.MaxValue;

            foreach (var position in destinies)
            {
                double p = getDiagonalDistance(origin, position);
                if (p < distance)
                {
                    distance = p;
                    res = position;
                }
            }

            return res;
        }

        public static Position getPseudoNearest(Position current, Position origin, List<Position> destinies, List<Position> future)
        {
            Position res = null;
            var distance = Double.MaxValue;

            double f = 0;
            double g = 0;
            double h = 0;

            foreach (var position in destinies)
            {
                g = getDiagonalDistance(origin, position);
                h = getHeuristic(current, position, future);
                f = g + h;
                if (f < distance)
                {
                    distance = f;
                    res = position;
                }
            }

            return res;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }

        public void setX(int x)
        {
            this.x = x;
        }

        public void setY(int y)
        {
            this.y = y;
        }
    }
}