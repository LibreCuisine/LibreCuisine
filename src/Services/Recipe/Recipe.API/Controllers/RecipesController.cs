using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recipe.API.Repositories;
using Recipe.Common.Dtos;

namespace Recipe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly ILogger<RecipesController> _logger;
        private readonly IMapper _mapper;

        public RecipesController(IRecipeRepository recipeRepo, ILogger<RecipesController> logger, IMapper mapper)
        {
            _recipeRepo = recipeRepo;
            _logger = logger;
            _mapper = mapper;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RecipeReadDto>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<RecipeReadDto>> GetRecipes()
        {
            _logger.LogInformation("Getting Recipes");
            var recipes = _recipeRepo.GetRecipes();
            return Ok(_mapper.Map<IEnumerable<RecipeReadDto>>(recipes));
        }

        [HttpGet("{id:guid}", Name = nameof(GetRecipeById))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RecipeReadDto), (int)HttpStatusCode.OK)]
        public ActionResult<RecipeReadDto> GetRecipeById(Guid id)
        {
            _logger.LogInformation("Getting Recipe with Id: {Id}", id);
            var recipe = _recipeRepo.GetRecipeById(id);
            if (recipe is null) return NotFound();
            return Ok(_mapper.Map<RecipeReadDto>(recipe));
        }

        [HttpPost]
        [ProducesResponseType(typeof(RecipeReadDto), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<RecipeReadDto>> CreateRecipe([FromBody] RecipeCreateDto recipeToCreate)
        {
            var recipe = _mapper.Map<Models.Recipe>(recipeToCreate);
            _logger.LogInformation("Creating new Recipe {Recipe}", recipe.Name);
            await _recipeRepo.CreateRecipe(recipe);
            return CreatedAtAction(nameof(GetRecipeById), new {id = recipe.Id}, _mapper.Map<RecipeReadDto>(recipe));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeCreateDto newData)
        {
            var recipe = _recipeRepo.GetRecipeById(id);
            if (recipe is null) return NotFound();
            var updateRecipe = _mapper.Map<Models.Recipe>(newData);
            updateRecipe.Id = recipe.Id;
            _logger.LogInformation("Updating Recipe: {UpdateRecipe}", updateRecipe.Name);
            await _recipeRepo.UpdateRecipe(id,updateRecipe);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteRecipeById(Guid id)
        {
            var recipe = _recipeRepo.GetRecipeById(id);
            if (recipe is null) return NotFound();
            _logger.LogInformation("Deleting Recipe: {Recipe}", recipe.Name);
            await _recipeRepo.DeleteRecipe(recipe.Id);
            return NoContent();
        }
    }
}
