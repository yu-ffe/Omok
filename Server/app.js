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

// 📌 MongoDB 연결 (옵션 정리)
var databaseURL = "mongodb://localhost:27017/Omok";
mongoose
  .connect(databaseURL)
  .then(() => console.log("✅ MongoDB 연결 성공"))
  .catch((err) => console.error("❌ MongoDB 연결 실패:", err));

app.set("view engine", "ejs");
app.set("views", path.join(__dirname, "views"));


// 📌 미들웨어 설정
app.use(logger("dev"));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, "public"))); // 📌 정적 파일 제공

// 📌 라우트 설정
app.use("/", indexRouter);
app.use("/users", usersRouter);
app.use("/auth", authRouter); // 로그인 & 회원가입 라우터 추가

// 📌 404 에러 핸들링
app.use(function (req, res, next) {
  next(createError(404));
});

// 📌 오류 핸들러
app.use(function (err, req, res, next) {
  res.locals.message = err.message;
  res.locals.error = req.app.get("env") === "development" ? err : {};
  res.status(err.status || 500);
  res.json({ error: err.message });
});

module.exports = app;

var session = require("express-session");

app.use(session({
  secret: "your_secret_key",  // 보안 키
  resave: false,
  saveUninitialized: true,
  cookie: { secure: false }   // HTTPS가 아니라면 false로 설정
}));

// 세션에 인위적으로 user 객체를 추가하여 테스트
app.use((req, res, next) => {
  console.log("세션 미들웨어 실행");
  if (!req.session.user) {
    req.session.user = { nickname: "testUser", id: 123 }; // 세션에 user 객체 설정
    console.log("세션에 user 설정:", req.session.user); // 콘솔에서 세션 값 확인
  }
  next();
});

app.get("/", (req, res) => {
  if (req.session.user) {
    console.log("세션에 저장된 user:", req.session.user); // 콘솔에서 확인
    res.send(`<h1>Welcome, ${req.session.user.nickname}!</h1>`);
  } else {
    res.send("<h1>No user in session</h1>");
  }
});

app.listen(3000, () => {
  console.log("서버가 3000번 포트에서 실행 중...");
});