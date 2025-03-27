var express = require("express");
const bcrypt = require("bcrypt");
const User = require("../../models/User");
const jwt = require("jsonwebtoken");
var router = express.Router();

// 📌 로그인 처리
router.post("/", async (req, res) => {
  const { email, password } = req.body;

  console.log("\n[로그인 요청]");
  console.log("📌 요청 받은 데이터:", req.body);

  try {
    // 사용자 존재 여부 확인
    const user = await User.findOne({ email });
    console.log("🔍 사용자 조회 결과:", user);

    if (!user) {
      console.log("❌ 사용자 없음: 아이디 또는 비밀번호 잘못됨");
      return res
        .status(400)
        .json({ error: "아이디 또는 비밀번호가 잘못되었습니다." });
    }

    // 비밀번호 검증
    const isPasswordValid = password === user.password;
    console.log("🔍 비밀번호 검증 결과:", isPasswordValid);

    if (!isPasswordValid) {
      console.log("❌ 비밀번호 틀림: 아이디 또는 비밀번호 잘못됨");
      return res
        .status(400)
        .json({ error: "아이디 또는 비밀번호가 잘못되었습니다." });
    }

    // JWT 토큰 생성 (세션 관리용)
    const accessToken = jwt.sign({ email }, process.env.JWT_SECRET, {
      expiresIn: "1h",
    });
    const refreshToken = jwt.sign({ email }, process.env.JWT_SECRET, {
      expiresIn: "7d",
    });
    console.log("✅ 새로운 토큰 생성 완료:", { accessToken, refreshToken });

    // 로그인 성공 후 응답 전송 (DB에 저장하지 않음)
    res.json({
      message: "로그인 성공",
      accessToken: accessToken,
      refreshToken: refreshToken,
    });
  } catch (err) {
    console.error("🚨 로그인 중 오류 발생:", err);
    res.status(500).json({ error: "로그인 중 오류 발생" });
  }
});

// 📌 자동 로그인 처리 (Refresh Token을 사용하여 자동 로그인)
router.get("/autoSignIn", async (req, res) => {
  const refreshToken = req.headers["authorization"]?.split(" ")[1]; // 'Bearer <refreshToken>'

  console.log("\n[자동 로그인 요청]");
  console.log("📌 받은 Refresh Token:", refreshToken);

  if (!refreshToken) {
    console.log("❌ Refresh Token이 제공되지 않음");
    return res.status(400).json({ error: "Refresh Token을 제공해야 합니다." });
  }

  try {
    // Refresh Token 검증
    let decoded;
    try {
        decoded = jwt.verify(refreshToken, process.env.JWT_SECRET);  // 비밀 키를 사용하여 디코딩
        console.log("🔍 Refresh Token 검증 완료. 사용자 이메일:", decoded.userId);  // 디버깅 추가
    } catch (err) {
        console.error("❌ Refresh Token 검증 실패:", err);  // 디버깅 추가
        return res.status(401).json({ message: "유효하지 않은 Refresh Token입니다." });
    }

    console.log("🔍 디코딩된 토큰:", decoded);  // 디버깅 추가
    const email = decoded.email;  // 토큰에서 이메일 추출

    // 해당 유저 정보 조회
    const user = await User.findOne({ email });  // 이메일로 User 찾기
    if (!user) {
        return res.status(404).json({ message: "유저를 찾을 수 없습니다." });
    }

    // 새로운 Session Token 발급 (accessToken, refreshToken)
    const newAccessToken = jwt.sign({ email }, process.env.JWT_SECRET, {
        expiresIn: "1h",  // 1시간 동안 유효
    });
    const newRefreshToken = jwt.sign({ email }, process.env.JWT_SECRET, {
        expiresIn: "7d",  // 7일 동안 유효
    });

    console.log("✅ 새로운 토큰 발급 완료:", {
        newAccessToken,
        newRefreshToken,
    });

    // 유저 정보와 함께 새로운 토큰을 응답
    res.json({
        message: "자동 로그인 성공",
        accessToken: newAccessToken,
        refreshToken: newRefreshToken
    });
} catch (err) {
    console.error("🚨 자동 로그인 중 오류 발생:", err);  // 디버깅 추가
    res.status(500).json({ message: "자동 로그인 중 오류 발생" });
}
});

// 📌 세션 갱신 처리 (Refresh Token을 사용하여 새로운 Session Token 발급)
router.post("/refresh", async (req, res) => {
  const { refreshToken } = req.body;

  console.log("\n[세션 갱신 요청]");
  console.log("📌 받은 Refresh Token:", refreshToken);

  if (!refreshToken) {
    console.log("❌ Refresh Token이 제공되지 않음");
    return res.status(400).json({ error: "Refresh Token을 제공해야 합니다." });
  }

  try {
    // Refresh Token 검증
    const decoded = jwt.verify(refreshToken, process.env.JWT_SECRET);
    const email = decoded.email;
    console.log("🔍 Refresh Token 검증 완료. 사용자 이메일:", userEmail);

    // 새로운 Session Token 발급
    const newSessionToken = jwt.sign({ email }, process.env.JWT_SECRET, {
      expiresIn: "1h",
    });
    const newRefreshToken = jwt.sign({ email }, process.env.JWT_SECRET, {
      expiresIn: "7d",
    });
    console.log("✅ 새로운 토큰 발급 완료:", {
      newSessionToken,
      newRefreshToken,
    });

    res.json({
      success: true,
      message: "세션 갱신 성공",
      accessToken: newSessionToken,
      refreshToken: newRefreshToken,
    });
  } catch (err) {
    console.error("🚨 세션 갱신 중 오류 발생:", err);
    res.status(500).json({ error: "세션 갱신 중 오류 발생" });
  }
});

module.exports = router;
