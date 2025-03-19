var express = require("express");
var jwt = require("jsonwebtoken");
var router = express.Router();

const SECRET_KEY = process.env.JWT_SECRET || "your_jwt_secret";
const REFRESH_SECRET = process.env.JWT_REFRESH_SECRET || "your_refresh_secret";

let refreshTokens = []; // 저장소 (DB 사용 권장)

// 📌 토큰 발급
router.post("/generate", function (req, res) {
  const { userId, username } = req.body;
  if (!userId || !username) {
    return res.status(400).json({ error: "userId와 username이 필요합니다." });
  }

  const payload = { userId, username };
  const accessToken = jwt.sign(payload, SECRET_KEY, { expiresIn: "15m" });
  const refreshToken = jwt.sign(payload, REFRESH_SECRET, { expiresIn: "7d" });

  refreshTokens.push(refreshToken); // 임시 저장 (DB 활용 권장)

  res.json({ accessToken, refreshToken });
});

// 📌 토큰 검증
router.post("/verify", function (req, res) {
  const { token } = req.body;
  if (!token) {
    return res.status(400).json({ error: "토큰이 필요합니다." });
  }

  jwt.verify(token, SECRET_KEY, (err, decoded) => {
    if (err) {
      return res.status(401).json({ error: "유효하지 않은 토큰입니다." });
    }
    res.json({ decoded });
  });
});

// 📌 토큰 갱신
router.post("/refresh", function (req, res) {
  const { refreshToken } = req.body;
  if (!refreshToken || !refreshTokens.includes(refreshToken)) {
    return res.status(403).json({ error: "유효하지 않은 리프레시 토큰입니다." });
  }

  jwt.verify(refreshToken, REFRESH_SECRET, (err, decoded) => {
    if (err) {
      return res.status(403).json({ error: "리프레시 토큰 검증 실패." });
    }
    
    const newAccessToken = jwt.sign({ userId: decoded.userId, username: decoded.username }, SECRET_KEY, { expiresIn: "15m" });
    res.json({ accessToken: newAccessToken });
  });
});

// 📌 로그아웃 (리프레시 토큰 제거)
router.post("/logout", function (req, res) {
  const { refreshToken } = req.body;
  refreshTokens = refreshTokens.filter(token => token !== refreshToken);
  res.json({ message: "로그아웃 성공" });
});

module.exports = router;