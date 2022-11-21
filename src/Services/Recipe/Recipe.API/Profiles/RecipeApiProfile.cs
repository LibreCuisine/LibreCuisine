using AutoMapper;
using Recipe.API.Models;
using Recipe.Common.Dtos;

namespace Recipe.API.Profiles;

public class RecipeApiProfile: Profile
{
    public RecipeApiProfile()
    {
        CreateMap<Models.Recipe, RecipeReadDto>();
        CreateMap<Ingredient, IngredientDto>();
        CreateMap<IngredientDto, Ingredient>();
        CreateMap<RecipeCreateDto, Models.Recipe>();
    }
}