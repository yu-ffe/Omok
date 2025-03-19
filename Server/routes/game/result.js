const express = require('express');
var router = express.Router();

// 승/패 결과 저장 API
router.post('/', async (req, res) => {
    try {
        // 로그인된 유저 정보 가져오기 (JWT 또는 세션을 통해 인증된 유저 정보)
        const userId = req.user.id;  // 예시: JWT로 인증된 유저 ID (세션에서 가져올 수 있음)

        // 클라이언트에서 받은 게임 결과 (승리 여부)
        const { result } = req.body;  // { result: true } or { result: false }

        if (result === undefined) {
            return res.status(400).json({ message: "게임 결과가 제공되지 않았습니다." });
        }

        // 해당 유저의 정보 조회
        const user = await User.findOne({ id: userId });

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
