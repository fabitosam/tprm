﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;
using TPRM.Application.Interface;
using TPRM.Domain.Entities;
using TPRM.MVC.ViewModels;

namespace TPRM.MVC.Controllers
{
    
    public class ServicosController : Controller
    {

        private readonly IServicoAppService _servicoApp;
        public ServicosController(IServicoAppService servicoApp)
        {
            _servicoApp = servicoApp;
        }

        [Authorize(Roles = "admin, analista")]
        // GET: Servicos
        //public ActionResult Index()
        //{
        //    var servicoViewModel = Mapper.Map<IEnumerable<Servico>, IEnumerable<ServicoViewModel>>(_servicoApp.GetAll());
        //    return View(servicoViewModel);
        //}
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var servicoViewModel = Mapper.Map<IEnumerable<Servico>, IEnumerable<ServicoViewModel>>(_servicoApp.GetAll());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescricaoParam = string.IsNullOrEmpty(sortOrder) ? "Descricao_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                servicoViewModel = servicoViewModel.Where(p => p.DescricaoServico.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "Descricao_desc":
                    servicoViewModel = servicoViewModel.OrderByDescending(s => s.DescricaoServico);
                    break;
                default:
                    servicoViewModel = servicoViewModel.OrderBy(p => p.DataCadastro);
                    break;
            }

            int pageSize = 3;
            int pageNumber = page ?? 1;
            return View(servicoViewModel.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "admin")]
        // GET: Servicos/Details/5
        public ActionResult Details(int id)
        {
            var servico = _servicoApp.GetById(id);
            var servicoViewModel = Mapper.Map<Servico, ServicoViewModel>(servico);

            return View(servicoViewModel);
        }

        [Authorize(Roles = "admin")]
        // GET: Servicos/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        // POST: Servicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServicoViewModel servico)
        {
            if (ModelState.IsValid)
            {
                var servicoDomain = Mapper.Map<ServicoViewModel, Servico>(servico);
                _servicoApp.Add(servicoDomain);
                return RedirectToAction("Index");
            }

            return View(servico);
        }

        [Authorize(Roles = "admin")]
        // GET: Servicos/Edit/5
        public ActionResult Edit(int id)
        {
            var servico = _servicoApp.GetById(id);
            var servicoViewModel = Mapper.Map<Servico, ServicoViewModel>(servico);

            return View(servicoViewModel);
        }

        [Authorize(Roles = "admin")]
        // POST: Servicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServicoViewModel servico)
        {
            if (ModelState.IsValid)
            {
                var servicoDomain = Mapper.Map<ServicoViewModel, Servico>(servico);
                _servicoApp.Update(servicoDomain);

                return RedirectToAction("Index");
            }

            return View(servico);
        }

        //// GET: Servicos/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    var servico = _servicoApp.GetById(id);
        //    var servicoViewModel = Mapper.Map<Servico, ServicoViewModel>(servico);

        //    return View(servicoViewModel);
        //}

        //// POST: Servicos/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var servico = _servicoApp.GetById(id);
        //    _servicoApp.Remove(servico);

        //    return RedirectToAction("Index");
        //}
    }
}