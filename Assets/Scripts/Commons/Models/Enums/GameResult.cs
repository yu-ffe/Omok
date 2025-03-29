// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Enums
{
    /// <summary>
    /// 게임 결과
    /// </summary>
    public enum GameResult
    {
        None,       // 게임 진행 중
        Win,        // 플레이어 승
        Lose,       // 플레이어 패
        Draw,       // 비김
        Player1Win, // DualPlayer용
        Player2Win  // DualPlayer용
    }
}
