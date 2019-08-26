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

        // GET: Вывод информации о ссылке
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

        // GET: Links/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Links/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LongURL,ShortURL")] Link link)
        {
            string shortLink = string.Empty;

            //Проверка сущетвует ли сгенерированная ссылка в БД
            while (true)
            {
                // Генерируемая ссылка
                shortLink = LinkGenerator.GeneratorShort();
                // Проверка на соответсвие
                if (!DuplicationCheck(shortLink))
                {
                    // Если сылки нет то выходим из цикла
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
        
        // GET: Links/Edit/5
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

        // POST: Links/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Удаление ссылки
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

        // POST: Links/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var link = await _context.Link.FindAsync(id);
            _context.Link.Remove(link);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
            // Проверяем есть ли сущности в БД с созданой ссылкой
            IEnumerable<Link> links = _context.Link.Where(s => s.ShortURL == shortLink);

            if (links.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
