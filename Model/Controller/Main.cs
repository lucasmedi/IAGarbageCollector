using System;
using System.Collections.Generic;
using System.Diagnostics;
using Model.Agents;
using Model.Utils;

namespace Model.Controller
{
    public class Main
    {
        private int amountCollectors;
        private int amountTrashCans;
        private int amountRechargers;

        private int maxTrashCanCapacity;
        private int maxTrashCapacity;
        private int maxBatteryCapacity;

        private Matrix matrix;
        private List<Collector> collectors;
        private List<TrashCan> trashCans;
        private List<Recharger> rechargers;

        public void init()
        {
            Debug.WriteLine(" Reloaded! ");
            amountCollectors = 1;
            amountTrashCans = 4;
            amountRechargers = 1;
            maxTrashCanCapacity = 5;
            maxTrashCapacity = 3;
            maxBatteryCapacity = 10;
            initArrayList();
        }

        public void createMatrix()
        {
            initArrayList();
            matrix = new Matrix(amountCollectors, amountTrashCans, amountRechargers);
            matrix.createMatrix();

            createEnvironment();
            TrashTypeGenerator.setIndex(0);

            //RequestContext.getCurrentInstance().update("agents");
        }

        public void createMatrixTestMoving()
        {
            amountCollectors = 1;
            amountTrashCans = 0;
            amountRechargers = 0;
            maxTrashCanCapacity = 5;
            maxTrashCapacity = 3;
            maxBatteryCapacity = 10;
            initArrayList();
            matrix = new Matrix(amountCollectors, amountTrashCans, amountRechargers);
            matrix.createMatrix();

            createEnvironment();
            //RequestContext.getCurrentInstance().update("agents");
        }

        public void next()
        {
            foreach (var c in collectors)
            {
                Debug.WriteLine("\nColetor: " + c.getName() + " em " + c.getPosition() + " Status: " + c.getStatus());

                Block from = matrix.getBlock(c.getPosition());
                Block to = c.run(matrix.getNeighbors(c.getPosition()));

                if (to != null)
                {
                    matrix.move(c, from, to);
                    Debug.WriteLine("* Moveu para " + to.getPosition());
                }

                Debug.WriteLine("Status: " + c.ToString());
                Debug.WriteLine(string.Empty);
            }

            //RequestContext.getCurrentInstance().update("agents");
        }

        private void createEnvironment()
        {
            Debug.WriteLine("Criou coletor em ");
            for (int i = 0; i < amountCollectors; i++)
            {
                insertCollector(i);
            }

            Debug.WriteLine("Criou latas de lixo em ");
            for (int i = 0; i < amountTrashCans; i++)
            {
                insertTrashCan(i);
            }

            Debug.WriteLine("Criou carregadores em ");
            for (int i = 0; i < amountRechargers; i++)
            {
                insertRechargers();
            }

            insertTrash();
        }

        private void insertCollector(int index)
        {
            var position = Position.getRandomPosition(matrix.getSize());

            if (matrix.hasAgent(position))
            {
                insertCollector(index);
                return;
            }

            var collector = new Collector("C" + index, maxBatteryCapacity, maxTrashCapacity, matrix.getBlock(position));

            matrix.add(position, collector);
            collectors.Add(collector);

            Debug.Write(position.ToString());
        }

        private void insertTrashCan(int index)
        {
            var position = Position.getRandomPosition(matrix.getSize());

            if (matrix.hasAgent(position))
            {
                insertTrashCan(index);
                return;
            }

            var trashCanType = TrashTypeGenerator.next();

            var img = TrashTypeGenerator.getTrashCanIcon(trashCanType);
            var trashCan = new TrashCan("L" + index, img, maxTrashCanCapacity, trashCanType);

            matrix.add(position, trashCan);
            trashCans.Add(trashCan);

            foreach (var collector in collectors)
            {
                collector.addTrashCan(trashCan.getColor(), position.getX(), position.getY());
            }
        }

        private void insertRechargers()
        {
            var position = Position.getRandomPosition(matrix.getSize());

            if (matrix.hasAgent(position))
            {
                insertRechargers();
                return;
            }

            var recharger = new Recharger();

            matrix.add(position, recharger);
            rechargers.Add(recharger);

            foreach (var collector in collectors)
            {
                collector.addRecharger(position.getX(), position.getY());
            }
        }

        private void insertTrash()
        {
            int freeBlocks = (matrix.getSize() ^ 2) - (amountTrashCans + amountRechargers + amountCollectors);

            var rnd = new Random(freeBlocks);
            int amountTrashes = (int)(rnd.Next() * freeBlocks) + (matrix.getSize() / 2);

            if (amountTrashes == 0)
            {
                insertTrash();
                return;
            }

            for (int i = 0; i < amountTrashes; i++)
            {
                var position = Position.getRandomPosition(matrix.getSize());
                if (!matrix.hasAgent(position))
                {
                    var trashType = TrashTypeGenerator.getRandomTrashType();
                    var img = TrashTypeGenerator.getTrashIcon(trashType);
                    var trash = new Trash(trashType, img);
                    matrix.add(position, trash);
                }
            }
        }

        private void initArrayList()
        {
            collectors = new List<Collector>();
            trashCans = new List<TrashCan>();
            rechargers = new List<Recharger>();
        }

        /* GETTERS AND SETTERS */
        public Matrix getMatrix()
        {
            return matrix;
        }

        public void setMatrix(Matrix matrix)
        {
            this.matrix = matrix;
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

        public int getMaxTrashCanCapacity()
        {
            return maxTrashCanCapacity;
        }

        public void setMaxTrashCanCapacity(int maxTrashCanCapacity)
        {
            this.maxTrashCanCapacity = maxTrashCanCapacity;
        }

        public int getMaxTrashCapacity()
        {
            return maxTrashCapacity;
        }

        public void setMaxTrashCapacity(int maxTrashCapacity)
        {
            this.maxTrashCapacity = maxTrashCapacity;
        }

        public int getMaxBatteryCapacity()
        {
            return maxBatteryCapacity;
        }

        public void setMaxBatteryCapacity(int maxBatteryCapacity)
        {
            this.maxBatteryCapacity = maxBatteryCapacity;
        }
    }
}