using AmhPorterTest.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AmhPorterTest.Controllers
{
    public class SearchController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Search()
        {
            return View();
        }

        public IActionResult StartSearch(string Query)
        {
            SystemManager systemManager = new SystemManager();
            systemManager.TriggerCorpusPreprocessing();
            var result = systemManager.CommenceSearch(Query);
            return View("Search", result);
        }
    }
}
