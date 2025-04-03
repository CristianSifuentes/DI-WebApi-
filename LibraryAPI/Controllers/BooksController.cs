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

    [HttpGet("{id}")]
    public IActionResult GetBookById(int id){
        _loggerService.Log($"GET book with id: {id}");
        var book = _bookService.GetBookById(id);
        return book != null ? Ok(book): NotFound();
    }
    [HttpPost]
    public IActionResult AddBook([FromBody] Book book){
        _bookService.AddBook(book);
        _loggerService.Log($"Added book: {book.Title}");
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }
    [HttpDelete("{id}")]
    public IActionResult DeletedBook(int id){
        _bookService.DeleteBook(id);
        _loggerService.Log($"Deleted book with id: {id}");
        return NoContent();
    }
}