package main

import (
	"encoding/json"
	"net/http"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/librecuisine/mvc/dtos"
)

type Config struct{
	ServerUrl string
}

func main() {
	config := Config{}
	serverUrl, ok := os.LookupEnv("SERVER_URL");
	if ok {
		config.ServerUrl = serverUrl
	} else {
		config.ServerUrl = "http://localhost:5200"
	}
	r := gin.Default()
	r.LoadHTMLGlob("templates/*")
	r.GET("/ping", func (c *gin.Context)  {
		c.JSON(200, gin.H{
			"message": "pong",
		})
	})
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
		c.HTML(http.StatusOK, "index.tmpl", gin.H{
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
		c.HTML(http.StatusOK, "recipe.tmpl", recipe)

	})
	r.Run();
}
