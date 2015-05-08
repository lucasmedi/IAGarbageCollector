using Model.Interfaces;
using Model.Utils;

namespace Model.Agents
{
    public class Block : IBlock
    {
        private IPosition position;

        private IAgent agent;
        private ITrash trash;

        public Block(IPosition position)
        {
            this.position = position;
            this.setAgent(null);
            this.setTrash(null);
        }

        public Block(int x, int y)
        {
            this.position = new Position(x, y);
            this.setAgent(null);
            this.setTrash(null);
        }

        public Block(IPosition position, IAgent agent, ITrash trash)
        {
            this.position = position;
            this.setAgent(agent);
            this.setTrash(trash);
        }

        public Block(int x, int y, IAgent agent, ITrash trash)
        {
            this.position = new Position(x, y);
            this.setAgent(agent);
            this.setTrash(trash);
        }

        public string getIcon()
        {
            if (agent != null)
            {
                return agent.getIcon();
            }

            if (trash != null)
            {
                return trash.getIcon();
            }

            return null;
        }

        public ITrash collectTrash()
        {
            var t = this.trash;
            this.trash = null;
            return t;
        }

        public IPosition getPosition()
        {
            return position;
        }

        public void setPosition(IPosition position)
        {
            this.position = position;
        }

        public bool hasAgent()
        {
            return (this.agent != null);
        }

        public IAgent getAgent()
        {
            return agent;
        }

        public void setAgent(IAgent agent)
        {
            this.agent = agent;
        }

        public bool hasTrash()
        {
            return (this.trash != null);
        }

        public ITrash getTrash()
        {
            return trash;
        }

        public void setTrash(ITrash trash)
        {
            this.trash = trash;
        }

        public override string ToString()
        {
            return "(" + position.getX() + ", " + position.getY() + ")";
        }
    }
}