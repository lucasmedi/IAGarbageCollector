using Model.Interfaces;
using Model.Utils;

namespace Model.Agents
{
    public class Trash : ITrash
    {
        private string icon;

        private TrashType trashType;

        public Trash(TrashType trashType, string icon)
        {
            this.trashType = trashType;
            this.icon = icon;
        }

        public string getIcon()
        {
            return icon;
        }

        public TrashType getTrashType()
        {
            return trashType;
        }

        public override string ToString()
        {
            return trashType.ToString();
        }
    }
}