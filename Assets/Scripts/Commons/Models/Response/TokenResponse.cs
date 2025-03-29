// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Response {
    public struct TokenResponse {
        // 메시지
        public readonly string Message;

        // 액세스 토큰
        public readonly string AccessToken;

        // 리프레시 토큰
        public readonly string RefreshToken;
        
        /// <summary>
        /// 토큰 응답
        /// </summary>
        public TokenResponse(string message, string accessToken, string refreshToken) {
            Message = message;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
