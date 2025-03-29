// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Enums {
    /// <summary>
    /// 게임에서 사용하는 플레이어 유형
    /// </summary>
    public enum PlayerType {
        // 플레이어가 존재하지 않는 상태
        None,
        
        // 첫 번째 플레이어 (PlayerA)
        PlayerA,
        
        // 두 번째 플레이어 (PlayerB)
        PlayerB,
        
        // 특수한 역할을 가진 플레이어 (PlayerX)
        PlayerX
    }
    
}
