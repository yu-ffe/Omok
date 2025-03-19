require("dotenv").config();
var path = require("path"); 
var createError = require("http-errors");
var express = require("express");
var cookieParser = require("cookie-parser");
var logger = require("morgan");
var mongoose = require("mongoose");
var session = require("express-session");
var cors = require("cors"); // cors 모듈 추가


var app = express();

// 📌 MongoDB 연결 (환경 변수 활용)
var databaseURL = process.env.MONGO_URI || "mongodb://localhost:27017/Omok";
mongoose
  .connect(databaseURL)
  .then(() => console.log("✅ MongoDB 연결 성공"))
  .catch((err) => console.error("❌ MongoDB 연결 실패:", err));

// 📌 뷰 엔진 설정
app.set("view engine", "ejs");
app.set("views", path.join(__dirname, "views"));

// 📌 미들웨어 설정 (❗ 순서 중요)
app.use(logger("dev"));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static("public")); // 정적 파일 제공


// 📌 캐시 방지 설정 (브라우저가 항상 최신 HTML 요청) ✅ **위치 이동**
app.use((req, res, next) => {
  res.set("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
  res.set("Pragma", "no-cache");
  res.set("Expires", "0");
  next();
});

// 📌 세션 미들웨어 (라우터보다 먼저 적용)
app.use(session({
  secret: process.env.SESSION_SECRET || "your_secret_key",
  resave: false,
  saveUninitialized: true,
  cookie: { secure: false }  // HTTPS가 아니면 false
}));

// 📌 모든 요청에서 user 정보 제공 ✅ **위치 이동**
app.use((req, res, next) => {
  res.locals.user = req.session.user || null;
  console.log("👤 user:", res.locals.user);
  next();
});

// 기본 프로필 이미지 정적파일 제공
app.use("/images", express.static(path.join(__dirname, "public/images")))

var indexRouter = require("./routes/index");
var userRouter = require("./routes/user");
var authRouter = require("./routes/auth");
var signupRouter = require("./routes/auth/signup");
var signinRouter = require("./routes/auth/signin");
var signoutRouter = require("./routes/auth/signout");
var gameResultRouter = require("./routes/game/result");
var userInfoRouter = require("./routes/user/info");
var profileRouter = require("./routes/user/profile");

// 📌 라우트 설정
app.use("/", indexRouter);
app.use("/user", userRouter);
app.use("/auth", authRouter); 
app.use("/auth/signup", signupRouter);
app.use("/auth/signin", signinRouter); 
app.use("/auth/signout", signoutRouter); 
app.use("/game/result", gameResultRouter); 
app.use("/user/info", userInfoRouter); 
app.subscribe("/user", profileRouter);



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

// 📌 module.exports는 마지막에 실행
module.exports = app;
