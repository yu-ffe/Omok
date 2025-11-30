const express = require('express');
const jwt = require('jsonwebtoken');  // JWT 토큰을 처리하기 위한 라이브러리
const User = require('../../models/User'); // User 모델
var router = express.Router();

// 유저 정보 가져오기 API (accessToken을 이용한 인증)
router.get('/', async (req, res) => {
    try {
        // Authorization 헤더에서 Bearer 토큰 추출
        const authHeader = req.headers['authorization']; // Authorization 헤더
        console.log("Authorization 헤더:", authHeader);  // 디버깅 추가

        if (!authHeader) {
            return res.status(400).json({ message: "Authorization 헤더가 제공되지 않았습니다." });
        }

        const token = authHeader.split(' ')[1];  // 'Bearer <token>' 형태로 토큰 추출
        console.log("추출된 토큰:", token);  // 디버깅 추가

        if (!token) {
            return res.status(400).json({ message: "토큰이 제공되지 않았습니다." });
        }

        // 토큰 검증 (JWT 토큰을 디코딩하여 userId 추출)
        let decoded;
        try {
            decoded = jwt.verify(token, process.env.JWT_SECRET);  // 환경변수에서 JWT 비밀 키를 사용하여 디코딩
            console.log("디코딩된 토큰:", decoded);  // 디버깅 추가
        } catch (err) {
            console.error("토큰 검증 실패:", err);  // 디버깅 추가
            return res.status(401).json({ message: "토큰이 유효하지 않습니다." });
        }

        const email = decoded.email;  // 토큰에서 유저 ID 추출
        console.log("디코딩된 이메일:", email);  // 디버깅 추가

        // 해당 유저의 정보 조회
        const user = await User.findOne({ email });  // email로 User 찾기 (findById는 _id 기준으로 검색하므로 변경)
        console.log("조회된 유저:", user);  // 디버깅 추가

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
        console.log("응답할 유저 정보:", userInfo);  // 디버깅 추가

        // 유저 정보 응답
        res.json(userInfo);
    } catch (error) {
        console.error("유저 정보 조회 오류:", error);  // 디버깅 추가
        res.status(500).json({ message: "서버 오류" });
    }
});

module.exports = router;
