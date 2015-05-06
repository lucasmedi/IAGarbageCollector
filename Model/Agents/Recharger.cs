using System.Collections.Generic;

namespace Model.Agents
{
    public class Recharger : Agent
    {
        private static string icon = "img/recharger.png";

        private List<Collector> collectors;
        private static int capacity = 2;

        public Recharger()
            : base(Recharger.icon)
        {
            collectors = new List<Collector>();
        }

        public bool isBusy()
        {
            return collectors.Count == capacity;
        }

        public bool addCollector(Collector collector)
        {
            if (collector == null || isBusy())
            {
                return false;
            }

            if (!collectors.Contains(collector))
            {
                collectors.Add(collector);
            }

            return true;
        }

        public bool removeCollector(Collector collector)
        {
            return collectors.Remove(collector);
        }

        public string getIcon()
        {
            return icon;
        }

        public void setIcon(string icon)
        {
            Recharger.icon = icon;
        }
    }
}