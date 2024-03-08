using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Content
{
    public class WorkshopItem : IDisposable
    {
        public string Name, Description, Author;

        public string[] Tags;

        public int Votes, UpVotes, DownVotes;

        public long Views, Subscribers, Favorites;

        public float Rating;

        public string PreviewURL;

        public List<WorkshopItemPreview> AdditionalPreviews;

        public PublishedFileId_t[] Children;

        public CSteamID AuthorID;

        public PublishedFileId_t ItemID;

        public EWorkshopFileType ItemType;

        public string Folder;

        public float Size;

        public DateTime PostDate, UpdateDate;

        public bool InstallInfoError, PreviewURLError, GetChildrenError;

        private bool m_disposed;

        public bool IsDisposed()
        {
            return m_disposed;
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                if (AdditionalPreviews != null)
                    AdditionalPreviews.Clear();

                Name = null;
                Description = null;
                Author = null;
                Tags = null;
                Votes = 0;
                UpVotes = 0;
                Rating = 0f;
                Views = 0;
                Subscribers = 0;
                Favorites = 0;
                Size = 0f;
                Folder = null;
                PreviewURL = null;
                AdditionalPreviews = null;
                Children = null;
                AuthorID = default;
                ItemID = default;
                ItemType = default;
                InstallInfoError = false;
                PreviewURLError = false;
                GetChildrenError = false;
                PostDate = default;
                UpdateDate = default;
                m_disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~WorkshopItem()
        {
            Dispose();
        }
    }
}
