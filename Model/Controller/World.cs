using System.Collections.Generic;
using Model.Agents;
using Model.Interfaces;

namespace Model.Controller
{
    public class World
    {
        public IBlock[,] Matrix { get; private set; }
        public IModel Model { get; private set; }

        private List<Collector> collectors;
        private List<TrashCan> trashCans;
        private List<Recharger> rechargers;

        public World(IModel param)
        {
            this.Model = param;

            Initialize();
            CreateMatrix();
        }

        private void Initialize()
        {
            this.collectors = new List<Collector>();
            this.trashCans = new List<TrashCan>();
            this.rechargers = new List<Recharger>();
        }

        private void CreateMatrix()
        {
            Matrix = new Block[Model.size, Model.size];
            for (int x = 0; x < Model.size; x++)
            {
                for (int y = 0; y < Model.size; y++)
                {
                    if (Matrix[x, y] == null)
                    {
                        Matrix[x, y] = new Block(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Place Agent into the given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="agent"></param>
        public void Place(IPosition position, IAgent agent)
        {
            if (agent == null)
                return;

            Matrix[position.getX(), position.getY()].setAgent(agent);
        }

        /// <summary>
        /// Place Trash into the given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="trash"></param>
        public void Place(IPosition position, ITrash trash)
        {
            if (trash == null)
                return;

            Matrix[position.getX(), position.getY()].setTrash(trash);
        }

        /// <summary>
        /// Move collector from one block to another
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Move(Collector collector, IBlock from, IBlock to)
        {
            Matrix[from.getPosition().getX(), from.getPosition().getY()].setAgent(null);
            Matrix[to.getPosition().getX(), to.getPosition().getY()].setAgent(collector);
            collector.setBlock(to);
        }

        /// <summary>
        /// Get block of a specific position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public IBlock GetBlock(IPosition pos)
        {
            return Matrix[pos.getX(), pos.getY()];
        }

        /// <summary>
        /// Get available neighbors of a specific position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public List<IBlock> GetNeighbors(IPosition pos)
        {
            var neighbors = new List<IBlock>();

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    // same position
                    if ((x == 0) && (y == 0))
                    {
                        continue;
                    }

                    int relX = (pos.getX() + x);
                    int relY = (pos.getY() + y);

                    // out of bounds for column
                    if ((relX < 0) || (relX >= Model.size))
                    {
                        continue;
                    }

                    // out of bounds for row
                    if ((relY < 0) || (relY >= Model.size))
                    {
                        continue;
                    }

                    // occupied
                    /*if  (matrix[relX][relY].hasAgent()) {
                        continue;
                    }*/

                    // if it made this far, add :)
                    neighbors.Add(Matrix[relX, relY]);
                }
            }

            return neighbors;
        }

        public bool HasAgent(IPosition position)
        {
            return Matrix[position.getX(), position.getY()].hasAgent();
        }

        public bool HasTrash(IPosition position)
        {
            return Matrix[position.getX(), position.getY()].hasTrash();
        }

        public int GetSize()
        {
            return Model.size;
        }
    }
}