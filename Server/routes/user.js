var express = require('express');
var router = express.Router();

var userInfoRouter = require("./user/info"); 

/* GET users listing. */
router.get('/user', function(req, res, next) {
  res.send('respond with a resource');
});

module.exports = router;
