using LibraryAPI.Models;

namespace LibraryAPI.Services;

public class BookService : IBookService {
    private readonly List<Book> _books = new () {
        new Book { Id = 1, Title = "1984", Author = "George Orwell" }, 
        new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee" }
    };

    public IEnumerable<Book> GetAllBooks() => _books;

    public Book GetBookById(int id) => _books.FirstOrDefault(b => b.Id == id);

    public void AddBook(Book book) => _books.Add(book);

    public void DeleteBook(int id){
        var book = GetBookById(id);
        if(book != null) _books.Remove(book);
    }


}