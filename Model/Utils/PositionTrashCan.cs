
namespace Model.Utils
{
    public class PositionTrashCan : Position
    {
        private TrashType trashType;

        public PositionTrashCan(TrashType trashType, int x, int y)
            : base(x, y)
        {
            this.trashType = trashType;
        }

        public TrashType getTrashType()
        {
            return trashType;
        }

        public void setTrashType(TrashType trashType)
        {
            this.trashType = trashType;
        }
    }
}