var express = require("express");
var router = express.Router();
const User = require("../models/User");
const bcrypt = require("bcrypt");

// 📌 로그인 페이지 렌더링
router.get("/signin", (req, res) => {
    res.render("signin"); // "views/login.ejs" 렌더링
});

// 📌 회원가입 페이지 렌더링
router.get("/signup", (req, res) => {
    res.render("signup"); // "views/register.ejs" 렌더링
});

// 📌 회원가입 처리 (웹과 Unity 모두 지원)
router.post("/signup", async (req, res) => {
    const { nickname, password } = req.body;

    try {
        const existingUser = await User.findOne({ nickname });
        if (existingUser) {
            return res.status(400).send("이미 존재하는 닉네임입니다.");
        }

        const hashedPassword = await bcrypt.hash(password, 10);
        const newUser = new User({ nickname, password: hashedPassword });

        await newUser.save();

        req.session.user = { id: newUser._id, nickname: newUser.nickname };

        // 웹 요청일 경우 리다이렉트
        if (req.xhr || req.body.isJson) {
            return res.json({ success: true, message: "회원가입 성공!" });
        }

        // 웹 로그인 성공 시 리다이렉트
        res.redirect("/");

    } catch (err) {
        console.error("회원가입 오류:", err);
        res.status(500).send("회원가입 중 오류 발생");
    }
});

// 📌 로그인 처리
router.post("/signin", async (req, res) => {
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

        // 웹에서는 리다이렉트
        if (req.xhr || req.body.isJson) {
            // Unity 연동 시 JSON 반환
            return res.json({
                success: true,
                nickname: user.nickname,
                coins: user.coins || 0,
                profileNum: user.profileNum || 0,
                grade: user.grade || 0
            });
        }

        // 웹 로그인 성공 시 리다이렉트
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
