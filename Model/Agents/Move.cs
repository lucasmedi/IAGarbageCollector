using System.Text;
using Model.Interfaces;

namespace Model.Agents
{
    public class Move : IMove
    {
        public IBlock From { get; set; }

        public string Log { get; set; }

        public IBlock To { get; set; }
    }
}