package dtos

type Recipe struct {
	Id          string       `json:"id"`
	Name        string       `json:"name"`
	Desc        string       `json:"desc"`
	Ingredients []Ingredient `json:"ingredients"`
	Steps       []string     `json:"steps"`
}

type RecipeCreate struct {
	Name        string   `form:"name"`
	Desc        string   `form:"desc"`
	Ingredients []string `form:"ingredients[]"`
	Steps       []string `form:"steps[]"`
}

type Ingredient struct {
	Name   string  `json:"name"`
	Amount float64 `json:"amount"`
	Unit   string  `json:"unit"`
}
