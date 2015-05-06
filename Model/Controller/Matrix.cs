using System.Collections.Generic;
using Model.Agents;
using Model.Utils;

namespace Model.Controller
{
    public class Matrix
    {
        private Block[,] matrix;

        private List<Collector> collectors;
        private List<TrashCan> trashCans;
        private List<Recharger> rechargers;

        private int size;

        private int amountCollectors;
        private int amountTrashCans;
        private int amountRechargers;

        private Collector collector;

        public Matrix()
        {
            // Valores iniciais
            this.size = 15;
            this.amountCollectors = 1;
            this.amountTrashCans = 4;
            this.amountRechargers = 1;
            this.collectors = new List<Collector>();
            this.trashCans = new List<TrashCan>();
            this.rechargers = new List<Recharger>();
        }

        public Matrix(int amountCollectors, int amountTrashCans, int amountRechargers)
        {
            this.size = 15;
            this.amountCollectors = amountCollectors;
            this.amountTrashCans = amountTrashCans;
            this.amountRechargers = amountRechargers;
            this.collectors = new List<Collector>();
            this.trashCans = new List<TrashCan>();
            this.rechargers = new List<Recharger>();
        }

        public void createMatrix()
        {
            matrix = new Block[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (matrix[x, y] == null)
                    {
                        matrix[x, y] = new Block(x, y);
                    }
                }
            }
        }

        public void add(Position position, Agent agent)
        {
            if (agent == null)
                return;

            matrix[position.getX(), position.getY()].setAgent(agent);
        }

        public void add(Position position, Trash trash)
        {
            if (trash == null)
                return;

            matrix[position.getX(), position.getY()].setTrash(trash);
        }

        public void move(Collector collector, Block from, Block to)
        {
            matrix[from.getPosition().getX(), from.getPosition().getY()].setAgent(null);
            matrix[to.getPosition().getX(), to.getPosition().getY()].setAgent(collector);
            collector.setBlock(to);
        }

        /**
         * Get block of a specific position
         * @param pos
         * @return block
         */
        public Block getBlock(Position pos)
        {
            return matrix[pos.getX(), pos.getY()];
        }

        /**
         * Get available neighbors of a specific position.
         * @param pos
         * @return list of neighbors
         */
        public List<Block> getNeighbors(Position pos)
        {
            var neighbors = new List<Block>();

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
                    if ((relX < 0) || (relX >= size))
                    {
                        continue;
                    }

                    // out of bounds for row
                    if ((relY < 0) || (relY >= size))
                    {
                        continue;
                    }

                    // occupied
                    /*if  (matrix[relX][relY].hasAgent()) {
                        continue;
                    }*/

                    // if it made this far, add :)
                    neighbors.Add(matrix[relX, relY]);
                }
            }

            return neighbors;
        }

        public bool hasAgent(Position position)
        {
            return matrix[position.getX(), position.getY()].hasAgent();
        }

        public bool hasTrash(Position position)
        {
            return matrix[position.getX(), position.getY()].hasTrash();
        }

        /* GETTERS AND SETTERS */
        public Block[,] getMatrix()
        {
            return matrix;
        }

        public void setMatrix(Block[,] matrix)
        {
            this.matrix = matrix;
        }

        public int getSize()
        {
            return size;
        }

        public int getAmountCollectors()
        {
            return amountCollectors;
        }

        public void setAmountCollectors(int amountCollectors)
        {
            this.amountCollectors = amountCollectors;
        }

        public int getAmountTrashCans()
        {
            return amountTrashCans;
        }

        public void setAmountTrashCans(int amountTrashCans)
        {
            this.amountTrashCans = amountTrashCans;
        }

        public int getAmountRechargers()
        {
            return amountRechargers;
        }

        public void setAmountRechargers(int amountRechargers)
        {
            this.amountRechargers = amountRechargers;
        }

        public Collector getCollector()
        {
            return collector;
        }

        public void setCollector(Collector collector)
        {
            this.collector = collector;
        }
    }
}