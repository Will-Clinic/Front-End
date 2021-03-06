﻿using System;
using Xunit;
using WillClinic.Models;
using Microsoft.EntityFrameworkCore;
using WillClinic.Data;
using WillClinic.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;

using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Google.Apis.Logging;

namespace WillClinicTestBattery
{
    public class HomeControllerTest
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()

        .UseInMemoryDatabase(Guid.NewGuid().ToString())

        .Options;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        [Fact]
        public void HomeAboutResultView()
        {

            HomeController home = new HomeController();

            IActionResult result = home.About();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeContactResultView()
        {

            HomeController home = new HomeController();

            IActionResult result = home.Contact();

            Assert.IsType<ViewResult>(result);
        }        

    }
}
