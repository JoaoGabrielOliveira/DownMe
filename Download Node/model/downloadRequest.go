package model

type DownloadRequest struct {
	Download Download          `json:"download"`
	Action   DownloadOrderType `json:"action"`
}

type Download struct {
	Id               uint64 `json:"id"`
	Name             string `json:"Name"`
	Link             string `json:"Link"`
	Status           int8   `json:"status"`
	StartedAt        string `json:"StartedAt"`
	LastTimeAccessed string `json:"LastTimeAccessed"`
	FinishedAt       string `json:"FinishedAt"`
}

type DownloadOrderType struct {
	Name string `json:"name"`
	Key  string `json:"key"`
}
