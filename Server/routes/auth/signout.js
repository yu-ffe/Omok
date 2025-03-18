var express = require("express");
var router = express.Router();

// 로그아웃 처리
router.post("/", async (req, res) => {
    try {
        const { refreshToken } = req.body;

        // Refresh Token이 전달되지 않으면 오류 처리
        if (!refreshToken) {
            return res.status(400).json({ error: "Refresh token을 제공해야 합니다." });
        }

        // Refresh Token을 검증하여 유저 정보를 추출
        const decoded = jwt.verify(refreshToken, process.env.JWT_SECRET);
        const userId = decoded.userId;

        // 해당 유저를 찾아서 토큰 삭제
        const user = await User.findById(userId);
        if (!user) {
            return res.status(400).json({ error: "유효하지 않은 유저입니다." });
        }

        // 토큰 삭제
        user.sessionToken = null;  // 또는 user.sessionToken = '';
        user.refreshToken = null;  // 또는 user.refreshToken = '';
        await user.save();

        // 로그아웃 후 성공 메시지 전송
        res.json({ success: true, message: "로그아웃 성공" });
    } catch (err) {
        console.error("로그아웃 처리 오류:", err);
        res.status(500).json({ error: "로그아웃 처리 중 오류 발생" });
    }
});


module.exports = router;
