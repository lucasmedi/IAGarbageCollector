using System;
using System.Collections.Generic;
using System.Diagnostics;
using Model.Agents;
using Model.Interfaces;
using Model.Utils;

namespace Model.Controller
{
    public class Creator
    {
        public World World { get; private set; }

        private List<Collector> collectors;
        private List<TrashCan> trashCans;
        private List<Recharger> rechargers;

        public Creator(IModel model)
        {
            this.World = new World(model);
        }

        public void Initialize()
        {
            collectors = new List<Collector>();
            trashCans = new List<TrashCan>();
            rechargers = new List<Recharger>();
        }

        public void New(IModel model)
        {
            Initialize();
            World = new World(model);

            CreateEnvironment();
            //TrashTypeGenerator.setIndex(0);
        }

        public void Reset()
        {
            Initialize();

            var model = World.Model;
            World = new World(model);

            CreateEnvironment();
        }

        public IList<IMove> NextAge()
        {
            var result = new List<IMove>();

            foreach (var c in collectors)
            {
                var move = new Move();

                move.Log += "\nColetor: " + c.getName() + " em " + c.getPosition() + " Status: " + c.getStatus();

                IBlock from = World.GetBlock(c.getPosition());
                IBlock to = c.run(World.GetNeighbors(c.getPosition()));

                if (to != null)
                {
                    World.Move(c, from, to);
                    move.Log += "\n* Moveu para " + to.getPosition();
                }

                move.Log +=  "\nStatus: " + c.ToString();

                move.From = from;
                move.To = to;

                result.Add(move);
            }

            return result;
        }

        private void CreateEnvironment()
        {
            Debug.WriteLine("Criou coletor em ");
            for (int i = 0; i < World.Model.amountCollectors; i++)
            {
                InsertCollector(i);
            }

            Debug.WriteLine("Criou latas de lixo em ");
            for (int i = 0; i < World.Model.amountTrashCans; i++)
            {
                InsertTrashCan(i);
            }

            Debug.WriteLine("Criou carregadores em ");
            for (int i = 0; i < World.Model.amountRechargers; i++)
            {
                InsertRechargers();
            }

            InsertTrash();
        }

        private void InsertCollector(int index)
        {
            var position = Position.getRandomPosition(World.GetSize());

            if (World.HasAgent(position))
            {
                InsertCollector(index);
                return;
            }

            var collector = new Collector("C" + index, World.Model.maxBatteryCapacity, World.Model.maxTrashCapacity, World.GetBlock(position));

            World.Place(position, collector);
            collectors.Add(collector);

            Debug.Write(position.ToString());
        }

        private void InsertTrashCan(int index)
        {
            var position = Position.getRandomPosition(World.GetSize());

            if (World.HasAgent(position))
            {
                InsertTrashCan(index);
                return;
            }

            var trashCanType = TrashTypeGenerator.next();

            var img = TrashTypeGenerator.getTrashCanIcon(trashCanType);
            var trashCan = new TrashCan("L" + index, img, World.Model.maxTrashCanCapacity, trashCanType);

            World.Place(position, trashCan);
            trashCans.Add(trashCan);

            foreach (var collector in collectors)
            {
                collector.addTrashCan(trashCan.getColor(), position.getX(), position.getY());
            }
        }

        private void InsertRechargers()
        {
            var position = Position.getRandomPosition(World.GetSize());

            if (World.HasAgent(position))
            {
                InsertRechargers();
                return;
            }

            var recharger = new Recharger();

            World.Place(position, recharger);
            rechargers.Add(recharger);

            foreach (var collector in collectors)
            {
                collector.addRecharger(position.getX(), position.getY());
            }
        }

        private void InsertTrash()
        {
            int freeBlocks = ((int)Math.Pow(World.GetSize(), 2)) - (World.Model.amountTrashCans + World.Model.amountRechargers + World.Model.amountCollectors);

            var rnd = new Random();
            int amountTrashes = (int)rnd.Next(freeBlocks) + (World.GetSize() / 2);

            if (amountTrashes == 0)
            {
                InsertTrash();
                return;
            }

            for (int i = 0; i < amountTrashes; i++)
            {
                var position = Position.getRandomPosition(World.GetSize());
                if (!World.HasAgent(position))
                {
                    var trashType = TrashTypeGenerator.getRandomTrashType();
                    var img = TrashTypeGenerator.getTrashIcon(trashType);
                    var trash = new Trash(trashType, img);
                    World.Place(position, trash);
                }
            }
        }
    }
}