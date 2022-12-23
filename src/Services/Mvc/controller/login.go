package controller

import (
	"bytes"
	"encoding/json"
	"io"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/librecuisine/mvc/config"
)

func GetLogin(c *gin.Context) {
	returnUrl := c.Query("returnUrl")
	if returnUrl == "" {
		returnUrl = "/"
	}
	token, notLoggedIn := c.Cookie("token")
	if token != "" && notLoggedIn == nil {
		c.Redirect(http.StatusFound, returnUrl)
		return
	}
	c.HTML(http.StatusOK, "login.gohtml", gin.H{
		"ReturnUrl": returnUrl,
	})
}

func GetRegister(c *gin.Context) {
	returnUrl := c.Query("returnUrl")
	if returnUrl == "" {
		returnUrl = "/"
	}
	token, notLoggedIn := c.Cookie("token")
	if token != "" && notLoggedIn == nil {
		c.Redirect(http.StatusFound, returnUrl)
		return
	}
	c.HTML(http.StatusOK, "register.gohtml", gin.H{
		"ReturnUrl": returnUrl,
	})
}

type LoginDto struct {
	Username string `form:"username" json:"username" binding:"required"`
	Password string `form:"password" json:"password" binding:"required"`
}

func PostLogin(c *gin.Context) {
	returnUrl := c.Query("returnUrl")
	if returnUrl == "" {
		returnUrl = "/"
	}
	var loginDto LoginDto
	err := c.ShouldBind(&loginDto)
	if err != nil {
		c.HTML(http.StatusBadRequest, "login.gohtml", gin.H{
			"ReturnUrl": c.Request.Form["returnUrl"],
			"Error":     err.Error(),
		})
		return
	}
	jsonLoginDto, err := json.Marshal(loginDto)
	res, err := http.Post(config.GetServerUrl()+"/i/login", "application/json", bytes.NewBuffer(jsonLoginDto))
	if err != nil {
		c.HTML(http.StatusBadRequest, "login.gohtml", gin.H{
			"ReturnUrl": returnUrl,
			"Error":     err.Error(),
		})
		return
	}
	if res.StatusCode != http.StatusOK {
		c.HTML(http.StatusBadRequest, "login.gohtml", gin.H{
			"ReturnUrl": returnUrl,
			"Error":     "Login failed",
		})
		return
	}
	var token string
	err = json.NewDecoder(res.Body).Decode(&token)
	if err != nil {
		c.HTML(http.StatusInternalServerError, "login.gohtml", gin.H{
			"ReturnUrl": returnUrl,
			"Error":     err.Error(),
		})
		return
	}
	c.SetCookie("token", token, 3600, "/", "", false, true)
	c.Redirect(http.StatusFound, returnUrl)
}

func PostRegister(c *gin.Context) {
	returnUrl := c.Query("returnUrl")
	if returnUrl == "" {
		returnUrl = "/"
	}
	var loginDto LoginDto
	err := c.ShouldBind(&loginDto)
	if err != nil {
		c.HTML(http.StatusBadRequest, "register.gohtml", gin.H{
			"ReturnUrl": c.Request.Form["returnUrl"],
			"Error":     err.Error(),
		})
		return
	}
	jsonLoginDto, err := json.Marshal(loginDto)
	res, err := http.Post(config.GetServerUrl()+"/i/register", "application/json", bytes.NewBuffer(jsonLoginDto))
	if err != nil {
		c.HTML(http.StatusBadRequest, "register.gohtml", gin.H{
			"ReturnUrl": returnUrl,
			"Error":     err.Error(),
		})
		return
	}
	if res.StatusCode != http.StatusOK {
		c.HTML(http.StatusBadRequest, "register.gohtml", gin.H{
			"ReturnUrl": returnUrl,
			"Error":     "Login failed",
		})
		return
	}
	var token []byte
	token, err = io.ReadAll(res.Body)
	if err != nil {
		c.HTML(http.StatusInternalServerError, "register.gohtml", gin.H{
			"ReturnUrl": returnUrl,
			"Error":     err.Error(),
		})
		return
	}
	c.SetCookie("token", string(token), 3600, "/", "", false, true)
	c.Redirect(http.StatusFound, returnUrl)

}

func Logout(c *gin.Context) {
	c.SetCookie("token", "", -1, "/", "", false, true)
	c.Redirect(http.StatusFound, "/")
}
