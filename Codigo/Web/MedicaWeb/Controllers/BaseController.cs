using Microsoft.AspNetCore.Mvc;

namespace MedicaWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        protected uint GetIdUserLogado()
        {
            var claimId = User.FindFirst("idUser")?.Value;

            if (uint.TryParse(claimId, out uint idCuidador))
            {
                return idCuidador;
            }

            return 0;
        }
    }
}