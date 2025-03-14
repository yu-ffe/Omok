const express = require("express");
const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const User = require("../models/User");

const router = express.Router();

// 🔹 회원가입 (Register)
router.post("/register", async (req, res) => {
  try {
    const { nickname, profileNum, password } = req.body;

    // 닉네임 중복 확인
    const existingUser = await User.findOne({ nickname });
    if (existingUser) {
      return res.status(400).json({ message: "이미 존재하는 닉네임입니다." });
    }

    // 비밀번호 해싱
    const hashedPassword = await bcrypt.hash(password, 10);

    // 새 사용자 생성
    const newUser = new User({
      nickname,
      profileNum,
      password: hashedPassword,
      coins: 0,
      grade: 1,
      rankPoint: 0,
      winCount: 0,
      loseCount: 0,
    });

    await newUser.save();
    res.status(201).json({ message: "회원가입 성공" });
  } catch (error) {
    res.status(500).json({ message: "서버 오류", error });
  }
});

// 🔹 로그인 (Login)
router.post("/login", async (req, res) => {
  try {
    const { nickname, password } = req.body;

    // 사용자 검색
    const user = await User.findOne({ nickname });
    if (!user) {
      return res.status(400).json({ message: "존재하지 않는 사용자입니다." });
    }

    // 비밀번호 확인
    const isMatch = await bcrypt.compare(password, user.password);
    if (!isMatch) {
      return res.status(400).json({ message: "비밀번호가 일치하지 않습니다." });
    }

    // JWT 토큰 생성
    const token = jwt.sign({ userId: user._id }, process.env.JWT_SECRET, { expiresIn: "1h" });

    // UserSession 형식으로 데이터 반환
    res.json({
      message: "로그인 성공",
      token,
      session: {
        Nickname: user.nickname,
        ProfileNum: user.profileNum,
        Coins: user.coins,
        Grade: user.grade,
        RankPoint: user.rankPoint,
        WinCount: user.winCount,
        LoseCount: user.loseCount,
      },
    });
  } catch (error) {
    res.status(500).json({ message: "서버 오류", error });
  }
});

module.exports = router;
