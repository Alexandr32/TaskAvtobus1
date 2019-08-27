using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LinksController : Controller
    {
        private readonly WebApplication1Context _context;

        public LinksController(WebApplication1Context context)
        {
            _context = context;
        }

        // GET: Links
        public async Task<IActionResult> Index()
        {
            return View(await _context.Link.ToListAsync());
        }

        /// <summary>
        /// Метод для подсчета переходов
        /// </summary>
        public async Task<IActionResult> Count(string linkShort)
        {
            Link link = await _context.Link.FirstOrDefaultAsync(s => s.ShortURL == linkShort);
            if (link != null)
            {
                link.Count++;

                _context.Update(link);
                await _context.SaveChangesAsync();

                return View(link);
            }
            else
            {
                return NotFound("Ссылка не найдена");
            }
            
        }

        /// <summary>
        /// GET: Вывод информации о ссылке
        /// </summary>
        /// <param name="id"></param>
        /// <returns>id записи</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Link.FirstOrDefaultAsync(m => m.Id == id);
            if (link == null)
            {
                return NotFound();
            }

            return View(link);
        }

        /// <summary>
        /// GET: Links/Create создание новой записи
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Links/Create создание новой записи
        /// </summary>
        /// <param name="link">Созданая сулка по данным введеных в форму</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LongURL,ShortURL")] Link link)
        {
            // Короткая ссылка
            string shortLink = string.Empty;

            //Проверка сущетвует ли сгенерированная ссылка в БД
            while (true)
            {
                // Генерируем ссылку
                shortLink = LinkGenerator.GeneratorShort();
                // Проверка на соответсвие
                if (!DuplicationCheck(shortLink))
                {
                    // Если ссылки нет то выходим из цикла
                    break;
                }
                
            }

            link.ShortURL = shortLink;
            link.DateCcreation = DateTime.Now;
            link.Count = 0;
            _context.Add(link);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET: Links/Edit/5 Редактирование
        /// </summary>
        /// <param name="id">id записи</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Link.FindAsync(id);
            // Дата создания и счетчик не меняется при редактировании сохраняем 
            // значение для вставки после редактирования
            TempData["DateCcreation"] = link.DateCcreation;
            TempData["Count"] = link.Count;
            if (link == null)
            {
                return NotFound();
            }
            return View(link);
        }

        /// <summary>
        /// POST: Links/Edit/5
        /// </summary>
        /// <param name="id">id записи</param>
        /// <param name="editLink">Отредактированая ссылка по даннм из формы</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LongURL,ShortURL")] Link editLink)
        {
            if (id != editLink.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Проверяем есть ли данная сылка
                    if (!DuplicationCheck(editLink.ShortURL))
                    {
                        // Оставляем дату
                        editLink.DateCcreation = (DateTime)TempData["DateCcreation"];
                        // Оставляем счетчик
                        editLink.Count = (int)TempData["Count"];

                        _context.Update(editLink);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound("Данная сылка уже существует");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinkExists(editLink.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editLink);
        }

        /// <summary>
        /// GET: Удаление ссылки
        /// </summary>
        /// <param name="id">id записи</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Link.FirstOrDefaultAsync(m => m.Id == id);
            if (link == null)
            {
                return NotFound();
            }

            return View(link);
        }

        /// <summary>
        /// POST: Links/Delete/5
        /// </summary>
        /// <param name="id">id удаляемой записи</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var link = await _context.Link.FindAsync(id);
            _context.Link.Remove(link);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверка на существование записи в БД
        /// </summary>
        /// <param name="id">id записи</param>
        /// <returns></returns>
        private bool LinkExists(int id)
        {
            return _context.Link.Any(e => e.Id == id);
        }

        /// <summary>
        /// Проверка существует ли данная ссылка в таблице
        /// </summary>
        /// <returns>True - существует, False - не существует</returns>
        private bool DuplicationCheck(string shortLink)
        {
            return _context.Link.Any(e => e.ShortURL == shortLink);
        }
    }
}
