using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public interface IValidator {
    bool Validate(string input, out string message);
}

public class EmailValidator : IValidator {
    public bool Validate(string input, out string message) {
        if (Regex.IsMatch(input, @"^[A-Za-z0-9+_.-]+@(.+)$")) {
            message = "이메일이 유효합니다.";
            return true;
        }
        else {
            message = "유효한 이메일 형식이 아닙니다.";
            return false;
        }
    }
}

public class PasswordValidator : IValidator {
    public bool Validate(string input, out string message) {
        if (Regex.IsMatch(input, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$")) {
            message = "비밀번호가 유효합니다.";
            return true;
        }
        else {
            message = "비밀번호는 최소 8자 이상, 숫자와 문자를 포함해야 합니다.";
            return false;
        }
    }
}

public class PasswordMatchValidator : IValidator {
    private readonly string _confirmPassword;

    public PasswordMatchValidator(string confirmPassword) {
        _confirmPassword = confirmPassword;
    }

    public bool Validate(string input, out string message) {
        if (input == _confirmPassword) {
            message = "비밀번호가 일치합니다.";
            return true;
        }
        else {
            message = "비밀번호가 일치하지 않습니다.";
            return false;
        }
    }
}

public class NicknameValidator : IValidator {
    public bool Validate(string input, out string message) {
        if (Regex.IsMatch(input, @"^[A-Za-z0-9_.-]{3,20}$")) {
            message = "닉네임이 유효합니다.";
            return true;
        }
        else {
            message = "닉네임은 알파벳, 숫자, '_', '-', '.'만 포함할 수 있으며, 3자 이상 20자 이하이어야 합니다.";
            return false;
        }
    }
}

public static class ValidationManager {
    private static readonly Dictionary<string, IValidator> validators = new Dictionary<string, IValidator>();

    static ValidationManager() {
        validators.Add("email", new EmailValidator());
        validators.Add("password", new PasswordValidator());
        validators.Add("nickname", new NicknameValidator());
    }

    public static bool Validate(string type, string input, Action<bool, string> callback) {
        if (!validators.ContainsKey(type.ToLower())) return false;
        bool isValid = true;
        isValid = validators[type.ToLower()].Validate(input, out string message);

        if (!isValid) {
            callback(isValid, message);
        }

        return isValid;
    }

    public static bool ValidatePasswordMatch(string password, string confirmPassword, Action<bool, string> callback) {
        var passwordMatchValidator = new PasswordMatchValidator(confirmPassword);
        bool isValid = passwordMatchValidator.Validate(password, out string message);

        if (!isValid) {
            callback(isValid, message);
        }

        return isValid;
    }
}
