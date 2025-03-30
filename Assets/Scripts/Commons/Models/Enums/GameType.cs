// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Enums {
    /// <summary>
    /// 게임 플레이 유형
    /// </summary>
    public enum GameType {
        // 싱글 플레이어 모드
        SinglePlayer,

        // 두 명의 플레이어가 참여하는 듀얼 플레이어 모드
        DualPlayer,

        // 여러 명의 플레이어가 참여하는 멀티 플레이어 모드
        MultiPlayer,

        // 기록 모드 (게임을 기록하고 분석하는 모드)
        Record
    }
}
