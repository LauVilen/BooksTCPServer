using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BookLibrary;

namespace BooksTCPServer
{
    public static class ClientHandler
    {
        public static void HandleClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            string message = InitiateClient(ns, reader, writer);

            while (message != "close")
            {
                switch (message)
                {
                    case "h":
                        AnswerHelp(ns, reader, writer);
                        break;
                    case "get all":
                        GetAll_ToClient(ns, reader, writer);
                        break;
                    case "get":
                        Get_ToClient(ns, reader, writer);
                        break;
                    case "save":
                        Add_FromClient(ns, reader, writer);
                        break;
                    case "delete":
                        Delete_FromClient(ns, reader, writer);
                        break;
                    case "update":
                        Update_FromClient(ns, reader, writer);
                        break;
                    default:
                        writer.WriteLine("No method matched the received input, please try again. \n\r");
                        break;
                }
                Console.WriteLine();
                message = InitiateClient(ns, reader, writer);
                
            }
            socket.Close();
            Console.WriteLine("Client closed the connection.");

        }

        private static string InitiateClient(NetworkStream ns, StreamReader reader, StreamWriter writer)
        {
            writer.WriteLine("Please state the type of request you wish to send the server. For help, please type: \"h\" \n\r");
            writer.Flush();
            string request = reader.ReadLine()?.ToLower();
            Console.WriteLine("Client wrote: " + request);
            return request;
        }

        public static void AnswerHelp(NetworkStream ns, StreamReader reader, StreamWriter writer)
        {
            string helpMessage = "Type: \"Get all\" to receive a list of all books in the library. \n\r" +
                                 "Type: \"Get\" to search for a specific book in the library. \n\r" +
                                 "Type: \"Save\" to save a new book to the library. \n\r" +
                                 "Type: \"Delete\" to delete a book from the library. \n\r" +
                                 "Type: \"Update\" to update an existing book in the library \n\r" +
                                 "To close the connection please type \"Close\" \n\r";

            writer.WriteLine(helpMessage);
            writer.Flush();
        }

        public static void GetAll_ToClient(NetworkStream ns, StreamReader reader, StreamWriter writer)
        {
            Console.WriteLine("List sent to the client: ");
            foreach (var book in BooksManager.GetAllBooks())
            {
                writer.WriteLine(book);
                writer.Flush();
                Console.WriteLine(book);
            }
            writer.WriteLine();
            writer.Flush();
            Console.WriteLine();
        }

        public static void Get_ToClient(NetworkStream ns, StreamReader reader, StreamWriter writer)
        {
            writer.WriteLine("Please write the ISBN13 of the book you wish to get from the library.");
            writer.Flush();
            string isbnToGet = reader.ReadLine();
            Console.WriteLine("Client wrote: " + isbnToGet);
            writer.WriteLine(BooksManager.GetBook(isbnToGet) + "\n\r");
            writer.Flush();
            Console.WriteLine();
        }

        public static void Add_FromClient(NetworkStream ns, StreamReader reader, StreamWriter writer)
        {
            writer.WriteLine("Please write the book you wish to save to the library, as a JSON string.");
            writer.Flush();
            string newBookAsJson = reader.ReadLine();
            Console.WriteLine("Client added the following book to the library: " + newBookAsJson);
            Book addedBook = BooksManager.AddBook(newBookAsJson);
            if (addedBook != null && addedBook.ISBN13 != "0")
            {
                writer.WriteLine("Book successfully saved to the library. \n\r");
                writer.Flush();
            }
            else if (addedBook.ISBN13 == "0")
            {
                writer.WriteLine("Error. Book was not added to the library due to a duplicate ISBN13. Please try again with a different ISBN13 \n\r");
                writer.Flush();
            }
            else
            {
                writer.WriteLine("Error. Book was not added to the library due to incorrect formatting of the received Json string. \n\r");
                writer.Flush();
            }
            Console.WriteLine();
        }

        public static void Delete_FromClient(NetworkStream ns, StreamReader reader, StreamWriter writer)
        {
            writer.WriteLine("Please write the ISBN13 of the book you wish to delete from the library.");
            writer.Flush();
            string isbnToDelete = reader.ReadLine();
            Console.WriteLine("Client deleted a book in the library with the following ISBN13: " + isbnToDelete);
            Book deletedBook = BooksManager.DeleteBook(isbnToDelete);
            if (deletedBook != null)
            {
                writer.WriteLine("Book successfully deleted. \n\r");
                writer.Flush();
            }
            else
            {
                writer.WriteLine("Error. The typed ISBN13 does not match any book in the library. No book was deleted. \n\r");
                writer.Flush();
            }
            Console.WriteLine();
        }

        public static void Update_FromClient(NetworkStream ns, StreamReader reader, StreamWriter writer)
        {
            writer.WriteLine("Please write the ISBN of the book you wish to update");
            writer.Flush();
            string isbnToUpdate = reader.ReadLine();
            Console.WriteLine("Client wishes to update the book with the following ISBN13: " + isbnToUpdate);
            writer.WriteLine($"Please write the new book you wish to override ISBN13 \"{isbnToUpdate}\" with, as a JSON string.");
            writer.Flush();
            string updateBookAsJson = reader.ReadLine();
            Console.WriteLine("Client has updated ISBN13: " + isbnToUpdate + "with the following book data: " + updateBookAsJson);
            Book updatedBook = BooksManager.UpdateBook(isbnToUpdate, updateBookAsJson);
            if (updatedBook !=null && updatedBook.ISBN13 != "0")
            {
                writer.WriteLine("Update successful. \n\r");
                writer.Flush();
            }
            else if (updatedBook.ISBN13 == "0")
            {
                writer.WriteLine("Error. Could not update the requested book due to incorrect formatting of the received Json string. \n\r");
                writer.Flush();
            }
            else
            {
                writer.WriteLine("Error. The typed ISBN13 does not match any book in the library. No books were updated. \n\r");
                writer.Flush();
            }

            Console.WriteLine();
        }
    }
}
