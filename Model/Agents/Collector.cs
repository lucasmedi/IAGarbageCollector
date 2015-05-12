using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Model.Interfaces;
using Model.Utils;

namespace Model.Agents
{
    public class Collector : Agent
    {
        private static string icon = "Content/Images/collector.png";

        /* KNOWLEDGE About THE WORLD */
        private List<IPosition> glassTrashCans;
        private List<IPosition> metalTrashCans;
        private List<IPosition> paperTrashCans;
        private List<IPosition> plasticTrashCans;
        private List<IPosition> rechargers;

        private List<IBlock> neighbors;

        /* KNOWLEDGE About ITSELF */
        private List<ITrash> glassTrash;
        private List<ITrash> metalTrash;
        private List<ITrash> paperTrash;
        private List<ITrash> plasticTrash;

        private IBlock currentBlock;

        private int maxBatteryCapacity;
        private int maxTrashCapacity;

        private int batteryCharge;

        private CollectorStatus status;

        private Way way;
        private Direction direction;

        /* CONTROL VARIABLES */
        private IPosition objective;
        private TrashType trashType;
        private List<IBlock> possibleBlocks;
        private List<IBlock> excludedBlocks;

        private StringBuilder log;

        /* CONSTRUCTOR */
        public Collector(string name, int batteryCapacity, int trashCapacity, IBlock current)
            : base(name, Collector.icon)
        {
            // current position
            this.currentBlock = current;

            // rechargers
            rechargers = new List<IPosition>();

            // trash cans
            glassTrashCans = new List<IPosition>();
            metalTrashCans = new List<IPosition>();
            paperTrashCans = new List<IPosition>();
            plasticTrashCans = new List<IPosition>();

            // garbage
            glassTrash = new List<ITrash>();
            metalTrash = new List<ITrash>();
            paperTrash = new List<ITrash>();
            plasticTrash = new List<ITrash>();

            // inner status
            maxBatteryCapacity = batteryCapacity;
            maxTrashCapacity = trashCapacity;
            batteryCharge = batteryCapacity;

            // wandering config
            status = CollectorStatus.WANDER;
            direction = Direction.NONE;
            way = Way.DOWNRIGHT;
        }

        /* CONFIGURATION METHODS */
        public bool addTrashCan(TrashType trashType, int x, int y)
        {
            var pos = new Position(x, y);

            switch (trashType)
            {
                case TrashType.GLASS:
                    if (glassTrashCans.Contains(pos))
                    {
                        return false;
                    }

                    glassTrashCans.Add(pos);
                    break;
                case TrashType.METAL:
                    if (metalTrashCans.Contains(pos))
                    {
                        return false;
                    }

                    metalTrashCans.Add(pos);
                    break;
                case TrashType.PAPER:
                    if (paperTrashCans.Contains(pos))
                    {
                        return false;
                    }

                    paperTrashCans.Add(pos);
                    break;
                case TrashType.PLASTIC:
                    if (plasticTrashCans.Contains(pos))
                    {
                        return false;
                    }

                    plasticTrashCans.Add(pos);
                    break;
                default:
                    break;
            }

            return true;
        }

        public bool addRecharger(int x, int y)
        {
            var pos = new Position(x, y);
            if (rechargers.Contains(pos))
            {
                return false;
            }

            rechargers.Add(pos);
            return true;
        }

        /* PUBLIC METHODS */
        /**
         * Execute action
         * @param neighbors
         */
        public IBlock run(List<IBlock> neighbors)
        {
            log = new StringBuilder();

            if ((neighbors == null) || (neighbors.Count == 0))
            {
                log.AppendLine("Sem espaço para andar. :(");
                return null;
            }

            if (batteryCharge <= 0)
            {
                log.AppendLine("Acabou a bateria. :(");
                return null;
            }

            this.neighbors = neighbors;
            this.possibleBlocks = getPossibleBlocks();

            observe();

            plan();

            IBlock moveTo = act();

            if (moveTo != null)
            {
                batteryCharge--;
            }

            return moveTo;
        }

