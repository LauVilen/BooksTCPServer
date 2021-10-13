using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BookLibrary;

namespace BooksTCPServer
{
    static class BooksManager
    {
        private static readonly List<Book> _books = new List<Book>
        {
            new Book(){ISBN13 = "9780609806951", Title = "Who Cooked the Last Supper?", Author = "Rosalind Miles", NumberOfPages = 352},
            new Book(){ISBN13 = "9781911344056", Title = "South of Forgiveness", Author = "Thordis Elva", NumberOfPages = 320},
            new Book(){ISBN13 = "9788702055023", Title = "Paula", Author = "Isabel Allende", NumberOfPages = 337}
        };
        private static Book falseBook = new Book("0", "no title", "no author", 0);

        // {"ISBN13":"9788772390130","Title":"Hannies Bog", "Author":"Lisa Wingate", "NumberOfPages": 457}
        // {"ISBN13":"9788771553277","Title":"Den Lille Prins", "Author":"Antoine de Saint-Exupéry", "NumberOfPages": 128}

        public static List<string> GetAllBooks()
        {
            List<string> booksAsJson = new List<string>();
            foreach (var book in _books)
            {
                booksAsJson.Add(JsonSerializer.Serialize(book));
            }
            return booksAsJson;
        }

        public static string GetBook(string isbn)
        {
            Book book = _books.Find(book => book.ISBN13 == isbn);
            if (book == null) return null;
            return JsonSerializer.Serialize(book);
        }

        //TODO: Find out why this is not working and FIX IT - why does "fromJsonBook.ISBN13 != null" break this shit???
        public static Book AddBook(string serializedBook)
        {
            Book fromJsonBook = null;
            try
            {
                fromJsonBook = JsonSerializer.Deserialize<Book>(serializedBook);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (fromJsonBook != null && !string.IsNullOrEmpty(fromJsonBook?.ISBN13))
            {
                if (!_books.Exists(book => fromJsonBook?.ISBN13 == book.ISBN13))
                {
                    _books.Add(fromJsonBook);
                    return fromJsonBook;
                }

                return falseBook;
            }

            return null;
        }

        public static Book DeleteBook(string isbn)
        {
            Book bookToDelete = _books.Find(book => book.ISBN13 == isbn);
            if (bookToDelete != null)
            {
                _books.Remove(bookToDelete);
                return bookToDelete;
            }
            return null;
        }

        public static Book UpdateBook(string isbn, string serializedBook)
        {
            Book bookToUpdate = _books.Find(book => book.ISBN13 == isbn);
            if (bookToUpdate != null)
            {
                Book updatedBook = null;
                try
                {
                    updatedBook = JsonSerializer.Deserialize<Book>(serializedBook);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (updatedBook != null)
                {
                    bookToUpdate.ISBN13 = updatedBook.ISBN13;
                    bookToUpdate.Title = updatedBook.Title;
                    bookToUpdate.Author = updatedBook.Author;
                    bookToUpdate.NumberOfPages = updatedBook.NumberOfPages;
                    return updatedBook;
                }
                return falseBook;
            }

            return null;
        }
    }
}
