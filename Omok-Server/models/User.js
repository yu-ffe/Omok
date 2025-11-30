const mongoose = require("mongoose");
const { v4: uuidv4 } = require('uuid');  // uuid 라이브러리 추가

const UserSchema = new mongoose.Schema({
    id: { type: String, default: uuidv4 },  // id 필드 추가
    email: { type: String, required: true, unique: true },
    nickname: { type: String, required: true, unique: true },
    profileNum: { type: Number, default: 1 },
    profileUrl: { type: String },
    coins: { type: Number, default: 1000 },
    grade: { type: Number, default: 18 },
    rankPoint: { type: Number, default: 0 },
    winCount: { type: Number, default: 0 },
    loseCount: { type: Number, default: 0 },
    password: { type: String, required: true },
    sessionToken: { type: String },
    refreshToken: { type: String },
});

module.exports = mongoose.model("User", UserSchema);