        /**
         * Observe its state and prepare to plan the next move.
         */
        private void observe()
        {
            if (status != CollectorStatus.LOOKINGRECHARGER
                && status != CollectorStatus.WALKINGRECHARGER
                && status != CollectorStatus.RECHARGING
                && batteryLow())
            {
                status = CollectorStatus.LOOKINGRECHARGER;
                log.AppendLine("* Precisa carregar, procurar por carregador");
                return;
            }

            if (status == CollectorStatus.WALKINGRECHARGER || status == CollectorStatus.RECHARGING)
            {
                return;
            }

            if (hasFullTrash())
            {
                status = CollectorStatus.LOOKINGTRASHCAN;
                log.AppendLine("* Precisa esvaziar, procurar por lixeira");
                return;
            }

            if (status == CollectorStatus.WALKINGTRASHCAN || status == CollectorStatus.EMPTYING)
            {
                return;
            }

            if (status != CollectorStatus.WALKINGTRASH && hasTrash())
            {
                status = CollectorStatus.LOOKINGTRASH;
                return;
            }
        }

        /**
         * Plans the next move.
         */
        private void plan()
        {
            switch (status)
            {
                case CollectorStatus.LOOKINGRECHARGER:
                    objective = Position.getPseudoNearest(currentBlock.getPosition(), rechargers);
                    status = CollectorStatus.WALKINGRECHARGER;
                    log.AppendLine("* Definiu ir para o carregador em " + objective.ToString());
                    break;
                case CollectorStatus.WALKINGRECHARGER:
                    log.AppendLine("* Indo para o carregador em " + objective.ToString());
                    break;
                case CollectorStatus.LOOKINGTRASHCAN:
                    objective = getNearestTrashCan();
                    status = CollectorStatus.WALKINGTRASHCAN;
                    log.AppendLine("* Definiu ir para lixeira em " + objective.ToString());
                    break;
                case CollectorStatus.WALKINGTRASHCAN:
                    log.AppendLine("* Indo para a lixeira em " + objective.ToString());
                    break;
                case CollectorStatus.LOOKINGTRASH:
                    objective = Position.getPseudoNearest(currentBlock.getPosition(), getTrashFound());
                    status = CollectorStatus.WALKINGTRASH;
                    log.AppendLine("* Achou lixo em " + objective.ToString());
                    break;
                case CollectorStatus.WALKINGTRASH:
                    log.AppendLine("* Indo para o lixo em " + objective.ToString());
                    break;
                case CollectorStatus.RECHARGING:
                    log.AppendLine("* Recarregando...");
                    break;
                case CollectorStatus.EMPTYING:
                    log.AppendLine("* Esvaziando...");
                    break;
                case CollectorStatus.WANDER:
                default:
                    objective = wander().getPosition();
                    status = CollectorStatus.WANDER;
                    log.AppendLine("* Nada encontrado, então caminhar");
                    break;
            }
        }

        /**
         * Act according to what was planned
         */
        private IBlock act()
        {
            if (currentBlock.getPosition().Equals(objective))
            {
                if (status == CollectorStatus.WALKINGTRASH)
                {
                    collectTrash();
                    return null;
                }
            }

            if (status == CollectorStatus.WALKINGRECHARGER || status == CollectorStatus.RECHARGING)
            {
                IBlock aux = null;
                foreach (var block in excludedBlocks)
                {
                    if (block.getPosition().Equals(objective))
                    {
                        aux = block;
                        continue;
                    }
                }

                if (aux != null)
                {
                    recharge(aux);
                    return null;
                }
            }

            if (status == CollectorStatus.WALKINGTRASHCAN || status == CollectorStatus.EMPTYING)
            {
                IBlock aux = null;
                foreach (var block in excludedBlocks)
                {
                    if (block.getPosition().Equals(objective))
                    {
                        aux = block;
                        continue;
                    }
                }

                if (aux != null)
                {
                    emptying(aux);
                    return null;
                }
            }

            var positions = new List<IPosition>();
            foreach (var block in possibleBlocks)
            {
                if (block.getPosition().Equals(objective))
                {
                    return block;
                }

                positions.Add(block.getPosition());
            }

            var future = new List<IPosition>();
            foreach (var block in neighbors)
            {
                if (!possibleBlocks.Contains(block) && !block.hasAgent())
                {
                    future.Add(block.getPosition());
                }
            }

            var pos = Position.getPseudoNearest(currentBlock.getPosition(), objective, positions, future);
            foreach (var block in possibleBlocks)
            {
                if (block.getPosition().Equals(pos))
                {
                    return block;
                }
            }

            return null;
        }

