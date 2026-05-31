using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaAPI.Controllers
{
    public class ExecucaoController : Controller
    {
        // GET: ExecucaoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ExecucaoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExecucaoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExecucaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExecucaoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ExecucaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExecucaoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExecucaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
