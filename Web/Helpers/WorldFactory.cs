using Web.Models;

namespace Web.Helpers
{
    public class WorldFactory
    {
        public static CreatorViewModel NewInstance(ModelViewModel model)
        {
            var creatorViewModel = new CreatorViewModel();
            creatorViewModel.Creator.New(model);
            SessionHelper.CreatorViewModel = creatorViewModel;

            return SessionHelper.CreatorViewModel;
        }

        public static CreatorViewModel GetInstance()
        {
            if (SessionHelper.CreatorViewModel == null)
            {
                var creatorViewModel = new CreatorViewModel();
                creatorViewModel.Creator.New(creatorViewModel.ModelViewModel);
                SessionHelper.CreatorViewModel = creatorViewModel;
            }

            return SessionHelper.CreatorViewModel;
        }
    }
}