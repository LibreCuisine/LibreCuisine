using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Recipe.API.Settings;

namespace Recipe.API.Repositories;

public class RecipeRepository: IRecipeRepository
{
    private readonly ILogger<RecipeRepository> _logger;
    private readonly IMongoCollection<Models.Recipe> _recipes;

    public RecipeRepository(ILogger<RecipeRepository> logger, IOptions<RecipeDatabaseSettings> options)
    {
        _logger = logger;
        var settings = options.Value;
        _logger.LogInformation("Initializing RecipeRepository");
        _logger.LogDebug("ConnectionString: {SettingsConnectionString}", settings.ConnectionString);
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _recipes = database.GetCollection<Models.Recipe>(settings.CollectionName);
    }

    public IEnumerable<Models.Recipe> GetRecipes() => _recipes.FindSync(_ => true).ToList();
    public Models.Recipe? GetRecipeById(Guid id) => _recipes.Find(r => r.Id == id).FirstOrDefault();
    public async Task CreateRecipe(Models.Recipe recipe) => await _recipes.InsertOneAsync(recipe);
    public async Task UpdateRecipe(Guid id, Models.Recipe recipe) => await _recipes.ReplaceOneAsync(x => x.Id == id, recipe);
    public async Task DeleteRecipe(Guid id) => await _recipes.DeleteOneAsync(x => x.Id == id);
}