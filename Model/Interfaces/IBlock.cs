
namespace Model.Interfaces
{
    public interface IBlock
    {
        string getIcon();

        ITrash collectTrash();

        IPosition getPosition();
        void setPosition(IPosition position);

        bool hasAgent();
        IAgent getAgent();
        void setAgent(IAgent agent);

        bool hasTrash();
        ITrash getTrash();
        void setTrash(ITrash trash);
    }
}