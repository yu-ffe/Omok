const express = require('express');
const jwt = require('jsonwebtoken');  // JWT 토큰을 처리하기 위한 라이브러리
const User = require('../../models/User'); // User 모델
var router = express.Router();

// JSON 바디를 파싱하기 위해 express의 미들웨어 추가
router.use(express.json());

// 승/패 결과 저장 API
router.post('/', async (req, res) => {
    try {
        // Authorization 헤더에서 Bearer 토큰 추출
        const token = req.headers['authorization']?.split(' ')[1]; // 'Bearer <token>' 형태로 전달됨

        if (!token) {
            return res.status(400).json({ message: "토큰이 제공되지 않았습니다." });
        }

        // 토큰 검증 (JWT 토큰을 디코딩하여 userId 추출)
        const decoded = jwt.verify(token, process.env.JWT_SECRET);  // 환경변수에서 JWT 비밀 키를 사용하여 디코딩

        const userId = decoded.userId;  // 토큰에서 유저 ID 추출

        // 클라이언트에서 받은 게임 결과 (승리 여부)
        const { result } = req.body;  // { result: true } or { result: false }

        if (result === undefined) {
            return res.status(400).json({ message: "게임 결과가 제공되지 않았습니다." });
        }

        // 해당 유저의 정보 조회
        const user = await User.findById(userId);  // userId로 User 찾기

        if (!user) {
            return res.status(404).json({ message: "유저를 찾을 수 없습니다." });
        }

        // 승/패에 따라 카운트 업데이트
        if (result) {
            user.winCount += 1;  // 승리 횟수 증가
        } else {
            user.loseCount += 1;  // 패배 횟수 증가
        }

        // 업데이트된 유저 정보 저장
        await user.save();

        // 응답: 게임 결과 저장 성공
        res.json({ message: "게임 결과가 저장되었습니다.", winCount: user.winCount, loseCount: user.loseCount });
    } catch (error) {
        console.error("게임 결과 저장 오류:", error);
        res.status(500).json({ message: "서버 오류" });
    }
});

module.exports = router;
