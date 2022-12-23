package config

import (
	"fmt"
	"os"
)

type Config struct {
	ServerUrl string
	Port      string
}

var config *Config = DefaultConfig()

func DefaultConfig() *Config {
	conf := Config{
		ServerUrl: "http://localhost:5200",
		Port:      ":8080",
	}
	serverUrl, ok := os.LookupEnv("SERVER_URL")
	if ok {
		fmt.Printf("SERVER_URL=%s\n", serverUrl)
		conf.ServerUrl = serverUrl
	}
	port, ok := os.LookupEnv("MVC_PORT")
	if ok {
		conf.Port = port
	}
	return &conf
}

func GetConfig() *Config {
	return config
}

func GetServerUrl() string {
	return config.ServerUrl
}

func GetPort() string {
	return config.Port
}
