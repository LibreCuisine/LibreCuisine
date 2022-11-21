namespace Recipe.API.Repositories;

public interface IRecipeRepository
{
    IEnumerable<Models.Recipe> GetRecipes();
    Models.Recipe? GetRecipeById(Guid id);
    Task CreateRecipe(Models.Recipe recipe);
    Task UpdateRecipe(Guid id, Models.Recipe recipe);
    Task DeleteRecipe(Guid id);
}