        // REFAZER A LOGICA DE TIRAR O LIXO DO COLETOR PARA LIXEIRA
        private void emptying(IBlock block)
        {
            if (!(block != null && block.hasAgent() && block.getAgent() is TrashCan))
            {
                return;
            }

            var trashCan = (TrashCan)block.getAgent();

            if (trashCan.isFull())
            {
                log.AppendLine(trashCan.getName() + " está cheia!");
                return;
            }

            switch (trashType)
            {
                case TrashType.GLASS:
                    if (!glassTrash.Any())
                    {
                        status = CollectorStatus.WANDER;
                        return;
                    }

                    var removedGlassTrash = glassTrash[0];
                    glassTrash.RemoveAt(0);

                    trashCan.addTrash(removedGlassTrash);
                    break;
                case TrashType.METAL:
                    if (!metalTrash.Any())
                    {
                        status = CollectorStatus.WANDER;
                        return;
                    }

                    var removedMetalTrash = metalTrash[0];
                    metalTrash.RemoveAt(0);

                    trashCan.addTrash(removedMetalTrash);
                    break;
                case TrashType.PAPER:
                    if (!paperTrash.Any())
                    {
                        status = CollectorStatus.WANDER;
                        return;
                    }

                    var removedPaperTrash = paperTrash[0];
                    paperTrash.RemoveAt(0);

                    trashCan.addTrash(removedPaperTrash);
                    break;
                case TrashType.PLASTIC:
                    if (!plasticTrash.Any())
                    {
                        status = CollectorStatus.WANDER;
                        return;
                    }

                    var removedPlasticTrash = plasticTrash[0];
                    plasticTrash.RemoveAt(0);

                    trashCan.addTrash(removedPlasticTrash);
                    break;
                default:
                    // Oops. :(
                    break;
            }

            status = CollectorStatus.EMPTYING;
        }

        private void recharge(IBlock block)
        {
            if (!(block != null && block.hasAgent() && block.getAgent() is Recharger))
            {
                return;
            }

            var recharger = (Recharger)block.getAgent();

            if (status == CollectorStatus.WALKINGRECHARGER)
            {
                if (recharger.isBusy())
                {
                    log.AppendLine("* O carregador está ocupado :(.");
                    return;
                }
                else
                {
                    recharger.addCollector(this);
                    status = CollectorStatus.RECHARGING;
                    return;
                }
            }

            if (maxBatteryCapacity > batteryCharge)
            {
                batteryCharge++;
                log.AppendLine("* Carregando: bateria: " + batteryCharge + " >> " + maxBatteryCapacity);
            }
            else
            {
                recharger.removeCollector(this);
                status = CollectorStatus.WANDER;
                log.AppendLine("* Carregou jóia. :)");
            }
        }

        private void collectTrash()
        {
            var trash = currentBlock.collectTrash();
            switch (trash.getTrashType())
            {
                case TrashType.GLASS:
                    glassTrash.Add(trash);
                    break;
                case TrashType.METAL:
                    metalTrash.Add(trash);
                    break;
                case TrashType.PAPER:
                    paperTrash.Add(trash);
                    break;
                case TrashType.PLASTIC:
                    plasticTrash.Add(trash);
                    break;
                default:
                    break;
            }

            log.AppendLine("* Pegou lixo " + trash.ToString());

            status = CollectorStatus.WANDER;
        }

        private IBlock wander()
        {
            if (possibleBlocks.Count == 0)
            {
                return null;
            }
            else if (way == Way.DOWNRIGHT && this.isDownRightEnd())
            {
                way = Way.UPLEFT;
                direction = Direction.LEFT;
                log.AppendLine("* inverteu o caminho para UPLEFT");
            }
            else if (way == Way.UPLEFT && this.isUpLeftEnd())
            {
                way = Way.DOWNRIGHT;
                direction = Direction.RIGHT;
                log.AppendLine("* inverteu o caminho para DOWNRIGHT");
            }

            if (way == Way.DOWNRIGHT)
            {
                return WanderDownRight();
            }
            else if (way == Way.UPLEFT)
            {
                return WanderUpLeft();
            }

            return null;
        }

