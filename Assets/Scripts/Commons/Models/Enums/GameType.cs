// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Enums {
    /// <summary>
    /// 게임 타입
    /// </summary>
    public enum GameType {
        SinglePlayer,
        DualPlayer,
        MultiPlayer,
        Record
    }
}
