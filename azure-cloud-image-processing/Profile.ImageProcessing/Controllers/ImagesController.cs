using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profile.ImageProcessing.Models;
using Profile.ImageProcessing.Services;
using System.Threading.Tasks;

namespace Profile.ImageProcessing.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ImageStoreServices imageStoreServices;
        public ImagesController(ImageStoreServices imageStoreServices)
        {
            this.imageStoreServices = imageStoreServices;
        }

        // GET: Images/Details/imageIdimageId
        [HttpGet("{imageId}")]
        public ActionResult Details(string imageId)
        {
            var model = new ShowModel { Uri = imageStoreServices.UriFor(imageId) };
            return View(model);
        }

        // GET: Images/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var imageId = await imageStoreServices.SaveImage(stream);
                        return RedirectToAction("Details", new { imageId });
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

    }
}