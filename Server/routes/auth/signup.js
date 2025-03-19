var express = require("express");
const bcrypt = require("bcrypt");
const User = require("../../models/User");
var router = express.Router();

// 📌 회원가입 처리
router.post("/", async (req, res) => {
    console.log("회원가입 요청:", req.body); // 요청 바디 로그
    const { id, password, nickname, profileNum } = req.body;

    try {
        // 닉네임 중복 체크
        const existingUser = await User.findOne({ nickname });
        if (existingUser) {
            console.log("이미 존재하는 닉네임:", nickname); // 닉네임 중복 로그
            return res.status(400).json({ error: "이미 존재하는 닉네임입니다." });
        }

        // 새로운 사용자 생성
        let newUser = new User({ id, password, nickname, profileNum });
        await newUser.save();
        console.log("새로운 사용자 생성됨:", newUser); // 생성된 사용자 정보 로그

        // 응답 전송
        res.json({
            success: true,
            message: "회원가입 성공",
        });
    } catch (err) {
        console.error("회원가입 오류:", err); // 오류 발생 시 로그
        res.status(500).json({ error: "회원가입 중 오류 발생" });
    }
});

router.post("/check", async (req, res) => {
    const { type, value } = req.body; // req.query -> req.body로 변경

    if (!type || !value) {
        return res.status(400).json({ success: false, message: "type과 value를 모두 제공해야 합니다." });
    }

    try {
        let existingUser;

        // 닉네임 중복 체크
        if (type === "nickname") {
            existingUser = await User.findOne({ nickname: value });
            if (existingUser) {
                return res.status(400).json({ success: false, message: "이미 존재하는 닉네임입니다." });
            }
            return res.json({ success: true, message: "사용 가능한 닉네임입니다." });

        // 아이디 중복 체크
        } else if (type === "id") {
            existingUser = await User.findOne({ id: value });
            if (existingUser) {
                return res.status(400).json({ success: false, message: "이미 존재하는 아이디입니다." });
            }
            return res.json({ success: true, message: "사용 가능한 아이디입니다." });

        } else {
            return res.status(400).json({ success: false, message: "유효한 type 값(nickname 또는 id)을 제공해야 합니다." });
        }
    } catch (err) {
        console.error("중복 체크 오류:", err);
        res.status(500).json({ success: false, message: "중복 체크 중 오류 발생" });
    }
});


module.exports = router;
