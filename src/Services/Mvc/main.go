package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"
	"os"
	"regexp"
	"strconv"

	"github.com/gin-gonic/gin"
	"github.com/librecuisine/mvc/dtos"
)

type Config struct {
	ServerUrl string
}

func main() {
	config := Config{}
	serverUrl, ok := os.LookupEnv("SERVER_URL")
	if ok {
		config.ServerUrl = serverUrl
	} else {
		config.ServerUrl = "http://localhost:5200"
	}
	r := gin.Default()
	r.Static("/static", "./static")
	r.LoadHTMLGlob("templates/**/*")
	r.GET("/", func(c *gin.Context) {
		var recipes []dtos.Recipe
		resp, err := http.Get(config.ServerUrl + "/r/api/Recipes")
		if err != nil {
			c.Status(http.StatusBadRequest)
			return
		}
		err = json.NewDecoder(resp.Body).Decode(&recipes)
		if err != nil {
			c.Status(http.StatusInternalServerError)
			return
		}
		c.HTML(http.StatusOK, "index.gohtml", gin.H{
			"Recipes": recipes,
		})
	})
	r.GET("/:id", func(c *gin.Context) {
		id := c.Param("id")
		resp, err := http.Get(config.ServerUrl + "/r/api/Recipes/" + id)
		if err != nil {
			c.Status(http.StatusBadRequest)
			return
		}
		var recipe dtos.Recipe
		err = json.NewDecoder(resp.Body).Decode(&recipe)
		if err != nil {
			c.Status(http.StatusInternalServerError)
			return
		}
		c.HTML(http.StatusOK, "recipe.gohtml", recipe)

	})
	r.GET("/create", func(c *gin.Context) {
		c.HTML(http.StatusOK, "create.gohtml", nil)
	})
	r.POST("/create", func(c *gin.Context) {
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
		_, err = http.Post(config.ServerUrl+"/r/api/Recipes", "application/json", bytes.NewBuffer(jsonRecipe))
		if err != nil {
			fmt.Printf("error posting recipe: %v", err)
			c.Status(http.StatusBadRequest)
			return
		}
		c.Redirect(http.StatusFound, "/")
	})
	r.Run()
}
