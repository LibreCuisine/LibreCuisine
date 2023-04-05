using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Recipe.API.Controllers;
using Recipe.API.Models;
using Recipe.API.Profiles;
using Recipe.API.Repositories;
using Recipe.Common.Dtos;
using Recipe.Tests.Utils;
using Xunit.Abstractions;

namespace Recipe.Tests;

public class RecipesControllerTests
{
    private readonly ILogger<RecipesController> _logger;
    private readonly IMapper _mapper;

    private readonly List<API.Models.Recipe> _recipes = new()
    {
        new API.Models.Recipe
        {
            Id = Guid.NewGuid(),
            Name = "Test Recipe 1",
            Desc = "Test Recipe 1 Desc",
            Ingredients = new List<Ingredient>
            {
                new()
                {
                    Name = "Test Ingredient 1",
                    Amount = 1,
                    Unit = "g"
                },
                new()
                {
                    Name = "Test Ingredient 2",
                    Amount = 2,
                    Unit = "g"
                }
            },
            Steps = new List<string>
            {
                "Test Step 1",
                "Test Step 2"
            }
        }
    };
    
    public RecipesControllerTests(ITestOutputHelper output)
    {
        _logger = new XunitLogger<RecipesController>(output);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<RecipeApiProfile>());
        _mapper = config.CreateMapper();
    }
    [Fact]
    public void GetRecipes__Return_Recipes()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.GetRecipes()).Returns(_recipes);
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.GetRecipes();
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<ActionResult<IEnumerable<RecipeReadDto>>>(result);
        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.IsAssignableFrom<IEnumerable<RecipeReadDto>>(okResult.Value);
        var recipes = okResult.Value as IEnumerable<RecipeReadDto>;
        Assert.NotNull(recipes);
        Assert.Equal(_recipes.Count, recipes.Count());
    }
    [Fact]
    public void GetRecipeById__Return_Recipe()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.GetRecipeById(It.IsAny<Guid>())).Returns<Guid>(id => _recipes.FirstOrDefault(r => r.Id == id));
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.GetRecipeById(_recipes[0].Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<ActionResult<RecipeReadDto>>(result);
        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.IsAssignableFrom<RecipeReadDto>(okResult.Value);
        var recipe = okResult.Value as RecipeReadDto;
        Assert.NotNull(recipe);
        Assert.Equal(_recipes[0].Id, recipe.Id);
        Assert.Equal(_recipes[0].Name, recipe.Name);
        Assert.Equal(_recipes[0].Desc, recipe.Desc);
        Assert.Equal(_recipes[0].Ingredients.Count, recipe.Ingredients.Count());
        Assert.Equal(_recipes[0].Steps.Count, recipe.Steps.Count());
    }
    
    [Fact]
    public void GetRecipeById__Return_NotFound()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.GetRecipeById(It.IsAny<Guid>())).Returns<Guid>(id => _recipes.FirstOrDefault(r => r.Id == id));
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.GetRecipeById(Guid.NewGuid());
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<ActionResult<RecipeReadDto>>(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }
    
    [Fact]
    public void CreateRecipe__Return_CreatedRecipe()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.CreateRecipe(It.IsAny<API.Models.Recipe>())).Returns<API.Models.Recipe>(recipe =>
        {
            recipe.Id = Guid.NewGuid();
            _recipes.Add(recipe);
            return Task.CompletedTask;
        });
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.CreateRecipe(new RecipeCreateDto(
            "Test Recipe 2", 
            "Test Recipe 2 Desc",
            new List<IngredientDto>
            {
                new(
                    "Test Ingredient 3",
                    3,
                    "g"
                ),
                new(
                    "Test Ingredient 4",
                    4,
                    "g"
                )
            },
            new List<string>
            {
                "Test Step 3",
                "Test Step 4"
            }
        ));
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdResult);
        Assert.IsAssignableFrom<RecipeReadDto>(createdResult.Value);
        var recipe = createdResult.Value as RecipeReadDto;
        Assert.NotNull(recipe);
        Assert.Equal(recipe.Id, _recipes.Last().Id);
    }
    
    [Fact]
    public void UpdateRecipe__Return_NoContent()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.UpdateRecipe(It.IsAny<Guid>(), It.IsAny<API.Models.Recipe>())).Returns<Guid, API.Models.Recipe>((id, recipe) =>
        {
            var recipeToUpdate = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipeToUpdate == null)
            {
                return Task.CompletedTask;
            }
            recipeToUpdate.Name = recipe.Name;
            recipeToUpdate.Desc = recipe.Desc;
            recipeToUpdate.Ingredients = recipe.Ingredients;
            recipeToUpdate.Steps = recipe.Steps;
            return Task.CompletedTask;
        });
        repositoryMock.Setup(repo => repo.GetRecipeById(It.IsAny<Guid>()))
            .Returns<Guid>(id => _recipes.FirstOrDefault(r => r.Id == id));
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.UpdateRecipe(_recipes[0].Id, new RecipeCreateDto(
            "Test Recipe 1 Updated", 
            "Test Recipe 1 Desc Updated",
            new List<IngredientDto>
            {
                new(
                    "Test Ingredient 1 Updated",
                    1,
                    "g"
                ),
                new(
                    "Test Ingredient 2 Updated",
                    2,
                    "g"
                )
            },
            new List<string>
            {
                "Test Step 1 Updated",
                "Test Step 2 Updated"
            }
        ));
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result.Result);
    }
    
    [Fact]
    public void UpdateRecipe__Return_NotFound()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.UpdateRecipe(It.IsAny<Guid>(), It.IsAny<API.Models.Recipe>()))
            .Returns<Guid, API.Models.Recipe>((id, recipe) =>
        {
            var recipeToUpdate = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipeToUpdate == null)
            {
                return Task.CompletedTask;
            }
            recipeToUpdate.Name = recipe.Name;
            recipeToUpdate.Desc = recipe.Desc;
            recipeToUpdate.Ingredients = recipe.Ingredients;
            recipeToUpdate.Steps = recipe.Steps;
            return Task.CompletedTask;
        });
        repositoryMock.Setup(repo => repo.GetRecipeById(It.IsAny<Guid>()))
            .Returns<Guid>(id => _recipes.FirstOrDefault(r => r.Id == id));
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.UpdateRecipe(Guid.NewGuid(), new RecipeCreateDto(
            "Test Recipe 1 Updated", 
            "Test Recipe 1 Desc Updated",
            new List<IngredientDto>
            {
                new(
                    "Test Ingredient 1 Updated",
                    1,
                    "g"
                ),
                new(
                    "Test Ingredient 2 Updated",
                    2,
                    "g"
                )
            },
            new List<string>
            {
                "Test Step 1 Updated",
                "Test Step 2 Updated"
            }
        ));
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }
    
    [Fact]
    public void DeleteRecipe__Return_NoContent()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.DeleteRecipe(It.IsAny<Guid>())).Returns<Guid>(id =>
        {
            var recipeToDelete = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipeToDelete == null)
            {
                return Task.CompletedTask;
            }
            _recipes.Remove(recipeToDelete);
            return Task.CompletedTask;
        });
        repositoryMock.Setup(repo => repo.GetRecipeById(It.IsAny<Guid>()))
            .Returns<Guid>(id => _recipes.FirstOrDefault(r => r.Id == id));
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.DeleteRecipeById(_recipes[0].Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result.Result);
    }
    
    [Fact]
    public void DeleteRecipe__Return_NotFound()
    {
        // Arrange
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock.Setup(repo => repo.DeleteRecipe(It.IsAny<Guid>())).Returns<Guid>(id =>
        {
            var recipeToDelete = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipeToDelete == null)
            {
                return Task.CompletedTask;
            }
            _recipes.Remove(recipeToDelete);
            return Task.CompletedTask;
        });
        repositoryMock.Setup(repo => repo.GetRecipeById(It.IsAny<Guid>()))
            .Returns<Guid>(id => _recipes.FirstOrDefault(r => r.Id == id));
        
        var controller = new RecipesController(repositoryMock.Object, _logger, _mapper);
        
        // Act
        var result = controller.DeleteRecipeById(Guid.NewGuid());
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }
}