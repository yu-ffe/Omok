using System;
using System.Text.RegularExpressions;

namespace workspace.YU__FFE.Scripts.Commons {
    using System;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    namespace workspace.YU__FFE.Scripts.Commons {
        // 검증 인터페이스
        public interface IValidator {
            bool Validate(string input);
        }

        // 이메일 검증
        public class EmailValidator : IValidator {
            public bool Validate(string input) {
                return Regex.IsMatch(input, @"^[A-Za-z0-9+_.-]+@(.+)$");
            }
        }

        // 비밀번호 검증
        public class PasswordValidator : IValidator {
            public bool Validate(string input) {
                return Regex.IsMatch(input, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$");
            }
        }

        // 두 비밀번호가 같은지 검증
        public class PasswordMatchValidator : IValidator {
            private readonly string _confirmPassword;

            public PasswordMatchValidator(string confirmPassword) {
                _confirmPassword = confirmPassword;
            }

            public bool Validate(string input) {
                return input == _confirmPassword;
            }
        }

        // 검증 종류에 따른 선택을 가능하게 하는 서비스
        public static class ValidationManager {
            private static readonly Dictionary<string, IValidator> validators = new Dictionary<string, IValidator>();

            // 검증 항목 등록 (필요한 검증기를 등록)
            static ValidationManager() {
                validators.Add("email", new EmailValidator());
                validators.Add("password", new PasswordValidator());
            }

            // 검증 실행 (유형에 맞는 검증기 선택)
            public static bool Validate(string type, string input) {
                if (validators.ContainsKey(type.ToLower())) {
                    return validators[type.ToLower()].Validate(input);
                }
                else {
                    throw new ArgumentException($"Invalid validation type: {type}");
                }
            }

            // 두 비밀번호가 같은지 검증
            public static bool ValidatePasswordMatch(string password, string confirmPassword) {
                var passwordMatchValidator = new PasswordMatchValidator(confirmPassword);
                return passwordMatchValidator.Validate(password);
            }
        }
    }

}
