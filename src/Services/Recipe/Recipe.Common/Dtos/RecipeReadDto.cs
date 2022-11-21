namespace Recipe.Common.Dtos;

public record RecipeReadDto(Guid Id, string Name, string Desc, IEnumerable<IngredientDto> Ingredients, IEnumerable<string> Steps);