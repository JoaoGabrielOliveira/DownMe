-- SQLite
CREATE TABLE downloads_history (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    link TEXT NOT NULL,
    status INT NOT NULL,
    started_at DATE,
    last_time_accessed DATE,
    finished_at DATE
);

CREATE TABLE download_files (
    id BIGINT UNIQUE PRIMARY KEY,
    download_id BIGINT,
        
    name TEXT,
    extension VARCHAR(5),
    directory TEXT,
    added_at DATE,
    updated_at DATE,

    FOREIGN KEY(download_id) REFERENCES downloads_history(id)
);

/*drop table downloads_history;*/