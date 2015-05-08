using Model.Utils;

namespace Model.Interfaces
{
    public interface ITrash
    {
        string getIcon();

        TrashType getTrashType();
    }
}