// 1. 전략 패턴: Strategy Pattern

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Commons.Utils {
    /// <summary>
    /// 유효성 검사 인터페이스
    /// </summary>
    /// <remarks>
    /// 이 인터페이스는 다양한 입력값을 검증하는 역할을 하는 클래스를 위한 기본 인터페이스입니다.
    /// </remarks>
    public interface IValidator {
        /// <summary>
        /// 주어진 입력값을 검증하는 메서드
        /// </summary>
        /// <param name="input">검증할 입력값</param>
        /// <param name="message">검증 결과 메시지</param>
        /// <returns>입력값이 유효한지 여부</returns>
        bool Validate(object input, out string message);
    }

    /// <summary>
    /// 이메일 검증 클래스
    /// </summary>
    /// <remarks>
    /// 이 클래스는 이메일 형식을 검증하는 역할을 합니다.
    /// </remarks>
    public class EmailValidator : IValidator {
        // 이메일 검증을 위한 정규식
        private static readonly Regex EmailRegex = new Regex(@"^[A-Za-z0-9+_.-]+@(.+)$", RegexOptions.Compiled);

        /// <summary>
        /// 이메일을 검증하는 메서드
        /// </summary>
        /// <param name="input">검증할 이메일</param>
        /// <param name="message">검증 결과 메시지</param>
        /// <returns>이메일이 유효한지 여부</returns>
        public bool Validate(object input, out string message) {
            if (input is string str && EmailRegex.IsMatch(str)) {
                message = "이메일이 유효합니다.";
                return true;
            } else {
                message = "유효한 이메일 형식이 아닙니다.";
                return false;
            }
        }
    }

    /// <summary>
    /// 비밀번호 검증 클래스
    /// </summary>
    /// <remarks>
    /// 이 클래스는 비밀번호가 특정 규칙을 충족하는지 검사합니다.
    /// 비밀번호는 최소 8자 이상, 숫자와 문자를 포함해야 합니다.
    /// </remarks>
    public class PasswordValidator : IValidator {
        /// <summary>
        /// 비밀번호를 검증하는 메서드
        /// </summary>
        /// <param name="input">검증할 비밀번호</param>
        /// <param name="message">검증 결과 메시지</param>
        /// <returns>비밀번호가 유효한지 여부</returns>
        public bool Validate(object input, out string message) {
            if (input is string str && Regex.IsMatch(str, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$")) {
                message = "비밀번호가 유효합니다.";
                return true;
            }
            else {
                message = "비밀번호는 최소 8자 이상, 숫자와 문자를 포함해야 합니다.";
                return false;
            }
        }
    }

    /// <summary>
    /// 비밀번호 일치 여부 검증 클래스
    /// </summary>
    /// <remarks>
    /// 이 클래스는 두 개의 비밀번호가 일치하는지 검증합니다.
    /// </remarks>
    public class PasswordMatchValidator : IValidator {
        private readonly string _confirmPassword;

        /// <summary>
        /// 생성자에서 확인할 비밀번호를 설정합니다.
        /// </summary>
        /// <param name="confirmPassword">확인할 비밀번호</param>
        public PasswordMatchValidator(string confirmPassword) {
            _confirmPassword = confirmPassword;
        }

        /// <summary>
        /// 비밀번호가 일치하는지 검증하는 메서드
        /// </summary>
        /// <param name="input">검증할 비밀번호</param>
        /// <param name="message">검증 결과 메시지</param>
        /// <returns>비밀번호가 일치하는지 여부</returns>
        public bool Validate(object input, out string message) {
            if (input is string str && str == _confirmPassword) {
                message = "비밀번호가 일치합니다.";
                return true;
            }
            else {
                message = "비밀번호가 일치하지 않습니다.";
                return false;
            }
        }
    }

    /// <summary>
    /// 닉네임 검증 클래스
    /// </summary>
    /// <remarks>
    /// 이 클래스는 닉네임의 형식이 유효한지 검사합니다.
    /// 유효한 닉네임은 알파벳, 숫자, '_', '-', '.' 문자만 포함하며, 길이는 3자 이상 20자 이하입니다.
    /// </remarks>
    public class NicknameValidator : IValidator {
        /// <summary>
        /// 닉네임을 검증하는 메서드
        /// </summary>
        /// <param name="input">검증할 닉네임</param>
        /// <param name="message">검증 결과 메시지</param>
        /// <returns>닉네임이 유효한지 여부</returns>
        public bool Validate(object input, out string message) {
            if (input is string str && Regex.IsMatch(str, @"^[A-Za-z0-9_.-]{3,20}$")) {
                message = "닉네임이 유효합니다.";
                return true;
            }
            else {
                message = "닉네임은 알파벳, 숫자, '_', '-', '.'만 포함할 수 있으며, 3자 이상 20자 이하이어야 합니다.";
                return false;
            }
        }
    }

    /// <summary>
    /// 이미지 인덱스 검증 클래스
    /// </summary>
    /// <remarks>
    /// 이 클래스는 이미지 인덱스가 1에서 4 사이의 값인지 검증합니다.
    /// </remarks>
    public class ImgIndexValidator : IValidator {
        /// <summary>
        /// 이미지 인덱스를 검증하는 메서드
        /// </summary>
        /// <param name="input">검증할 이미지 인덱스</param>
        /// <param name="message">검증 결과 메시지</param>
        /// <returns>이미지 인덱스가 유효한지 여부</returns>
        public bool Validate(object input, out string message) {
            if (input is int index && index >= 1 && index <= 4) {
                message = "이미지 인덱스가 유효합니다.";
                return true;
            }
            else {
                message = "이미지 인덱스는 1에서 4 사이의 값이어야 합니다.";
                return false;
            }
        }
    }
}
