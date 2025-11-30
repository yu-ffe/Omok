// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Enums {
    /// <summary>
    /// 게임에서 사용되는 AI의 난이도 수준
    /// </summary>
    public enum AILevel {
        // 쉬운 난이도
        Easy,

        // 중간 난이도
        Middle,

        // 어려운 난이도
        Hard
    }
}
