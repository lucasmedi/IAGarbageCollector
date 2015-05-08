using Model.Controller;

namespace Web.Models
{
    public class CreatorViewModel
    {
        public ModelViewModel ModelViewModel { get; set; }

        public Creator Creator { get; set; }

        public CreatorViewModel()
        {
            ModelViewModel = new ModelViewModel();
            Creator = new Creator(ModelViewModel);
        }
    }
}