// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Enums {
    /// <summary>
    /// 게임 플레이 결과
    /// </summary>
    public enum GameResult {
        // 게임 진행 중
        None,

        // 플레이어 승리
        Win,

        // 플레이어 패배
        Lose,

        // 비긴 상태
        Draw,

        // 듀얼 플레이어 모드에서 첫 번째 플레이어 승리
        Player1Win,

        // 듀얼 플레이어 모드에서 두 번째 플레이어 승리
        Player2Win
    }
}
