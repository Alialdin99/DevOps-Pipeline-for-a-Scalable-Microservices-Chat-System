const express = require("express");
const bodyParser = require("body-parser");
const cors = require("cors");
const urlRoutes = require("./routes/urlRoutes");

const app = express();
const PORT = 3000;

app.use(cors());
app.use(bodyParser.json()); 
app.use("/", urlRoutes);

app.listen(PORT, () => {
        console.log(` Server running on http://localhost:${PORT}`);
});
