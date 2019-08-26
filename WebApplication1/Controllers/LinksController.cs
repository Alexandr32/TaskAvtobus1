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
        /// <returns></returns>
        public IActionResult Count()
        {
            return View();
        }

        // GET: Links/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Link
                .FirstOrDefaultAsync(m => m.Id == id);
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
            IEnumerable<Link> links;

            //Проверка сущетвует ли сгенерированная ссылка в БД
            while (true)
            {
                // Генерируемая ссылка
                shortLink = LinkGenerator.GeneratorShort();
                // Проверяем есть ли сущности в БД с созданым паролем
                links = _context.Link.Where(s => s.ShortURL == shortLink);

                // Если сущностей нет выходит из цикла иначе цикл выполняется по новому
                if (links.Count() == 0)
                {
                    break;
                }
            }

            link.ShortURL = shortLink;
            // Записываем дату создания
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
                    // Оставляем дату
                    editLink.DateCcreation = (DateTime) TempData["DateCcreation"];
                    // Оставляем счетчик
                    editLink.Count = (int) TempData["Count"];

                    _context.Update(editLink);
                    await _context.SaveChangesAsync();
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

        // GET: Links/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var link = await _context.Link
                .FirstOrDefaultAsync(m => m.Id == id);
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
    }
}
