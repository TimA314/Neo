using Neo.Utilities;
using System.ComponentModel;

namespace Neo.Models
{
    public class Note : NotifyPropertyChangedBase
    {
        private string _displayName = string.Empty;
        private string _profileImage = string.Empty;
        private string _banner = string.Empty;
        private string _about = string.Empty;

        public string? Id { get; set; }
        public string? AuthorPublicKey { get; set; }
        public string? CreatedAt { get; set; }
        public string? Content { get; set; }
        public List<string>? Images { get; set; }
        public List<string>? Tags { get; set; }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public string ProfileImage
        {
            get => _profileImage;
            set => SetProperty(ref _profileImage, value);
        }

        public string Banner
        {
            get => _banner;
            set => SetProperty(ref _banner, value);
        }

        public string About
        {
            get => _about;
            set => SetProperty(ref _about, value);
        }
    }
}
