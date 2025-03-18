var express = require("express");
const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const User = require("../models/User");
var router = express.Router();

// 📌 로그인 페이지 렌더링
router.get("/login", (req, res) => {
    res.render("login"); // "views/login.ejs" 렌더링
});

// 📌 회원가입 페이지 렌더링
router.get("/register", (req, res) => {
    res.render("register"); // "views/register.ejs" 렌더링
});

const SECRET_KEY = process.env.JWT_SECRET || "session_key";
const REFRESH_SECRET = process.env.JWT_REFRESH_SECRET || "refresh_key";


// 📌 회원가입 처리
router.post("/signup", async (req, res) => {
    // 요청 본문 출력
    console.log("회원가입 요청 본문:", req.body);

    const { id, password, nickname, profileNum } = req.body;

    try {
        // 닉네임 중복 체크
        const existingUser = await User.findOne({ nickname });
        if (existingUser) {
            console.log("이미 존재하는 닉네임:", nickname); // 닉네임 중복 로그
            return res.status(400).json({ error: "이미 존재하는 닉네임입니다." });
        }

        // 새로운 사용자 생성
        let newUser = new User({ id, password, nickname, profileNum });
        await newUser.save();
        console.log("새로운 사용자 생성됨:", newUser); // 생성된 사용자 정보 로그

        // 토큰 발급
        const payload = { id: newUser.id, nickname: newUser.nickname };
        const accessToken = jwt.sign(payload, SECRET_KEY, { expiresIn: "15m" });
        const refreshToken = jwt.sign(payload, REFRESH_SECRET, { expiresIn: "7d" });

        // MongoDB에 토큰 업데이트
        await User.updateOne(
            { _id: newUser._id },
            { $set: { sessionToken: accessToken, refreshToken: refreshToken } }
        );
        console.log("토큰 업데이트 완료:", accessToken, refreshToken); // 토큰 발급 로그

        // 응답 전송
        res.json({
            success: true,
            message: "회원가입 성공",
            accessToken,
            refreshToken,
        });
    } catch (err) {
        console.error("회원가입 오류:", err); // 오류 발생 시 로그
        res.status(500).json({ error: "회원가입 중 오류 발생" });
    }
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
