using Microsoft.AspNetCore.Mvc;
using LibraryAPI.Models;
using LibraryAPI.Services;


namespace LibraryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase {

    private readonly IBookService _bookService;
    private readonly ILoggerService _loggerService;

    public BooksController(IBookService bookService, ILoggerService loggerService){
        _bookService = bookService;
        _loggerService = loggerService;
    }

    [HttpGet]
    public IActionResult GetAllBooks(){
        _loggerService.Log("GET all books");
        return Ok(_bookService.GetAllBooks());
    }

}