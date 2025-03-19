const express = require('express');
var router = express.Router();

// 유저 정보 가져오기 API
router.get('/', async (req, res) => {
    try {
        // 로그인된 유저 정보 가져오기 (세션 또는 JWT를 기반으로 유저 식별)
        const userId = req.user.id;  // 예시: JWT로 인증된 유저 ID (세션에서 가져올 수 있음)

        // 해당 유저의 정보 조회
        const user = await User.findOne({ id: userId });

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
