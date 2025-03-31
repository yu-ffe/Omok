// 1. 전략 패턴: Strategy Pattern
// 2. 생성자 주입: Constructor Injection
// 3. 팩토리 메서드 패턴: Factory Method Pattern

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Commons.Utils {
    /// <summary>
    /// 다양한 유효성 검사를 처리하는 유효성 검사 관리 클래스
    /// </summary>
    /// <remarks>
    /// 이 클래스는 특정 타입에 대한 유효성 검사를 수행하는 여러 검증기를 관리합니다. 
    /// 검증기는 이메일, 비밀번호, 닉네임, 이미지 인덱스 등을 검사할 수 있습니다.
    /// 각 검증은 입력값을 검사하고, 검사 결과와 메시지를 콜백으로 반환합니다.
    /// </remarks>
    public static class ValidationManager {
        // 유효성 검사를 수행할 검증기들을 저장하는 딕셔너리
        // 문자열 키를 사용하여 타입별로 유효성 검사를 찾을 수 있습니다.
        private static readonly Dictionary<string, IValidator> validators = new Dictionary<string, IValidator>(StringComparer.OrdinalIgnoreCase);

        static ValidationManager() {
            // 이메일, 비밀번호, 닉네임, 이미지 인덱스에 대한 검증기를 딕셔너리에 추가
            validators.Add("email", new EmailValidator());
            validators.Add("password", new PasswordValidator());
            validators.Add("nickname", new NicknameValidator());
            validators.Add("imgindex", new ImgIndexValidator());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 지정된 타입에 대한 유효성 검사를 수행하고 결과를 콜백으로 반환하는 메서드
        /// </summary>
        /// <param name="type">검사할 타입(예: "email", "password" 등)</param>
        /// <param name="input">검사할 입력값</param>
        /// <param name="callback">검사 결과와 메시지를 반환할 콜백 함수</param>
        /// <returns>유효성 검사 결과 (true 또는 false)</returns>
        /// <remarks>
        /// 이 메서드는 전달된 타입에 맞는 유효성 검사를 수행한 후, 콜백을 통해 검사 결과와 메시지를 반환합니다.
        /// 만약 타입에 해당하는 검증기가 존재하지 않으면, "지원하지 않는 유효성 검사 타입입니다."라는 메시지가 반환됩니다.
        /// </remarks>
        public static bool Validate(string type, object input, Action<bool, string> callback) {
            Debug.Log($"[ValidationManager] Validate - Type: {type}, Input: {input}");
            if (!validators.TryGetValue(type, out var validator)) {
                callback(false, "지원하지 않는 유효성 검사 타입입니다.");
                return false;
            }

            if (!validator.Validate(input, out string message)) {
                callback(false, message);
                return false;
            }

            Debug.Log($"[ValidationManager] Validate - {message}");

            return true;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 비밀번호와 비밀번호 확인이 일치하는지 검사하는 메서드
        /// </summary>
        /// <param name="password">비밀번호</param>
        /// <param name="confirmPassword">비밀번호 확인</param>
        /// <param name="callback">검사 결과와 메시지를 반환할 콜백 함수</param>
        /// <returns>유효성 검사 결과 (true 또는 false)</returns>
        /// <remarks>
        /// 이 메서드는 사용자가 입력한 비밀번호와 비밀번호 확인이 일치하는지 검사하고, 
        /// 그 결과를 콜백을 통해 반환합니다. 비밀번호가 일치하지 않으면 "비밀번호가 일치하지 않습니다."라는 메시지를 반환합니다.
        /// </remarks>
        public static bool ValidatePasswordMatch(string password, string confirmPassword, Action<bool, string> callback) {
            var passwordMatchValidator = new PasswordMatchValidator(confirmPassword);
            if (!passwordMatchValidator.Validate(password, out string message)){
                callback(false, message);
            }
            return true;
        }
    }
}
