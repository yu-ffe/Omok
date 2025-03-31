const express = require('express');
const jwt = require('jsonwebtoken');  
const User = require('../../models/User'); 
var router = express.Router();

router.use(express.json());

// 등급 및 랭크 포인트 로직
const rankPointRange = 30;
const winPoints = { low: 10, middle: 6, high: 3 };
const losePoint = 10;

// 승리 시 랭크 포인트 계산
function getWinPoint(grade) {
    if (grade >= 10) return winPoints.low;   // 10급 이상
    else if (grade >= 5) return winPoints.middle; // 5~9급
    else return winPoints.high;  // 1~4급
}

router.post('/', async (req, res) => {
    try {
        const token = req.headers['authorization']?.split(' ')[1]; 

        if (!token) {
            return res.status(400).json({ message: "토큰이 제공되지 않았습니다." });
        }

        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        const email = decoded.email;  

        const { result } = req.body;

        if (result === undefined) {
            return res.status(400).json({ message: "게임 결과가 제공되지 않았습니다." });
        }

        // 유저 데이터 조회
        const user = await User.findOne({ email });

        if (!user) {
            return res.status(404).json({ message: "유저를 찾을 수 없습니다." });
        }

        let { winCount, loseCount, grade, rankPoint } = user;

        if (result) {  // 승리 시
            winCount += 1;
            const winPoint = getWinPoint(grade);
            rankPoint += winPoint;

            // 승급 체크
            if (rankPoint >= rankPointRange) {
                rankPoint = 0;
                grade = Math.max(1, grade - 1); // 1급이 최대 등급 (숫자가 작을수록 높음)
            }
        } else {  // 패배 시
            loseCount += 1;
            rankPoint -= losePoint;

            // 강등 체크
            if (rankPoint <= -rankPointRange) {
                rankPoint = 0;
                grade = Math.min(18, grade + 1); // 18급이 최하 등급
            }
        }

        // 변경된 값 업데이트
        const updatedUser = await User.findOneAndUpdate(
            { email },
            { winCount, loseCount, grade, rankPoint },
            { new: true }
        );

        res.json({
            message: "게임 결과가 저장되었습니다.",
            winCount: updatedUser.winCount,
            loseCount: updatedUser.loseCount,
            grade: updatedUser.grade,
            rankPoint: updatedUser.rankPoint
        });

    } catch (error) {
        console.error("게임 결과 저장 오류:", error);
        res.status(500).json({ message: "서버 오류" });
    }
});

module.exports = router;
