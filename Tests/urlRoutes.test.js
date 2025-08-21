const request = require("supertest");
const express = require("express");
const urlRoutes = require("../routes/urlRoutes");

// create an Express app just for testing
const app = express();
app.use(express.json());
app.use("/", urlRoutes);

describe("URL Shortener Routes", () => {
  it("should return error if long_url is missing", async () => {
    const res = await request(app).post("/shorten").send({});
    expect(res.status).toBe(400);
    expect(res.body).toEqual({ error: "long_url is required" });
  });

  it("should return error if URL is invalid", async () => {
    const res = await request(app)
      .post("/shorten")
      .send({ long_url: "not-a-url" });
    expect(res.status).toBe(400);
    expect(res.body).toEqual({ error: "Invalid URL format" });
  });

  it("should create a short URL for a valid long_url", async () => {
    const res = await request(app)
      .post("/shorten")
      .send({ long_url: "https://www.google.com" });

    expect(res.status).toBe(200);
    expect(res.body.short_url).toMatch(/http:\/\/localhost:3000\/[a-f0-9]{8}/);
  });
});
it("should redirect to the original URL", async () => {
  // first, create a short URL
  const createRes = await request(app)
    .post("/shorten")
    .send({ long_url: "https://www.google.com" });

  const shortUrl = createRes.body.short_url;
  const shortCode = shortUrl.split("/").pop();

  // now test redirect
  const redirectRes = await request(app).get(`/${shortCode}`);
  expect(redirectRes.status).toBe(302);
  expect(redirectRes.header.location).toBe("https://www.google.com");
});
