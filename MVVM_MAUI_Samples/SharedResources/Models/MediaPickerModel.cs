namespace SharedResources.Models
{
    public class MediaPickerModel
    {
        public string PhotoPath { get; set; } = string.Empty;
        public string VideoPath { get; set; } = string.Empty;
        public bool ShowPhoto { get; set; } = false;
        public bool ShowVideo { get; set; } = false;
    }
}
