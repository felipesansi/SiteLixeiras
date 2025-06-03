using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace SiteLixeiras.Controllers
{
    public class CatalogoController : Controller
    {
        public IActionResult CatalogoCores()
        {
            var listaCores = new List<string>
    {
        "Preto", "Branco", "Fume", "Menta", "Azul céu", "Beringela",
        "Rosa itch", "Vermelho", "Coral", "Papaia","Hc28", "Hds29", "Hc31",
         "Hc35", "Hc53", "Hb77", "Hb50","Hf60", "Hk33", 
         "Hj70","He50"
    };

            return View(listaCores);
        }

    }
}
