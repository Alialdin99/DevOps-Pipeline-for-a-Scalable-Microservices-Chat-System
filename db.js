const sqlite3 = require("sqlite3").verbose();
const db = new sqlite3.Database("./urls.db");

// Create table if not exists
db.serialize(() => {
    db.run(`
    CREATE TABLE IF NOT EXISTS urls (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    short_code TEXT UNIQUE,
    long_url TEXT
    )
    `);
});

module.exports = db;
