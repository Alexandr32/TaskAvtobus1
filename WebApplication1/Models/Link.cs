using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Link
    {
        public int Id { get; set; }
        /// <summary>
        /// Длинные ссылки
        /// </summary>
        [Required, Display(Name = "Длинный URL")] // проверка на пробелы
        public string LongURL { get; set; }
        /// <summary>
        /// Короткий URL
        /// </summary>
        [Required, Display(Name = "Короткий URL")] // проверка на пробелы
        public string ShortURL { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        [Required, Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:hh:mm dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCcreation { get; set; }
        /// <summary>
        /// Кол-во переходов.
        /// </summary>
        [Required, Display(Name = "Кол-во переходов")]
        public int Count { get; set; }
    }
}
