var express = require("express");
const bcrypt = require("bcrypt");
const User = require("../../models/User");
const jwt = require("jsonwebtoken");  // JWT 라이브러리 추가
const mongoose = require("mongoose");

var router = express.Router();
// 📌 로그인 처리
router.post("/", async (req, res) => {
    const { id, password } = req.body;

    // 요청 받은 데이터 출력
    console.log("요청 받은 데이터:", req.body);

    try {
        // 사용자 존재 여부 확인
        const user = await User.findOne({ id });
        console.log("사용자 조회 결과:", user); // 사용자 조회 결과 확인

        if (!user) {
            console.log("사용자 없음: 아이디 또는 비밀번호 잘못됨");
            return res.status(400).json({ error: "아이디 또는 비밀번호가 잘못되었습니다." });
        }

        // 비밀번호 확인
        const isPasswordValid = password === user.password;

        if (!isPasswordValid) {
            console.log("비밀번호 틀림: 아이디 또는 비밀번호 잘못됨");
            return res.status(400).json({ error: "아이디 또는 비밀번호가 잘못되었습니다." });
        }

        // JWT 토큰 생성 (세션 관리용)
        const accessToken = jwt.sign({ userId: user._id }, process.env.JWT_SECRET, { expiresIn: '1h' });
        const refreshToken = jwt.sign({ userId: user._id }, process.env.JWT_SECRET, { expiresIn: '7d' });
        console.log("새로운 토큰 생성:", { accessToken, refreshToken });

        // 사용자 정보 업데이트 (세션 토큰과 리프레시 토큰 저장)
        user.accessToken = accessToken;
        user.refreshToken = refreshToken;
        await user.save();
        console.log("사용자 정보 업데이트 완료");

        // 로그인 성공 후 응답 전송
        res.json({
            message: "로그인 성공",
            accessToken: accessToken, // 새로 생성된 access token
            refreshToken: refreshToken // 새로 생성된 refresh token
        });
    } catch (err) {
        console.error("로그인 오류:", err);
        res.status(500).json({ error: "로그인 중 오류 발생" });
    }
});

// 📌 추가: `/api/login` 엔드포인트
router.post("/api/login", async (req, res) => {
    return res.redirect("/auth/signin"); // 🔥 `/api/login`을 `/auth/signin`으로 리디렉션
});


// 📌 세션 갱신 처리 (Refresh Token을 사용하여 새로운 Session Token 발급)
router.post("/refresh", async (req, res) => {
    const { refreshToken } = req.body;

    if (!refreshToken) {
        return res.status(400).json({ error: "Refresh Token을 제공해야 합니다." });
    }

    try {
        // Refresh Token 검증
        const decoded = jwt.verify(refreshToken, process.env.JWT_SECRET);
        const userId = decoded.userId;

        // 새로운 Session Token 발급
        const newSessionToken = jwt.sign({ userId }, process.env.JWT_SECRET, { expiresIn: '1h' });
        const newRefreshToken = jwt.sign({ userId }, process.env.JWT_SECRET, { expiresIn: '7d' });

        // 사용자 정보 업데이트 (새로운 토큰 저장)
        const user = await User.findById(userId);
        user.sessionToken = newSessionToken;
        user.refreshToken = newRefreshToken;
        await user.save();

        res.json({
            success: true,
            message: "세션 갱신 성공",
            accessToken: newSessionToken,  // 갱신된 access token
            refreshToken: newRefreshToken // 갱신된 refresh token
        });
    } catch (err) {
        console.error("세션 갱신 오류:", err);
        res.status(500).json({ error: "세션 갱신 중 오류 발생" });
    }
});

module.exports = router;
