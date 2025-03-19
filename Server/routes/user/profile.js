var express = require("express");
const User = require("../../models/User");
var router = express.Router();

// 📌 유저 프로필 정보 가져오기
router.get("/", async (req, res) => {
    const { userId } = req.query;

    if (!userId) {
        return res.status(400).json({ error: "userId가 필요합니다." });
    }

    try {
        const user = await User.findOne({ id: userId });

        if (!user) {
            return res.json({ 
                nickname: "Guest",
                profileUrl: "http://localhost/images/default_profile.png" // ✅ 기본 프로필 제공
            });
        }

        res.json({ 
            nickname: user.nickname,
            profileUrl: user.profileUrl || "http://localhost/images/default_profile.png" // ✅ 기본값 설정
        });
    } catch (err) {
        console.error("프로필 불러오기 오류:", err);
        res.status(500).json({ error: "프로필 정보를 불러오는 중 오류 발생" });
    }
});

module.exports = router;