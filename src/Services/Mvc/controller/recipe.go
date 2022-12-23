package controller

import (
	"encoding/json"
	"github.com/gin-gonic/gin"
	"github.com/librecuisine/mvc/config"
	"github.com/librecuisine/mvc/dtos"
	"net/http"
)

type RecipeViewModel struct {
	*dtos.Recipe
	LoggedIn bool
	Current  string
}

func Recipe(c *gin.Context) {
	id := c.Param("id")
	resp, err := http.Get(config.GetServerUrl() + "/r/api/Recipes/" + id)
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
	token, noCookie := c.Cookie("token")
	c.HTML(
		http.StatusOK,
		"recipe.gohtml",
		RecipeViewModel{
			Recipe:   &recipe,
			LoggedIn: token != "" && noCookie == nil,
			Current:  "/" + id,
		})
}
