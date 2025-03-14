require("dotenv").config();
var createError = require("http-errors");
var express = require("express");
var path = require("path");
var cookieParser = require("cookie-parser");
var logger = require("morgan");
var mongoose = require("mongoose");

var indexRouter = require("./routes/index");
var usersRouter = require("./routes/users");
var authRouter = require("./routes/auth");

var app = express();

// MongoDB 연결

var databaseURL = "mongodb://localhost:27017/Omok";
mongoose
  // .connect(process.env.MONGO_URI, {
  .connect(databaseURL, {
    useNewUrlParser: true,
    useUnifiedTopology: true,
  })
  .then(() => console.log("✅ MongoDB 연결 성공"))
  .catch((err) => console.error("❌ MongoDB 연결 실패:", err));

app.use(logger("dev"));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, "public")));

app.use("/", indexRouter);
app.use("/users", usersRouter);
app.use("/auth", authRouter); // 👈 로그인 & 회원가입 라우터 추가됨

// 404 에러 핸들링
app.use(function (req, res, next) {
  next(createError(404));
});

// 오류 핸들러
app.use(function (err, req, res, next) {
  res.locals.message = err.message;
  res.locals.error = req.app.get("env") === "development" ? err : {};
  res.status(err.status || 500);
  res.json({ error: err.message });
});

module.exports = app;



//ngrok config add-authtoken 2uIE2xkE7s0eq5mRRmjV20pYGHJ_2TEU1vctCqQBVmyZCXe1
//ngrok http http://localhost:80