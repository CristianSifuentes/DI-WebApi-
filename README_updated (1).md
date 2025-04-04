# Advanced Guide to Dependency Injection in ASP.NET Core

## Table of Contents

1. [Introduction](#introduction)
2. [Project Setup](#project-setup)
3. [Organizing the Project Structure](#organizing-the-project-structure)
4. [Creating the Book Model](#creating-the-book-model)
5. [Creating Service Interfaces](#creating-service-interfaces)
6. [Implementing the Services](#implementing-the-services)
7. [Registering Services in the DI Container](#registering-services-in-the-di-container)
8. [Creating the Controller](#creating-the-controller)
9. [Running and Testing the Application](#running-and-testing-the-application)
10. [Best Practices](#best-practices)
11. [Types of Dependency Injection](#types-of-dependency-injection)  
    11.1 [Constructor Injection](#constructor-injection)  
    11.2 [Property Injection](#property-injection)  
    11.3 [Method Injection](#method-injection)  
12. [Understanding Service Lifetimes](#understanding-service-lifetimes)  
    12.1 [Transient](#transient)  
    12.2 [Scoped](#scoped)  
    12.3 [Singleton](#singleton)
---

## Introduction

Dependency Injection (DI) is a core design pattern in ASP.NET Core. It allows developers to build decoupled, testable, and maintainable applications. This guide provides a hands-on, advanced implementation of DI in ASP.NET Core using only the .NET CLI. It is inspired by the in-depth walkthrough by [Ravi Patel.](https://medium.com/@ravipatel.it/dependency-injection-and-services-in-asp-net-core-a-comprehensive-guide-dd69858c1eab)

---

## Project Setup

Create a new ASP.NET Core Web API project:

```bash
dotnet new webapi -n LibraryAPI
cd LibraryAPI
```

This creates a basic API project preconfigured with Swagger, controllers, and the default DI container.

---

## Organizing the Project Structure

Create folders to separate concerns:

```bash
mkdir Models Services
```

---

## Creating the Book Model

**File:** `Models/Book.cs`

```csharp
namespace LibraryAPI.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
}
```

---

## Creating Service Interfaces

**File:** `Services/IBookService.cs`

```csharp
using LibraryAPI.Models;

namespace LibraryAPI.Services;

public interface IBookService
{
    IEnumerable<Book> GetAllBooks();
    Book GetBookById(int id);
    void AddBook(Book book);
    void DeleteBook(int id);
}
```

**File:** `Services/ILoggerService.cs`

```csharp
namespace LibraryAPI.Services;

public interface ILoggerService
{
    void Log(string message);
}
```

---

## Implementing the Services

**File:** `Services/BookService.cs`

```csharp
using LibraryAPI.Models;

namespace LibraryAPI.Services;

public class BookService : IBookService
{
    private readonly List<Book> _books = new()
    {
        new Book { Id = 1, Title = "1984", Author = "George Orwell" },
        new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee" }
    };

    public IEnumerable<Book> GetAllBooks() => _books;

    public Book GetBookById(int id) => _books.FirstOrDefault(b => b.Id == id);

    public void AddBook(Book book) => _books.Add(book);

    public void DeleteBook(int id)
    {
        var book = GetBookById(id);
        if (book != null) _books.Remove(book);
    }
}
```

**File:** `Services/LoggerService.cs`

```csharp
namespace LibraryAPI.Services;

public class LoggerService : ILoggerService
{
    public void Log(string message)
    {
        Console.WriteLine($"[LoggerService] {DateTime.Now}: {message}");
    }
}
```

---

## Registering Services in the DI Container

**Edit `Program.cs`:**

```csharp
using LibraryAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddSingleton<ILoggerService, LoggerService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## Creating the Controller

**File:** `Controllers/BooksController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using LibraryAPI.Models;
using LibraryAPI.Services;

namespace LibraryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILoggerService _loggerService;

    public BooksController(IBookService bookService, ILoggerService loggerService)
    {
        _bookService = bookService;
        _loggerService = loggerService;
    }

    [HttpGet]
    public IActionResult GetAllBooks()
    {
        _loggerService.Log("GET all books");
        return Ok(_bookService.GetAllBooks());
    }

    [HttpGet("{id}")]
    public IActionResult GetBookById(int id)
    {
        _loggerService.Log($"GET book with id: {id}");
        var book = _bookService.GetBookById(id);
        return book != null ? Ok(book) : NotFound();
    }

    [HttpPost]
    public IActionResult AddBook([FromBody] Book book)
    {
        _bookService.AddBook(book);
        _loggerService.Log($"Added book: {book.Title}");
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteBook(int id)
    {
        _bookService.DeleteBook(id);
        _loggerService.Log($"Deleted book with id: {id}");
        return NoContent();
    }
}
```

---

## Running and Testing the Application

Run the project using:

```bash
dotnet run
```

Open your browser and navigate to:

```
https://localhost:{PORT}/swagger
```

You can now interact with the API via Swagger UI.

---

## Best Practices

- **Use Constructor Injection**: Reliable and clear dependency provisioning.
- **Program to Interfaces**: Promote testability and flexibility.
- **Choose Proper Lifetimes**:
  - Singleton: For stateless shared services like logging.
  - Scoped: For services handling request-specific logic.
  - Transient: For lightweight, short-lived services.
- **Avoid the Service Locator Pattern**: Do not inject `IServiceProvider` just to resolve services.
- **Keep Controllers Thin**: Delegate logic to services, keeping controllers focused on HTTP concerns.


---

## Types of Dependency Injection

ASP.NET Core supports different types of Dependency Injection, each with its own use case:

### Constructor Injection
This is the most common and recommended form. Dependencies are provided through a class constructor.

```csharp
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }
}
```

### Property Injection
Dependencies are set via public properties. Useful in scenarios where constructor injection isn't feasible.

```csharp
public class BooksController : ControllerBase
{
    [FromServices]
    public IBookService BookService { get; set; }
}
```

### Method Injection
Dependencies are passed directly to methods, often through attributes like `[FromServices]`.

```csharp
[HttpGet]
public IActionResult GetAllBooks([FromServices] IBookService bookService)
{
    return Ok(bookService.GetAllBooks());
}
```

---

## Understanding Service Lifetimes

Service lifetimes determine how and when a service is instantiated:

### Transient
- A new instance is provided every time it's requested.
- Best for lightweight, stateless services.
```csharp
builder.Services.AddTransient<IMyService, MyService>();
```

### Scoped
- A single instance is used within a request.
- Ideal for services that work with scoped resources (e.g., database contexts).
```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

### Singleton
- A single instance is created and shared across the application lifetime.
- Great for shared, thread-safe services like logging.
```csharp
builder.Services.AddSingleton<IMyService, MyService>();
```

---

This guide reflects advanced, production-ready practices for DI in ASP.NET Core and is a strong foundation for scalable enterprise-grade APIs.

