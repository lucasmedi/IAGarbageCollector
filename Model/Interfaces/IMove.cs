namespace Model.Interfaces
{
    public interface IMove
    {
        IBlock From { get; set; }

        IBlock To { get; set; }

        string Log { get; set; }
    }
}