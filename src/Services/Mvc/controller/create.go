package controller

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/gin-gonic/gin"
	"github.com/librecuisine/mvc/config"
	"github.com/librecuisine/mvc/dtos"
	"net/http"
	"regexp"
	"strconv"
)

func GetCreate(c *gin.Context) {
	_, err := c.Cookie("token")
	if err != nil {
		c.Redirect(http.StatusFound, "/login?returnUrl=/create")
		return
	}
	c.HTML(http.StatusOK, "create.gohtml", nil)
}

func PostCreate(c *gin.Context) {
	var recipe dtos.RecipeCreate
	err := c.Bind(&recipe)
	if err != nil {
		fmt.Printf("error binding recipe: %v", err)
		c.Status(http.StatusBadRequest)
		return
	}
	var recipeDto dtos.Recipe
	recipeDto.Name = recipe.Name
	recipeDto.Desc = recipe.Desc
	re := regexp.MustCompile(`([0-9]+) (\w+) (.*)`)
	for _, ingredient := range recipe.Ingredients {
		var ingredientDto dtos.Ingredient
		matches := re.FindStringSubmatch(ingredient)
		if len(matches) != 4 {
			fmt.Printf("error parsing ingredient: %v", ingredient)
			c.Status(http.StatusBadRequest)
			return
		}
		ingredientDto.Amount, err = strconv.ParseFloat(matches[1], 64)
		if err != nil {
			fmt.Printf("error parsing ingredient amount: %v", ingredient)
			c.Status(http.StatusBadRequest)
			return
		}
		ingredientDto.Unit = matches[2]
		ingredientDto.Name = matches[3]
		recipeDto.Ingredients = append(recipeDto.Ingredients, ingredientDto)
	}
	recipeDto.Steps = recipe.Steps
	jsonRecipe, err := json.Marshal(recipeDto)
	if err != nil {
		fmt.Printf("error marshalling recipe: %v", err)
		c.Status(http.StatusInternalServerError)
		return
	}
	_, err = http.Post(config.GetServerUrl()+"/r/api/Recipes", "application/json", bytes.NewBuffer(jsonRecipe))
	if err != nil {
		fmt.Printf("error posting recipe: %v", err)
		c.Status(http.StatusBadRequest)
		return
	}
	c.Redirect(http.StatusFound, "/")
}
