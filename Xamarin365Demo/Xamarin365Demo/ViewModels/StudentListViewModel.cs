using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Graph;

using Xamarin.Forms;

using Xamarin365Demo.Models;
using Xamarin365Demo.Services;

namespace Xamarin365Demo.ViewModels
{
    public class StudentListViewModel : BaseViewModel
    {
        private ObservableCollection<Student> students;

        public ObservableCollection<Student> Students
        {
            get => students;
            set { students = value; OnPropertyChanged(); }
        }

        public ICommand ExcelExportCommand { private set; get; }
        private ExcelService excelService;

        public StudentListViewModel()
        {
            oneDriveFile = new DriveItem();
            excelService = new ExcelService();
            ExcelExportCommand = new Command(async () => await ExportToExcel());

            getData();
        }

        void getData()
        {
            var data = StudentService.GetStudents();
            Students = new ObservableCollection<Student>(data);
        }

        private DriveItem oneDriveFile;

        public DriveItem OneDriveFile
        {
            get { return oneDriveFile; }
            set { oneDriveFile = value; OnPropertyChanged(); }
        }

        async Task ExportToExcel()
        {
            if (App.IsSigned)
            {
                IsBusy = true;

                var localFile = await CreateExcel();
                OneDriveFile = await UploadFileToOneDrive(localFile);

                IsBusy = false;
            }
            else
            {

            }
        }

        async Task<string> CreateExcel()
        {
            var file = $"{Guid.NewGuid()}.xlsx";
            string filePath = excelService.CreateExcelFile(file);

            var headers = new List<string>() { "ID", "Code", "Name", "Faculty" };

            var data = new ExcelData();
            data.Headers = headers;

            foreach (var student in Students)
            {
                var row = new List<string>()
                {
                    student.Id.ToString(),
                    student.Code,
                    student.Name,
                    student.Faculty
                };

                data.Values.Add(row);
            }

            excelService.InsertDataInExcel(filePath, "Students", data);
            return filePath;

            /*
            await Launcher.OpenAsync(new OpenFileRequest()
            {
                File = new ReadOnlyFile(rutaArchivo)
            });*/
        }

        private async Task<DriveItem> UploadFileToOneDrive(string rutaArchivo)
        {
            string fileName = Path.GetFileName(rutaArchivo);
            Stream stream = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read);

            DriveItem oneDriveFile = await App.GraphClient.Me.Drive.Root
                .ItemWithPath(fileName).Content.Request().PutAsync<DriveItem>(stream);

            await sendMessage(oneDriveFile);

            return oneDriveFile;
        }

        private async Task sendMessage(DriveItem item)
        {
            var teams = await App.GraphClient.Me.JoinedTeams.Request().GetAsync();
            var team = teams.FirstOrDefault();
            var teamId = team.Id;

            var channels = await App.GraphClient.Teams[teamId]
                .Channels.Request().GetAsync();
            var channel = channels.FirstOrDefault();
            var channelId = channel.Id;

            var message = new ChatMessage()
            {
                Body = new ItemBody()
                {
                    ContentType = BodyType.Html,
                    Content = $"Your file is available at this location: {item.WebUrl}"
                }
            };

            var messages = await App.GraphClient.Teams[teamId]
                .Channels[channelId].Messages.Request().AddAsync(message);
        }
    }
}