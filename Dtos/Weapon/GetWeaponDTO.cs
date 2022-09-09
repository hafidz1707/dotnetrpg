using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Dtos.Weapon
{
    public class GetWeaponDTO
    {
        public string Name {get; set; } = string.Empty;
        public int Damage {get; set;}
    }
}