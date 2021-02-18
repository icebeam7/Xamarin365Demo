using System.Collections.Generic;

namespace Xamarin365Demo.Models
{
    public class ExcelData
    {
        public List<string> Headers { get; set; } = new List<string>();
        public List<List<string>> Values { get; set; } = new List<List<string>>();
    }
}
