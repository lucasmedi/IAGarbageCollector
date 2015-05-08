
namespace Model.Interfaces
{
    public interface IModel
    {
        int size { get; set; }
        int amountCollectors { get; set; }
        int amountTrashCans { get; set; }
        int amountRechargers { get; set; }
        int maxTrashCanCapacity { get; set; }
        int maxTrashCapacity{ get; set; }
        int maxBatteryCapacity { get; set; }
    }
}