        private IBlock WanderDownRight()
        {
            IBlock block = null;

            if (direction == Direction.NONE || direction == Direction.RIGHT)
            {
                block = goRight();
                if (block != null) return block;

                block = goDown();
                if (block != null) return block;

                block = goLeft();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goUp();
                if (block != null) return block;
            }
            else if (direction == Direction.DOWN)
            {
                block = goLeft();
                if (block != null) return block;

                block = goRight();
                if (block != null) return block;

                block = goDown();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goUp();
                if (block != null) return block;
            }
            else if (direction == Direction.LEFT)
            {
                block = goLeft();
                if (block != null) return block;

                block = goDown();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goUp();
                if (block != null) return block;
            }
            else if (direction == Direction.UP)
            {
                block = goRight();
                if (block != null) return block;

                block = goLeft();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goUp();
                if (block != null) return block;
            }

            return null;
        }

        private IBlock WanderUpLeft()
        {
            IBlock block = null;

            if (direction == Direction.NONE || direction == Direction.LEFT)
            {
                block = goLeft();
                if (block != null) return block;

                block = goUp();
                if (block != null) return block;

                block = goRight();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goDown();
                if (block != null) return block;
            }
            else if (direction == Direction.UP)
            {
                block = goRight();
                if (block != null) return block;

                block = goLeft();
                if (block != null) return block;

                block = goUp();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goDown();
                if (block != null) return block;
            }
            else if (direction == Direction.RIGHT)
            {
                block = goRight();
                if (block != null) return block;

                block = goUp();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goDown();
                if (block != null) return block;
            }
            else if (direction == Direction.DOWN)
            {
                block = goLeft();
                if (block != null) return block;

                block = goRight();
                if (block != null) return block;

                block = goDiagonal();
                if (block != null) return block;

                block = goDown();
                if (block != null) return block;
            }

            return null;
        }

        private bool isDownRightEnd()
        {
            int higher = 0;

            foreach (var block in possibleBlocks)
            {
                int xCurrent = currentBlock.getPosition().getX();
                int yCurrent = currentBlock.getPosition().getY();
                int xBlock = block.getPosition().getX();
                int yBlock = block.getPosition().getY();

                if (xCurrent < xBlock || yCurrent < yBlock)
                {
                    higher++;
                }
            }

            return (higher <= 1);
        }

        private bool isUpLeftEnd()
        {
            int lower = 0;

            foreach (var block in neighbors)
            {
                int xCurrent = currentBlock.getPosition().getX();
                int yCurrent = currentBlock.getPosition().getY();
                int xBlock = block.getPosition().getX();
                int yBlock = block.getPosition().getY();

                if (xCurrent > xBlock || yCurrent > yBlock)
                {
                    lower++;
                }
            }

            return (lower <= 1);
        }

        private IBlock goRight()
        {
            foreach (var possibleBlock in possibleBlocks)
            {
                if (possibleBlock.getPosition().getX() == currentBlock.getPosition().getX()
                    && possibleBlock.getPosition().getY() > currentBlock.getPosition().getY())
                {
                    direction = Direction.RIGHT;
                    return possibleBlock;
                }
            }

            return null;
        }

        private IBlock goDown()
        {
            foreach (var possibleBlock in possibleBlocks)
            {
                if (possibleBlock.getPosition().getX() > currentBlock.getPosition().getX()
                    && possibleBlock.getPosition().getY() == currentBlock.getPosition().getY())
                {
                    direction = Direction.DOWN;
                    return possibleBlock;
                }
            }

            return null;
        }

        private IBlock goLeft()
        {
            foreach (var possibleBlock in possibleBlocks)
            {
                if (possibleBlock.getPosition().getX() == currentBlock.getPosition().getX()
                    && possibleBlock.getPosition().getY() < currentBlock.getPosition().getY())
                {
                    direction = Direction.LEFT;
                    return possibleBlock;
                }
            }

            return null;
        }

        private IBlock goUp()
        {
            foreach (var possibleBlock in possibleBlocks)
            {
                if (possibleBlock.getPosition().getX() < currentBlock.getPosition().getX()
                    && possibleBlock.getPosition().getY() == currentBlock.getPosition().getY())
                {
                    direction = Direction.UP;
                    return possibleBlock;
                }
            }

            return null;
        }

