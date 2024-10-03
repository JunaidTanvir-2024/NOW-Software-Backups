using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THPromotionPortal.Models.ViewModel;
using THPromotionPortal.Services.Interfaces;

namespace THPromotionPortal.Controllers
{
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        public PromotionController(IPromotionService accountService)
        {
            _promotionService = accountService;

        }

        // GET: PromotionController
        public async Task<IActionResult> Index()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var promotionResponse = await _promotionService.GetAllPromotion();
                    return View(promotionResponse.payload);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: PromotionController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var promotionResponse = await _promotionService.GetPromotion(id);
                    return View(promotionResponse.payload[0]);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: PromotionController/Create
        public ActionResult Create()
        {
            return View("~/Views/Promotion/CreatePromotion.cshtml");
        }

        // POST: PromotionController/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("CreatePromotions")]
        public async Task<IActionResult> CreatePromotions(CreatePromotionViewModel createPromotion)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return Ok(await _promotionService.CreatePromotion(createPromotion));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: PromotionController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var promotionResponse = await _promotionService.GetPromotion(id);
                    return View(promotionResponse.payload[0]);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View();
            }
        }

        // POST: PromotionController/Edit/5
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

        // GET: PromotionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PromotionController/Delete/5
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
