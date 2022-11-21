using MongoDB.Bson.Serialization.Attributes;

namespace Recipe.API.Models;

public class Recipe
{
    [BsonId]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public List<string> Steps { get; set; }
}