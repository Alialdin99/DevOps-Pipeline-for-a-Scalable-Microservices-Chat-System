const express = require("express");
const db = require("../db");
const router = express.Router();
const crypto = require("crypto");
const validUrl = require("valid-url");

router.post("/shorten", (req, res) => {
  const { long_url } = req.body;

  if (!long_url) {
    return res.status(400).json({ error: "long_url is required" });
  }
  if (!validUrl.isUri(long_url)) {
    return res.status(400).json({ error: "Invalid URL format" });
  }

  db.get(
    "SELECT short_code FROM urls WHERE long_url = ?",
    [long_url],
    (err, row) => {
      if (err) {
        return res.status(500).json({ error: "Database error" });
      }

      if (row) {
        return res.json({
          short_url: `http://localhost:3000/${row.short_code}`,
        });
      }

      const shortCode = crypto.randomBytes(4).toString("hex");

      db.run(
        "INSERT INTO urls (short_code, long_url) VALUES (?, ?)",
        [shortCode, long_url],
        function (err) {
          if (err) {
            return res.status(500).json({ error: "Database error" });
          }
          res.json({ short_url: `http://localhost:3000/${shortCode}` });
        }
      );
    }
  );
});

router.get("/:shortCode", (req, res) => {
  const { shortCode } = req.params;

  db.get(
    "SELECT long_url FROM urls WHERE short_code = ?",
    [shortCode],
    (err, row) => {
      if (err) {
        return res.status(500).json({ error: "Database error" });
      }
      if (!row) {
        return res.status(404).json({ error: "Short URL not found" });
      }
      res.redirect(row.long_url);
    }
  );
});

module.exports = router;
