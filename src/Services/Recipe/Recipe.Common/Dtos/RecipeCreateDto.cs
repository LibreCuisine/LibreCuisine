namespace Recipe.Common.Dtos;

public record RecipeCreateDto(string Name, string Desc, IEnumerable<IngredientDto> Ingredients, IEnumerable<string> Steps);
