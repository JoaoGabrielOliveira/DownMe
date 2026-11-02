package model

type DownloadRequest struct {
	Link     string `json:"link"`
	Name     string `json:"name"`
	Priority int    `json:"priority"`
}
