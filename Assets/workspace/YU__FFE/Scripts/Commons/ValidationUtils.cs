using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace workspace.YU__FFE.Scripts.Commons {
    // 검증 인터페이스
    public interface IValidator {
        bool Validate(string input, out string message);
    }

    // 이메일 검증 클래스
    public class EmailValidator : IValidator {
        public bool Validate(string input, out string message) {
            // 이메일이 유효한지 정규식을 사용하여 검사
            if (Regex.IsMatch(input, @"^[A-Za-z0-9+_.-]+@(.+)$")) {
                message = "이메일이 유효합니다."; // 유효한 이메일
                return true;
            }
            else {
                message = "유효한 이메일 형식이 아닙니다."; // 유효하지 않은 이메일
                return false;
            }
        }
    }

    // 비밀번호 검증 클래스
    public class PasswordValidator : IValidator {
        public bool Validate(string input, out string message) {
            // 비밀번호가 최소 8자 이상, 숫자와 문자를 포함하는지 정규식을 사용하여 검사
            if (Regex.IsMatch(input, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$")) {
                message = "비밀번호가 유효합니다."; // 유효한 비밀번호
                return true;
            }
            else {
                message = "비밀번호는 최소 8자 이상, 숫자와 문자를 포함해야 합니다."; // 유효하지 않은 비밀번호
                return false;
            }
        }
    }

    // 두 비밀번호가 일치하는지 검증 클래스
    public class PasswordMatchValidator : IValidator {
        private readonly string _confirmPassword;

        public PasswordMatchValidator(string confirmPassword) {
            _confirmPassword = confirmPassword;
        }

        public bool Validate(string input, out string message) {
            // 입력된 비밀번호가 확인 비밀번호와 일치하는지 검사
            if (input == _confirmPassword) {
                message = "비밀번호가 일치합니다."; // 비밀번호 일치
                return true;
            }
            else {
                message = "비밀번호가 일치하지 않습니다."; // 비밀번호 불일치
                return false;
            }
        }
    }

    // 닉네임 검증 클래스
    public class NicknameValidator : IValidator {
        public bool Validate(string input, out string message) {
            // 닉네임은 알파벳, 숫자, '_', '-', '.' 만 포함하며 길이는 3자 이상 20자 이하
            if (Regex.IsMatch(input, @"^[A-Za-z0-9_.-]{3,20}$")) {
                message = "닉네임이 유효합니다."; // 유효한 닉네임
                return true;
            }
            else {
                message = "닉네임은 알파벳, 숫자, '_', '-', '.'만 포함할 수 있으며, 3자 이상 20자 이하이어야 합니다."; // 유효하지 않은 닉네임
                return false;
            }
        }
    }

    // 검증 종류에 따른 선택을 가능하게 하는 서비스
    public static class ValidationManager {
        private static readonly Dictionary<string, IValidator> validators = new Dictionary<string, IValidator>();

        // 검증 항목 등록 (필요한 검증기를 등록)
        static ValidationManager() {
            validators.Add("email", new EmailValidator()); // 이메일 검증기 등록
            validators.Add("password", new PasswordValidator()); // 비밀번호 검증기 등록
            validators.Add("nickname", new NicknameValidator()); // 닉네임 검증기 등록
        }

        // 검증 실행 (유형에 맞는 검증기 선택)
        public static bool Validate(string type, string input, Action<bool, string> callback) {
            if (!validators.ContainsKey(type.ToLower())) return false; // 해당 타입의 검증기가 없는 경우 false 반환
            bool isValid = true;
            isValid = validators[type.ToLower()].Validate(input, out string message); // 검증기 실행

            if (!isValid) {
                callback(isValid, message); // 유효하지 않은 경우에만 콜백 호출
            }

            return isValid;
        }

        // 두 비밀번호가 같은지 검증
        public static bool ValidatePasswordMatch(string password, string confirmPassword, Action<bool, string> callback) {
            var passwordMatchValidator = new PasswordMatchValidator(confirmPassword); // 비밀번호 일치 검증기 생성
            bool isValid = passwordMatchValidator.Validate(password, out string message); // 검증 실행

            if (!isValid) {
                callback(isValid, message); // 비밀번호 불일치 시 콜백 호출
            }

            return isValid;
        }
    }
}
