const jwt = require("jsonwebtoken");
var express = require("express");

const User = require('../../models/User'); 
var router = express.Router();

router.post("/", async (req, res) => {
  console.log("게임 시작 요청:", req.body);
  try {
    const authHeader = req.headers["authorization"]; // Authorization 헤더
    console.log("Authorization 헤더:", authHeader); // 디버깅 추가

    if (!authHeader) {
      return res
        .status(400)
        .json({ message: "Authorization 헤더가 제공되지 않았습니다." });
    }

    const token = authHeader.split(" ")[1]; // 'Bearer <token>' 형태로 토큰 추출
    console.log("추출된 토큰:", token); // 디버깅 추가

    if (!token) {
      return res.status(400).json({ message: "토큰이 제공되지 않았습니다." });
    }

    // 토큰 검증 (JWT 토큰을 디코딩하여 userId 추출)
    let decoded;
    try {
      decoded = jwt.verify(token, process.env.JWT_SECRET); // 환경변수에서 JWT 비밀 키를 사용하여 디코딩
      console.log("디코딩된 토큰:", decoded); // 디버깅 추가
    } catch (err) {
      console.error("토큰 검증 실패:", err); // 디버깅 추가
      return res.status(401).json({ message: "토큰이 유효하지 않습니다." });
    }

    const email = decoded.email; // 토큰에서 유저 ID 추출
    console.log("디코딩된 이메일:", email); // 디버깅 추가

    // 해당 유저의 정보 조회
    const user = await User.findOne({ email }); // email로 User 찾기 (findById는 _id 기준으로 검색하므로 변경)
    console.log("조회된 유저:", user); // 디버깅 추가

    if (!user) {
      return res.status(404).json({ message: "유저를 찾을 수 없습니다." });
    }

    // 코인이 100 이상이면 100 차감하고, 그 외에는 false와 메시지 반환
    if (user.coins >= 100) {
      const updatedUser = await User.findOneAndUpdate(
        { email },
        { $inc: { coins: -100 } }, // coins 필드를 100 차감
        { new: true } // 업데이트 후 새로 저장된 사용자 정보 반환
      );
      return res.json({
        success: true,
        message: "코인이 차감되었습니다. 게임 시작 성공",
        user: updatedUser, // 업데이트된 사용자 정보 포함
      });
    } else {
      return res.json({
        success: false,
        message: "코인이 부족합니다.",
      });
    }
  } catch (err) {
    console.error("로그아웃 처리 오류:", err);
    res.status(500).json({ error: "로그아웃 처리 중 오류 발생" });
  }
});

module.exports = router;
