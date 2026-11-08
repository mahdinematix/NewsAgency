using NewsAgency.EndPoints.WebApi.Controllers;

namespace NewsAgency.EndPoints.WebApi.Routing;

public static class ApiRouteNames
{
    public static class Articles
    {
        public const string GetAll =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.GetAll);

        public const string GetById =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.GetById);

        public const string Create =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.Create);

        public const string Edit =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.Edit);

        public const string Publish =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.Publish);

        public const string Reject =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.Reject);

        public const string Archive =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.Archive);

        public const string MoveToDraft =
            nameof(ArticlesController) + "." +
            nameof(ArticlesController.MoveToDraft);

        public const string GetByTitle = nameof(ArticlesController) + "." +
                                         nameof(ArticlesController.GetByTitle);
    }
}