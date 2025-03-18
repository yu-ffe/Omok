var express = require("express");
const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const User = require("../models/User");
var router = express.Router();


// var signUpRouter = require("./auth/signup"); 
// var signInRouter = require("./routes/auth/signin"); 
// var logOutRouter = require("./routes/auth/signout"); 

// app.use("/auth/signup", signUpRouter);
// app.use("/signin", signInRouter);
// app.use("/signout", logOutRouter);


// 📌 로그인 페이지 렌더링
router.get("/login", (req, res) => {
    res.render("login"); // "views/login.ejs" 렌더링
});

// 📌 회원가입 페이지 렌더링
router.get("/register", (req, res) => {
    res.render("register"); // "views/register.ejs" 렌더링
});

// 📌 로그아웃
router.get("/logout", (req, res) => {
    req.session.destroy(() => {
        res.redirect("/");
    });
});

module.exports = router;
