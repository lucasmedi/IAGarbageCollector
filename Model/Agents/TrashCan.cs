using System.Collections.Generic;
using Model.Interfaces;
using Model.Utils;

namespace Model.Agents
{
    public class TrashCan : Agent
    {
        private int capacity;

        private TrashType color;
        private List<ITrash> content;

        public TrashCan(string name, string icon, int capacity, TrashType color)
            : base(name, icon)
        {
            this.capacity = capacity;
            this.color = color;

            this.content = new List<ITrash>();
        }

        public bool addTrash(ITrash t)
        {
            if (isFull())
            {
                return false;
            }

            content.Add(t);

            return true;
        }

        public bool isFull()
        {
            bool isFull = (capacity == content.Count);
            if (isFull)
            {
                setIcon("Content/Images/trashCanFull.png");
            }

            return isFull;
        }

        public TrashType getColor()
        {
            return color;
        }
    }
}