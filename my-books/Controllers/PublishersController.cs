using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using my_books.Data.Services;
using my_books.Data.ViewModels;
using my_books.Exceptions;
using System;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private PublishersServices _publishersServices;
        private readonly ILogger<PublishersController> _logger;
        public PublishersController(PublishersServices publishersServices, ILogger<PublishersController> logger)
        {
            _publishersServices = publishersServices;
            _logger = logger;
        }

        [HttpGet("get-all-publishers")]
        public IActionResult GetAllPublishers(string sortBy, string searchString, int pageNumber)
        {
            // throw new Exception("This is an exeption throw from get all publishers");
            try
            {
                _logger.LogInformation("This is just a log in GetAllPublishers()");

                var _result = _publishersServices.GetAllPublishers(sortBy, searchString, pageNumber);

                return Ok(_result);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, we could not load the publishers!");
            }
        }

        [HttpPost("add-publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVM publisher)
        {
            try
            {
                var newPublisher = _publishersServices.AddPublisher(publisher);
                return Created(nameof(AddPublisher), newPublisher);
            }
            catch (PublisherNameException ex)
            {
                return BadRequest($"{ex.Message}, Publisher name: {ex.PublisherName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById(int id)
        {
            // throw new Exception("This is an exception that will be handled by middleware");
            var publisher = _publishersServices.GetPublisherById(id);
            if (publisher != null)
            {
                return Ok(publisher);
                // return publisher;

                //var _responseObj = new CustomActionResultVM()
                //{
                //    Publisher = publisher
                //};

                //return new CustomActionResult(_responseObj);
            }
            else
            {
                return NotFound();
                //var _responseObj = new CustomActionResultVM()
                //{
                //    Exception = new Exception("This is coming from publishers controller")
                //};

                //return new CustomActionResult(_responseObj);
            }
        }

        [HttpGet("get-publisher-books-with-authors/{id}")]
        public IActionResult GetPublisherData(int id)
        {
            var publisher = _publishersServices.GetPublisherData(id);

            return Ok(publisher);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            try
            {
                _publishersServices.DeletePublisherById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
