// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Response {
    public struct ResponseStatus {
        // 요청의 성공 여부
        public readonly bool Success;

        // 메시지
        public readonly string Message;

        /// <summary>
        /// 연결 상태 응답
        /// </summary>
        public ResponseStatus(bool success, string message) {
            Success = success;
            Message = message;
        }
    }
}