        private IBlock goDiagonal()
        {
            foreach (var possibleBlock in possibleBlocks)
            {
                if (way == Way.DOWNRIGHT)
                {
                    if ((possibleBlock.getPosition().getX() > currentBlock.getPosition().getX()
                            && possibleBlock.getPosition().getY() > currentBlock.getPosition().getY())
                        || (possibleBlock.getPosition().getX() > currentBlock.getPosition().getX()
                            && possibleBlock.getPosition().getY() < currentBlock.getPosition().getY()))
                    {
                        direction = Direction.DOWN;
                        return possibleBlock;
                    }
                }
                else if (way == Way.UPLEFT)
                {
                    if ((possibleBlock.getPosition().getX() < currentBlock.getPosition().getX()
                            && possibleBlock.getPosition().getY() < currentBlock.getPosition().getY())
                        || (possibleBlock.getPosition().getX() < currentBlock.getPosition().getX()
                            && possibleBlock.getPosition().getY() > currentBlock.getPosition().getY()))
                    {
                        direction = Direction.UP;
                        return possibleBlock;
                    }
                }
            }

            return null;
        }

        private List<IBlock> getPossibleBlocks()
        {
            this.possibleBlocks = new List<IBlock>();
            this.excludedBlocks = new List<IBlock>();

            foreach (var block in neighbors)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if ((x == 0) && (y == 0))
                        {
                            continue;
                        }

                        int relX = (currentBlock.getPosition().getX() + x);
                        int relY = (currentBlock.getPosition().getY() + y);

                        if (block.getPosition().getX() == relX
                            && block.getPosition().getY() == relY)
                        {
                            if (!block.hasAgent())
                            {
                                possibleBlocks.Add(block);
                            }
                            else
                            {
                                excludedBlocks.Add(block);
                            }
                        }
                    }
                }
            }

            return possibleBlocks;
        }

        private bool batteryLow()
        {
            double res = double.MaxValue;
            foreach (var position in rechargers)
            {
                double p = Position.getDiagonalDistance(currentBlock.getPosition(), position);
                if (res > p)
                {
                    res = p;
                }
            }

            return batteryCharge <= ((int)res) + 5;
        }

        private bool hasFullTrash()
        {
            if (glassTrash.Count == maxTrashCapacity)
            {
                trashType = TrashType.GLASS;
                return true;
            }

            if (metalTrash.Count == maxTrashCapacity)
            {
                trashType = TrashType.METAL;
                return true;
            }

            if (paperTrash.Count == maxTrashCapacity)
            {
                trashType = TrashType.PAPER;
                return true;
            }

            if (plasticTrash.Count == maxTrashCapacity)
            {
                trashType = TrashType.PLASTIC;
                return true;
            }

            return false;
        }

        private bool hasTrash()
        {
            foreach (var block in neighbors)
            {
                if (block.hasTrash())
                {
                    return true;
                }
            }

            return false;
        }

        private List<IPosition> getTrashFound()
        {
            var trashFound = new List<IPosition>();

            foreach (var block in neighbors)
            {
                if (block.hasTrash())
                {
                    trashFound.Add(block.getPosition());
                }
            }

            return trashFound;
        }

        private IPosition getNearestTrashCan()
        {
            switch (trashType)
            {
                case TrashType.GLASS:
                    return Position.getPseudoNearest(currentBlock.getPosition(), glassTrashCans);
                case TrashType.METAL:
                    return Position.getPseudoNearest(currentBlock.getPosition(), metalTrashCans);
                case TrashType.PAPER:
                    return Position.getPseudoNearest(currentBlock.getPosition(), paperTrashCans);
                case TrashType.PLASTIC:
                    return Position.getPseudoNearest(currentBlock.getPosition(), plasticTrashCans);
                default:
                    // Oops. :(
                    break;
            }

            return null;
        }

        public IPosition getPosition()
        {
            return currentBlock.getPosition();
        }

        public void setBlock(IBlock block)
        {
            this.currentBlock = block;
        }

        public CollectorStatus getStatus()
        {
            return status;
        }

        public string GetLog()
        {
            return log.ToString();
        }

        public override string ToString()
        {
            return "\n"
                + "Bateria: " + this.batteryCharge + "\n"
                + "Vidro: " + this.glassTrash.Count + "\n"
                + "Metal: " + this.metalTrash.Count + "\n"
                + "Papel: " + this.paperTrash.Count + "\n"
                + "Plástico: " + this.plasticTrash.Count;
        }
    }
}