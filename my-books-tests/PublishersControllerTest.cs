using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using my_books.Controllers;
using my_books.Data;
using my_books.Data.Models;
using my_books.Data.Services;
using my_books.Data.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_books_tests
{
    public class PublishersControllerTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase(databaseName: "BookDbControllerTest")
           .Options;

        AppDbContext context;
        PublishersServices publishersServices;
        PublishersController publishersController;

        [OneTimeSetUp]
        public void Setup()
        {
            // code before running the unit test
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();

            publishersServices = new PublishersServices(context);
            publishersController = new PublishersController(publishersServices, new NullLogger<PublishersController>());
        }

        [Test, Order(1)]
        public void HTTPGET_GetAllPublishers_WithSortBySearchPageNr_ReturnOk_Test()
        {
            IActionResult actionResult = publishersController.GetAllPublishers("name_desc", "Publisher", 1);

            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

            var actionResultData = (actionResult as OkObjectResult).Value as List<Publisher>;

            Assert.That(actionResultData.First().Name, Is.EqualTo("Publisher 6"));
            Assert.That(actionResultData.First().Id, Is.EqualTo(6));
            Assert.That(actionResultData.Count, Is.EqualTo(2));


            IActionResult actionResultSecondPage = publishersController.GetAllPublishers("name_desc", "Publisher", 2);

            Assert.That(actionResultSecondPage, Is.TypeOf<OkObjectResult>());

            var actionResultSecondPageData = (actionResultSecondPage as OkObjectResult).Value as List<Publisher>;

            Assert.That(actionResultSecondPageData.First().Name, Is.EqualTo("Publisher 4"));
            Assert.That(actionResultSecondPageData.First().Id, Is.EqualTo(4));
            Assert.That(actionResultSecondPageData.Count, Is.EqualTo(2));
        }

        [Test, Order(2)]
        public void HTTPGET_GetPublisherById_ReturnOk_Test()
        {
            int publisherId = 2;
            IActionResult actionResult = publishersController.GetPublisherById(publisherId);

            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

            var publisherData = (actionResult as OkObjectResult).Value as Publisher;
            Assert.That(publisherData.Id, Is.EqualTo(2));
            Assert.That(publisherData.Name, Is.EqualTo("publisher 2").IgnoreCase);
        }

        [Test, Order(3)]
        public void HTTPGET_GetPublisherById_ReturnNoFound_Test()
        {
            int publisherId = 99;
            IActionResult actionResult = publishersController.GetPublisherById(publisherId);

            Assert.That(actionResult, Is.TypeOf<NotFoundResult>());
        }

        [Test, Order(4)]
        public void HTTPPOST_AddPublisher_ReturnsCreated_Test()
        {
            var newPunlisherVM = new PublisherVM()
            {
                Name = "New Publisher"
            };

            IActionResult actionResult = publishersController.AddPublisher(newPunlisherVM);

            Assert.That(actionResult, Is.TypeOf<CreatedResult>());
        }

        [Test, Order(5)]
        public void HTTPPOST_AddPublisher_ReturnsBadRequest_Test()
        {
            var newPunlisherVM = new PublisherVM()
            {
                Name = "123 New Publisher"
            };

            IActionResult actionResult = publishersController.AddPublisher(newPunlisherVM);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test, Order(6)]
        public void HTTPDELETE_DeletePublisherById_ReturnsOk_Test()
        {
            int publisherId = 6;
            IActionResult actionResult = publishersController.DeletePublisherById(publisherId);

            Assert.That(actionResult, Is.TypeOf<OkResult>());
        }

        [Test, Order(7)]
        public void HTTPDELETE_DeletePublisherById_ReturnsBadRequest_Test()
        {
            int publisherId = 6;
            IActionResult actionResult = publishersController.DeletePublisherById(publisherId);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            var publishers = new List<Publisher>
            {
                    new Publisher() {
                        Id = 1,
                        Name = "Publisher 1"
                    },
                    new Publisher() {
                        Id = 2,
                        Name = "Publisher 2"
                    },
                    new Publisher() {
                        Id = 3,
                        Name = "Publisher 3"
                    },
                    new Publisher() {
                        Id = 4,
                        Name = "Publisher 4"
                    },
                    new Publisher() {
                        Id = 5,
                        Name = "Publisher 5"
                    },
                    new Publisher() {
                        Id = 6,
                        Name = "Publisher 6"
                    },
            };
            context.Publishers.AddRange(publishers);

            context.SaveChanges();
        }
    }
}
