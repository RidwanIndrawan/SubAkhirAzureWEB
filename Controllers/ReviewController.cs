using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BooksCatalogue.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksCatalogue.Controllers
{
    public class ReviewController : Controller
    {
        // private string apiEndpoint = "https://bookscatalogueapi-dicoding.azurewebsites.net/api/";
        private string apiEndpoint = "https://latihanazureapi.azurewebsites.net/api/";
        public string baseUrl = "https://latihanazureweb.azurewebsites.net/Books/Details/";
        public HttpClient _client;
        HttpClientHandler clientHandler = new HttpClientHandler();
        public ReviewController() {
            // Use this client handler to bypass ssl policy errors
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            _client = new HttpClient(clientHandler);
        }

        // GET: Review/AddReview/2
        public async Task<IActionResult> AddReview(int? bookId)
        {
            if (bookId == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint + "books/" + bookId);

            HttpResponseMessage response = await client.SendAsync(request);

            switch(response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string responseString = await response.Content.ReadAsStringAsync();
                    var book = JsonSerializer.Deserialize<Book>(responseString);

                    ViewData["BookId"] = bookId;
                    return View("Add");
                case HttpStatusCode.NotFound:
                    return NotFound();
                default:
                    return ErrorAction("Error. Status code = " + response.StatusCode + ": " + response.ReasonPhrase);
            }
        }

        // TODO: Tambahkan fungsi ini untuk mengirimkan atau POST data review menuju API
        // POST: Review/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview([Bind("Id,BookId,ReviewerName,Rating,Comment")] Review review)
        {

             MultipartFormDataContent content = new MultipartFormDataContent();

                content.Add(new StringContent(review.BookId.ToString()), "bookId");
                content.Add(new StringContent(review.ReviewerName), "reviewerName");
                content.Add(new StringContent(review.Rating.ToString()), "rating");
                content.Add(new StringContent(review.Comment), "comment");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "review/");
                request.Content = content;
                HttpResponseMessage response = await _client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Created:
                       int bookids = review.BookId;
                        return Redirect(baseUrl + bookids);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode + "; " + response.ReasonPhrase);
                }
        }

 public async Task<IActionResult> Delete(int? id)
        {
             
            if (id == null)
            {
                return NotFound();
            }
             _client = new HttpClient(clientHandler);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint + "review/" + id);

            HttpResponseMessage response = await _client.SendAsync(request);

            switch(response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string responseString = await response.Content.ReadAsStringAsync();
                    var book = JsonSerializer.Deserialize<Review>(responseString);

                    
                    return View(book);
                default:
                    return ErrorAction("Error. Status code = " + response.StatusCode + ": " + response.ReasonPhrase);
            }
        }

        [HttpPost, ActionName("DeleteReview")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            _client = new HttpClient(clientHandler);
        
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiEndpoint + "review/" + id);
            
            HttpResponseMessage response = await _client.SendAsync(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                    return Redirect("https://localhost:8000/api/books/");
                case HttpStatusCode.Unauthorized:
                    return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                default:
                    return ErrorAction("Error. Status code = " + response.StatusCode );
            }
        }
        private ActionResult ErrorAction(string message)
        {
            return new RedirectResult("/Home/Error?message=" + message);
        }
    }
}