using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin365Demo.Models;

namespace Xamarin365Demo.Services
{
    public class ReviewsService
    {
        public static async Task<List<Review>> GetReviews()
        {
            return new List<Review>()
            {
                new Review() { Id = 1, Customer = "Robert F.", Rating = 4, Text = "Penso che il tuo prodotto sia fantastico!!!" },
                new Review() { Id = 2, Customer = "Michal R.", Rating = 2, Text = "It was quite expensive!" },
                new Review() { Id = 3, Customer = "Juan S.", Rating = 5, Text = "Es una cámara muy buena, tiene una calidad muy alta, tiene acceso a los lentes intercambiables, creo que de las réflex de sensor pequeño ésta es la mejor; en relación calidad - precio, no tiene comparación." },
                new Review() { Id = 4, Customer = "Luis B.", Rating = 3, Text = "I am experiencing some issues when using your product. It doesn't work at all." },
                new Review() { Id = 5, Customer = "Anne S.", Rating = 1, Text = "Diese Kamera hat keinen Mittelkontakt mehr für Blitze von vielen Drittanbietern! Ich kann deswegen nur von dieser Kamera abraten." },
            };
        }
    }
}
