var express = require("express");
const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const User = require("../models/User");
var router = express.Router();

var signUpRouter = require("./auth/signup"); 
var signInRouter = require("./auth/signin"); 

// 📌 로그인 페이지 렌더링
router.get("/login", (req, res) => {
    res.render("login"); // "views/login.ejs" 렌더링
});

// 📌 회원가입 페이지 렌더링
router.get("/register", (req, res) => {
    res.render("register"); // "views/register.ejs" 렌더링
});


// 📌 로그인 처리
router.post("/login", async (req, res) => {
    const { nickname, password } = req.body;

    try {
        const user = await User.findOne({ nickname });
        if (!user) {
            return res.status(400).send("존재하지 않는 유저입니다.");
        }

        const isMatch = await bcrypt.compare(password, user.password);
        if (!isMatch) {
            return res.status(400).send("비밀번호가 일치하지 않습니다.");
        }

        req.session.user = { id: user._id, nickname: user.nickname };
        res.redirect("/");

    } catch (err) {
        console.error("로그인 오류:", err);
        res.status(500).send("로그인 중 오류 발생");
    }
});

// 📌 로그아웃
router.get("/logout", (req, res) => {
    req.session.destroy(() => {
        res.redirect("/");
    });
});

module.exports = router;
