const express = require('express');
const jwt = require('jsonwebtoken');  // JWT 토큰을 처리하기 위한 라이브러리
const User = require('../../models/User'); // User 모델
var router = express.Router();

// 유저 정보 가져오기 API (accessToken을 이용한 인증)
router.get('/', async (req, res) => {
    try {
        // Authorization 헤더에서 Bearer 토큰 추출
        const token = req.headers['authorization']?.split(' ')[1]; // 'Bearer <token>' 형태로 전달됨

        if (!token) {
            return res.status(400).json({ message: "토큰이 제공되지 않았습니다." });
        }

        // 토큰 검증 (JWT 토큰을 디코딩하여 userId 추출)
        const decoded = jwt.verify(token, process.env.JWT_SECRET);  // 환경변수에서 JWT 비밀 키를 사용하여 디코딩

        const userId = decoded.userId;  // 토큰에서 유저 ID 추출

        // 해당 유저의 정보 조회
        const user = await User.findById(userId);  // userId로 User 찾기

        if (!user) {
            return res.status(404).json({ message: "유저를 찾을 수 없습니다." });
        }

        // 필요한 유저 정보만 응답
        const userInfo = {
            nickname: user.nickname,
            profileNum: user.profileNum,
            coins: user.coins,
            grade: user.grade,
            rankPoint: user.rankPoint,
            winCount: user.winCount,
            loseCount: user.loseCount,
        };

        // 유저 정보 응답
        res.json(userInfo);
    } catch (error) {
        console.error("유저 정보 조회 오류:", error);
        res.status(500).json({ message: "서버 오류" });
    }
});

module.exports = router;
