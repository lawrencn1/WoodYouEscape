using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shooter
{
    public class SaveData
    {
        public static HashSet<ProjectileTypePlayer> UnlockedWeapons { get; private set; } = new HashSet<ProjectileTypePlayer>();

        public static void Initialize()
        {
            UnlockedWeapons.Clear();
            // Start ONLY with the Standard Axe
            UnlockedWeapons.Add(ProjectileTypePlayer.Standard);
        }

        public static void UnlockWeapon(ProjectileTypePlayer weapon)
        {
            if (!UnlockedWeapons.Contains(weapon))
            {
                UnlockedWeapons.Add(weapon);
            }
        }

        public static bool IsWeaponUnlocked(ProjectileTypePlayer weapon)
        {
            return UnlockedWeapons.Contains(weapon);
        }
    }
}

