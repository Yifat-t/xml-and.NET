using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml; 

namespace xmlBooks.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            IList<Models.Book> BookList = new List<Models.Book>();

            //load Books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();
            if(System.IO.File.Exists(path))
            {
                doc.Load(path);
                XmlNodeList Books = doc.GetElementsByTagName("book");

                foreach (XmlElement b in Books)
                {
                    Models.Book book = new Models.Book();
                    book.ID = int.Parse(b.GetElementsByTagName("id")[0].InnerText);
                    book.Title = b.GetElementsByTagName("title")[0].InnerText;
                    book.FirstName = b.GetElementsByTagName("firstname")[0].InnerText;       
                    book.LastName = b.GetElementsByTagName("lastname")[0].InnerText;

                    BookList.Add(book);
                }
            }
            return View(BookList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var book = new Models.Book();
            return View(book);
        }


        [HttpPost]
        public IActionResult Create(Models.Book b)
        {
            //load Books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                //if file exists, just load it and create new book
                doc.Load(path);

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);

                //get the root element
                doc.DocumentElement.AppendChild(book);
            }
            else
            {
                //file doesn't exist, so create it and create new book 
                XmlNode dec = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.AppendChild(dec);
                XmlNode root = doc.CreateElement("books");

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);
                root.AppendChild(book);
                doc.AppendChild(root);
            }
         
            doc.Save(path);

            return View();
        }
            

        private XmlElement _CreateBookElement(XmlDocument doc, Models.Book newBook)
        {
             
            XmlElement book = doc.CreateElement("book");
            
            XmlNode ID = doc.CreateElement("id");
            ID.InnerText = (GetLastBookID(doc) + 1).ToString("D4");
            book.AppendChild(ID);

            XmlNode Title = doc.CreateElement("title");
            Title.InnerText = newBook.Title;
            book.AppendChild(Title);

            XmlNode Author = doc.CreateElement("author");
            XmlNode FirstName = doc.CreateElement("firstname");
            FirstName.InnerText = newBook.FirstName;
            XmlNode LastName = doc.CreateElement("lastname");
            LastName.InnerText = newBook.LastName;


            Author.AppendChild(FirstName);
            Author.AppendChild(LastName);

            book.AppendChild(Author);

            return book;
        }

        private int GetLastBookID(XmlDocument doc)
        {
            XmlNode lastNode = doc.LastChild.LastChild;
            XmlNode n = lastNode.SelectSingleNode("id");
            return int.Parse(lastNode.SelectSingleNode("id").InnerText);
        }
    }
}
