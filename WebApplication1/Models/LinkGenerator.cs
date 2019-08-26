using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    /// <summary>
    /// Класс для генерации ссылок
    /// </summary>
    public static class LinkGenerator
    {
        /// <summary>
        /// Герератор коротких ссылок
        /// </summary>
        /// <returns>Короткая ссылка</returns>
        public static string GeneratorShort()
        {
            string resul = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                resul += RandomCharacterGenerator().ToString();
            }

            return string.Format("/{0}/", resul);
        }

        /// <summary>
        /// Генерирует случайный чисел
        /// </summary>
        /// <returns>Случайное число</returns>
        private static int RandomCharacterGenerator()
        {
            Random rand = new Random();
            return rand.Next(0, 9);
        }
    }
}
