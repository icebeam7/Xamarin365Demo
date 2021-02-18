using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin365Demo.Models;
using Xamarin365Demo.Services;

using Xamarin.Forms;

namespace Xamarin365Demo.ViewModels
{
    public class ReviewsViewModel : BaseViewModel
    {
        private ObservableCollection<Review> reviews;

        public ObservableCollection<Review> Reviews
        {
            get { return reviews; }
            set { reviews = value; OnPropertyChanged(); }
        }

        public Command AnalyzeCommand { get; set; }

        public ReviewsViewModel()
        {
            IsBusy = true;

            Task.Run(async () => await GetData());
            AnalyzeCommand = new Command(async () => await Analyze());

            IsBusy = false;
        }

        private async Task GetData()
        {
            var data = await ReviewsService.GetReviews();
            Reviews = new ObservableCollection<Review>(data);
        }

        private async Task Analyze()
        {
            IsBusy = true;

            var newReviews = await TextAnalyticsService.AnalyzeText(Reviews.ToList());

            if (newReviews != null)
                Reviews = new ObservableCollection<Review>(newReviews.OrderBy(x => x.Id));

            IsBusy = false;
        }
    }
}