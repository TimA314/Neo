﻿using Neo.Utilities;
using NostrNetTools.Nostr.Keys;

namespace Neo.Models
{
    public class Note : NotifyPropertyChangedBase
    {
        private string _displayName;
        private string _authorPublicKey;
        private string? _shortAuthorNpub;
        private string _profileImage = string.Empty;
        private string _banner = string.Empty;
        private string _about = string.Empty;
        private string _authorNpub = string.Empty;

        public required string Id { get; set; }
        public string AuthorPublicKey
        {
            get => _authorPublicKey;
            set
            {
                SetProperty(ref _authorPublicKey, value);
                ShortAuthorNpub = ShortenPublicKey(Keys.ConvertBech32ToNpub(value));
                DisplayName = string.IsNullOrEmpty(DisplayName) ? ShortAuthorNpub : DisplayName;
                _authorNpub = Keys.ConvertBech32ToNpub(value);
            }
        }
        public required string CreatedAt { get; set; }
        public required string Content { get; set; }
        public List<string>? Images { get; set; }
        public List<string>? Tags { get; set; }


        public string ShortAuthorNpub
        {
            get => _shortAuthorNpub;
            set => SetProperty(ref _shortAuthorNpub, value);
        }

        public string AuthorNpub
        {
            get => _authorNpub;
            set => SetProperty(ref _authorNpub, value);
        }

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

        private string ShortenPublicKey(string publicKey)
        {
            if (string.IsNullOrEmpty(publicKey)) return string.Empty;

            return publicKey.Length > 10 ? $"{publicKey[..6]}...{publicKey[^4..]}" : publicKey;
        }
    }
}
