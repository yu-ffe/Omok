const mongoose = require("mongoose");

const UserSchema = new mongoose.Schema({
    id: {type: String, required: true, unique: true},
    nickname: { type: String, required: true, unique: true },
    profileNum: { type: Number, default: 1 },
    coins: { type: Number, default: 0 },
    grade: { type: Number, default: 1 },
    rankPoint: { type: Number, default: 0 },
    winCount: { type: Number, default: 0 },
    loseCount: { type: Number, default: 0 },
    password: { type: String, required: true },
    sessionToken: { type: String },
    refreshToken: { type: String },
});

module.exports = mongoose.model("User", UserSchema);
