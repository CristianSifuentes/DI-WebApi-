using LibraryAPI.Models;

namespace LibraryAPI.Services;


public interface IBookService {
    IEnumerable<Book> GetAllBooks();
    Book GetBookById(int id);
    void AddBook(Book book);
    void DeleteBook(int id);
    
}