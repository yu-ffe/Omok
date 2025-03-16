var express = require("express");
var router = express.Router();
const User = require("../models/User");
const bcrypt = require("bcrypt");

// 회원가입
router.post("/register", async (req, res) => {
    const { nickname, password } = req.body;

    try {
        const existingUser = await User.findOne({ nickname });
        if (existingUser) {
            return res.status(400).send("이미 존재하는 닉네임입니다.");
        }

        const hashedPassword = await bcrypt.hash(password, 10);
        const newUser = new User({ nickname, password: hashedPassword });

        await newUser.save();

        req.session.user = newUser;  // 세션에 저장
        req.session.save(() => {  // 세션 저장 후 이동
            res.redirect("/");
        });

    } catch (err) {
        res.status(500).send("회원가입 중 오류 발생");
    }
});
// 로그인
router.post("/login", async (req, res) => {
  const { nickname, password } = req.body;

  try {
      const user = await User.findOne({ nickname });
      if (!user) {
          return res.status(400).send("존재하지 않는 유저입니다.");
      }

      const isMatch = await bcrypt.compare(password, user.password);
      if (!isMatch) {
          return res.status(400).send("비밀번호가 일치하지 않습니다.");
      }

      req.session.user = user;  // 세션에 user 저장
      req.session.save((err) => {  // 세션 저장 후 콜백
          if (err) {
              console.log('세션 저장 실패:', err);
              return res.status(500).send("세션 저장 오류");
          }
          console.log('세션 저장 후 user:', req.session.user);
          res.redirect("/");
      });
ss
  } catch (err) {
      res.status(500).send("로그인 중 오류 발생");
  }
});

// 로그아웃
router.get("/logout", (req, res) => {
    req.session.destroy(() => {
        res.redirect("/");
    });
});

module.exports = router;
