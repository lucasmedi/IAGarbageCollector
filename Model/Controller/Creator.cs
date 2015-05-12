using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        private StringBuilder log;

        public Creator(IModel model)
        {
            this.World = new World(model);
            this.log = new StringBuilder();
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

        public void NextAge()
        {
            log = new StringBuilder();

            foreach (var c in collectors)
            {
                log.AppendLine("\nColetor: " + c.getName() + " em " + c.getPosition() + " Status: " + c.getStatus());

                IBlock from = World.GetBlock(c.getPosition());
                IBlock to = c.run(World.GetNeighbors(c.getPosition()));

                if (to != null)
                {
                    World.Move(c, from, to);
                    log.AppendLine("* Moveu para " + to.getPosition());
                }

                log.AppendFormat(c.GetLog());

                log.AppendLine("Status: " + c.ToString());
                log.AppendLine(string.Empty);
            }
        }

        public string GetLog()
        {
            return log.ToString();
        }

        private void CreateEnvironment()
        {
            log.AppendLine("Collector placed in:");
            for (int i = 0; i < World.Model.amountCollectors; i++)
            {
                InsertCollector(i);
            }

            log.AppendLine("Trash cans placed in:");
            for (int i = 0; i < World.Model.amountTrashCans; i++)
            {
                InsertTrashCan(i);
            }

            log.AppendLine("Rechargers placed in:");
            for (int i = 0; i < World.Model.amountRechargers; i++)
            {
                InsertRechargers();
            }

            log.AppendLine("Trash placed randonly.");
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

            log.AppendLine("> " + collector.getName() + ": " + position.ToString());
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

            log.AppendLine("> " + trashCan.getName() + ": " + position.ToString());

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

            var recharger = new Recharger("R0");

            World.Place(position, recharger);
            rechargers.Add(recharger);

            log.AppendLine("> " + recharger.getName() + ": " + position.ToString());

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