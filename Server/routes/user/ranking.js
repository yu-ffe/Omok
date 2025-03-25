const express = require('express');
const User = require('../../models/User'); // User 모델
var router = express.Router();

// 랭킹 정보 가져오기 API
router.get('/', async (req, res) => {
    try {
        // DB에서 grade 오름차순, rankPoint 내림차순으로 정렬하여 상위 10명 가져오기
        const rankings = await User.find()
            .sort({ grade: -1, rankPoint: 1, winCount: -1, loseCount: 1 })  // grade 오름차순, rankPoint 내림차순
            .limit(10);  // 상위 10명만 가져오기

        // 조회된 랭킹 데이터가 없다면
        if (!rankings || rankings.length === 0) {
            return res.status(404).json({ message: "랭킹을 찾을 수 없습니다." });
        }

        // 필요한 정보만 응답
        const rankingInfo = rankings.map(user => ({
            nickname: user.nickname,
            profileNum: user.profileNum,
            grade: user.grade,
            rankPoint: user.rankPoint,
            winCount: user.winCount,
            loseCount: user.loseCount,
        }));

        // 랭킹 정보 응답
        res.json(rankingInfo);
    } catch (error) {
        console.error("랭킹 조회 오류:", error);
        res.status(500).json({ message: "서버 오류" });
    }
});

module.exports = router;
