using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenNos.Domain;

namespace OpenNos.GameObject.Extension
{
    public static class CharacterExtension
    {
        public static string GetFamilyNameType(this Character e)
        {
            // Member 3 / Keeper 2 / Deputy 1 / Head 0
            var temp = (short)e.FamilyCharacter.Authority;
            return $"{temp + 915}";
        }
    }
}
