﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WillClinic.Data;
using WillClinic.Models;

namespace WillClinic.Controllers
{
    public class AdminController : Controller
    {
        readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext ctx)
        {
            _context = ctx;
        }
        
        public IActionResult Index()
        {   
            // capturing the list of lawyers who are not yet verified
            var results = _context.Lawyers.Include(x => x.ApplicationUser).Where(x => x.IsVerified == false).Where(x => x.IsRejected != true).ToList();

            // Displaying above list to the View
            return View(results);            
        }

        public IActionResult VerifyLawyer(string id)
        {
            var attorney = _context.Lawyers.First(x => x.ApplicationUserId == id);

            if (attorney.ApplicationUserId == id)
            {
                attorney.IsVerified = true;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult RemoveLawyer(string id)
        {
            var attorney = _context.Lawyers.FirstOrDefault(x => x.ApplicationUserId == id);
            if (attorney != null)
            {
                attorney.IsRejected = true;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
    }
}