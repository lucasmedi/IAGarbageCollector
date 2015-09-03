using System.ComponentModel;
using Model.Interfaces;

namespace Web.Models
{
    public class ModelViewModel : IModel
    {
        #region IModel Members

        [DisplayName("World size")]
        public int size { get; set; }

        [DisplayName("Number of Collectors")]
        public int amountCollectors { get; set; }

        [DisplayName("Number of Trash Cans")]
        public int amountTrashCans { get; set; }

        [DisplayName("Number of Rechargers")]
        public int amountRechargers { get; set; }

        [DisplayName("Garbage capacity")]
        public int maxTrashCanCapacity { get; set; }

        [DisplayName("Collector capacity")]
        public int maxTrashCapacity { get; set; }

        [DisplayName("Battery Charge (per block)")]
        public int maxBatteryCapacity { get; set; }

        #endregion

        public ModelViewModel()
        {
            this.size = 15;
            this.amountCollectors = 1;
            this.amountTrashCans = 4;
            this.amountRechargers = 1;
            this.maxTrashCanCapacity = 5;
            this.maxTrashCapacity = 3;
            this.maxBatteryCapacity = 100;
        }
    }
}