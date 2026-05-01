using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedResources.Models;

namespace MVVM_MAUI_Samples.ViewModels
{
    public partial class MediaPickerViewModel : ObservableObject
    {
        private readonly MediaPickerModel _model;

        public MediaPickerViewModel(MediaPickerModel model)
        {
            _model = model;
        }

        public string PhotoPath
        {
            get => _model.PhotoPath;
            set => SetProperty(_model.PhotoPath, value, _model, (m, v) => m.PhotoPath = v);
        }

        public string VideoPath
        {
            get => _model.VideoPath;
            set => SetProperty(_model.VideoPath, value, _model, (m, v) => m.VideoPath = v);
        }

        public bool ShowPhoto
        {
            get => _model.ShowPhoto;
            set => SetProperty(_model.ShowPhoto, value, _model, (m, v) => m.ShowPhoto = v);
        }

        public bool ShowVideo
        {
            get => _model.ShowVideo;
            set => SetProperty(_model.ShowVideo, value, _model, (m, v) => m.ShowVideo = v);
        }

        [RelayCommand]
        async Task PickPhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PickPhotoAsync THREW: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(IsCaptureSupported))]
        async Task CapturePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        [RelayCommand]
        async Task PickVideoAsync()
        {
            try
            {
                var video = await MediaPicker.PickVideoAsync();
                await LoadVideoAsync(video);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PickVideoAsync THREW: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(IsCaptureSupported))]
        async Task CaptureVideoAsync()
        {
            try
            {
                var video = await MediaPicker.CaptureVideoAsync();
                await LoadVideoAsync(video);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CaptureVideoAsync THREW: {ex.Message}");
            }
        }

        private bool IsCaptureSupported() => MediaPicker.IsCaptureSupported;

        async Task LoadPhotoAsync(FileResult photo)
        {
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }

            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
            {
                await stream.CopyToAsync(newStream);
            }

            PhotoPath = newFile;
            ShowVideo = false;
            ShowPhoto = true;
        }

        async Task LoadVideoAsync(FileResult video)
        {
            if (video == null)
            {
                VideoPath = null;
                return;
            }

            var newFile = Path.Combine(FileSystem.CacheDirectory, video.FileName);
            using (var stream = await video.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
            {
                await stream.CopyToAsync(newStream);
            }

            VideoPath = newFile;
            ShowVideo = true;
            ShowPhoto = false;
        }
    }
}
