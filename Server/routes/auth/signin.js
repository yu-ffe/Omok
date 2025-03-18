var express = require("express");
const bcrypt = require("bcrypt");
const User = require("../../models/User");
const jwt = require("jsonwebtoken");  // JWT 라이브러리 추가
var router = express.Router();

// 📌 로그인 처리
router.post("/", async (req, res) => {
    const { id, password } = req.body;

    try {
        // 사용자 존재 여부 확인
        const user = await User.findOne({ id });
        if (!user) {
            return res.status(400).json({ error: "아이디 또는 비밀번호가 잘못되었습니다." });
        }

        // 비밀번호 확인
        const isPasswordValid = await bcrypt.compare(password, user.password);
        if (!isPasswordValid) {
            return res.status(400).json({ error: "아이디 또는 비밀번호가 잘못되었습니다." });
        }

        // JWT 토큰 생성 (세션 관리용)
        const sessionToken = jwt.sign({ userId: user._id }, process.env.JWT_SECRET, { expiresIn: '1h' });
        const refreshToken = jwt.sign({ userId: user._id }, process.env.JWT_SECRET, { expiresIn: '7d' });

        // 사용자 정보 업데이트 (세션 토큰과 리프레시 토큰 저장)
        user.sessionToken = sessionToken;
        user.refreshToken = refreshToken;
        await user.save();

        // 로그인 성공 후 응답 전송
        res.json({
            login: {
                success: true,
                message: "로그인 성공",
                accessToken: sessionToken, // 새로 생성된 access token
                refreshToken: refreshToken // 새로 생성된 refresh token
            },
            data: {
                nickname: user.nickname,
                profileNum: user.profileNum,
                coins: user.coins,
                grade: user.grade,
                rankPoint: user.rankPoint,
                winCount: user.winCount,
                loseCount: user.loseCount,
                sessionToken,
                refreshToken
            }
        });
    } catch (err) {
        console.error("로그인 오류:", err);
        res.status(500).json({ error: "로그인 중 오류 발생" });
    }
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
