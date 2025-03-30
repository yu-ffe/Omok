// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 도움 클래스: Utility Class

using System;

namespace Commons.Utils {
    /// <summary>
    /// 유틸리티 관련 기능을 제공하는 클래스
    /// </summary>
    /// <remarks>
    /// 이 클래스는 다양한 유틸리티 메서드를 제공하는 관리 클래스로,
    /// 주로 보안과 관련된 작업(예: 비밀번호 암호화)을 처리합니다.
    /// </remarks>
    public static class UtilityManager {
        /// <summary>
        /// 비밀번호를 Base64로 암호화하는 메서드
        /// </summary>
        /// <param name="plainPassword">암호화할 평문 비밀번호</param>
        /// <remarks>
        /// 이 메서드는 입력된 평문 비밀번호를 Base64로 인코딩하여 암호화된 비밀번호로 변경합니다.
        /// Base64 인코딩은 보안을 위한 본격적인 암호화 방법은 아니지만, 
        /// 간단한 변환 작업으로 평문 비밀번호를 숨기는데 사용됩니다.
        /// </remarks>
        internal static void EncryptPassword(ref string plainPassword) {
            // 평문 비밀번호를 UTF8 바이트 배열로 변환하고, Base64로 인코딩하여 암호화
            plainPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainPassword));
        }
    }
}
