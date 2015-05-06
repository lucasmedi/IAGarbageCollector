
namespace Model.Agents
{
    public abstract class Agent
    {
        private string name;
        private string icon;

        public Agent(string icon)
        {
            this.icon = icon;
        }

        public Agent(string name, string icon)
        {
            this.name = name;
            this.icon = icon;
        }

        public string getIcon()
        {
            return icon;
        }

        public void setIcon(string icon)
        {
            this.icon = icon;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}