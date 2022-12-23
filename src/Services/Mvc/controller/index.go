package controller

import (
	"encoding/json"
	"github.com/gin-gonic/gin"
	"github.com/librecuisine/mvc/config"
	"github.com/librecuisine/mvc/dtos"
	"net/http"
)

func Index(c *gin.Context) {
	token, noCookie := c.Cookie("token")

	var recipes []dtos.Recipe
	resp, err := http.Get(config.GetServerUrl() + "/r/api/Recipes")
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
		"Recipes":  recipes,
		"LoggedIn": token != "" && noCookie == nil,
		"Current":  "/",
	})
}
