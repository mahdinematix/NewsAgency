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
    public static class Categories
    {
        public const string GetAll =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.GetAll);

        public const string GetById =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.GetById);

        public const string GetByTitle =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.GetByTitle);

        public const string GetParentByChildId =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.GetParentByChildId);

        public const string GetChildrenByParentId =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.GetChildrenByParentId);

        public const string Create =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.Create);

        public const string ChangeTitle =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.ChangeTitle);

        public const string ChangeParent =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.ChangeParent);

        public const string Remove =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.Remove);

        public const string Restore =
            nameof(CategoriesController) + "." +
            nameof(CategoriesController.Restore);


    }

    public static class Authors
    {
        public const string GetAll =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.GetAll);

        public const string GetById =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.GetById);

        public const string GetByFirstName =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.GetByFirstName);

        public const string GetByLastName =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.GetByLastName);

        public const string GetByEmail =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.GetByEmail);

        public const string GetByPhoneNumber =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.GetByPhoneNumber);

        public const string Create =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.Create);

        public const string ChangeFirstName =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.ChangeFirstName);

        public const string ChangeLastName =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.ChangeLastName);

        public const string ChangeEmail =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.ChangeEmail);

        public const string ChangePhoneNumber =
            nameof(AuthorsController) + "." +
            nameof(AuthorsController.ChangePhoneNumber);
    }

}