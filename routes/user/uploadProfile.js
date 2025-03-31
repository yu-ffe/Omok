const express = require("express");
const multer = require("multer");
const path = require("path");
const router = express.Router();

// ✅ 이미지 업로드 설정
const storage = multer.diskStorage({
    destination: "./public/uploads",
    filename: (req, file, cb) => {
        cb(null, Date.now() + path.extname(file.originalname));
    }
});

const upload = multer({ storage: storage });

// ✅ 프로필 이미지 업로드 API
router.post("/", upload.single("file"), async (req, res) => {
    if (!req.file) {
        return res.status(400).json({ error: "파일 업로드 실패" });
    }

    const fileUrl = `http://localhost/uploads/${req.file.filename}`;
    res.json({ success: true, imageUrl: fileUrl });
});

// ✅ 반드시 `module.exports = router;` 추가!
module.exports = router;
