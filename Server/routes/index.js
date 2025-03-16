var express = require("express");
var router = express.Router();

router.get("/", function (req, res, next) {
    // req.session.user가 정의되어 있지 않으면 null로 처리
    res.render("index", { user:"ete" });
});

module.exports = router;
