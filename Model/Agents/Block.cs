using Model.Utils;

namespace Model.Agents
{
    public class Block
    {
        private Position position;

        private Agent agent;
        private Trash trash;

        public Block(Position position)
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

        public Block(Position position, Agent agent, Trash trash)
        {
            this.position = position;
            this.setAgent(agent);
            this.setTrash(trash);
        }

        public Block(int x, int y, Agent agent, Trash trash)
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

        public Trash collectTrash()
        {
            var t = this.trash;
            this.trash = null;
            return t;
        }

        public override string ToString()
        {
            return "(" + position.getX() + ", " + position.getY() + ")";
        }

        public Position getPosition()
        {
            return position;
        }

        public void setPosition(Position position)
        {
            this.position = position;
        }

        public bool hasAgent()
        {
            return (this.agent != null);
        }

        public Agent getAgent()
        {
            return agent;
        }

        public void setAgent(Agent agent)
        {
            this.agent = agent;
        }

        public bool hasTrash()
        {
            return (this.trash != null);
        }

        public Trash getTrash()
        {
            return trash;
        }

        public void setTrash(Trash trash)
        {
            this.trash = trash;
        }
    }
}