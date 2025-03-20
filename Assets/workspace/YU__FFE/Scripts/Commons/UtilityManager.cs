using System;

namespace workspace.YU__FFE.Scripts.Commons {
    public class UtilityManager {
        internal static void EncryptPassword(ref string plainPassword) {
            plainPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainPassword));
        }
    }
}
