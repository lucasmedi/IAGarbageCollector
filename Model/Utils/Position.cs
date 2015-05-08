using System;
using System.Collections.Generic;
using Model.Interfaces;

namespace Model.Utils
{
    public class Position : IPosition
    {
        private static Random rnd;

        private int x;
        private int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            int hashX = x.GetHashCode();
            int hashY = y.GetHashCode();

            return (hashX + hashY) * hashY + hashX;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position))
            {
                return false;
            }

            var otherPair = (Position)obj;

            return (
                (this.x == otherPair.x || this.x.Equals(otherPair.x))
                && (this.y == otherPair.y || this.y.Equals(otherPair.y))
            );
        }

        public static Position getRandomPosition(int seed)
        {
            rnd = rnd ?? new Random();
            int x = (int)rnd.Next(seed);
            int y = (int)rnd.Next(seed);

            return new Position(x, y);
        }

        public static double getDiagonalDistance(IPosition origin, IPosition destiny)
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

        public static int getHeuristic(IPosition origin, IPosition destiny, List<IPosition> future)
        {
            int xDiff = destiny.getX() - origin.getX();
            int yDiff = destiny.getY() - origin.getY();

            var possible = new Position(destiny.getX() + xDiff, destiny.getY() + yDiff);

            return (future.Contains(possible) ? 0 : 1);
        }

        public static IPosition getPseudoNearest(IPosition origin, List<IPosition> destinies)
        {
            IPosition res = null;
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

        public static IPosition getPseudoNearest(IPosition current, IPosition origin, List<IPosition> destinies, List<IPosition> future)
        {
            IPosition res = null;
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