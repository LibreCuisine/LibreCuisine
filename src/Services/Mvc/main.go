package main

import (
	"github.com/gin-gonic/gin"
	"github.com/librecuisine/mvc/config"
	"github.com/librecuisine/mvc/controller"
)

func main() {
	r := gin.Default()
	r.Static("/static", "./static")
	r.LoadHTMLGlob("templates/**/*")
	r.GET("/", controller.Index)
	r.GET("/:id", controller.Recipe)
	r.GET("/create", controller.GetCreate)
	r.POST("/create", controller.PostCreate)
	r.GET("/login", controller.GetLogin)
	r.POST("/login", controller.PostLogin)
	r.GET("/register", controller.GetRegister)
	r.POST("/register", controller.PostRegister)
	r.GET("/logout", controller.Logout)
	r.Run(config.GetPort())
}
