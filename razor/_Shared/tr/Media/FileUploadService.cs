using System;
using System.Collections.Generic;
using System.Linq;

namespace razor._Shared.tr.Media
{
    /// <summary>
    /// Bileşenler arası (Upload ↔ List) medya seçim ve yükleme bildirimi köprüsü.
    ///
    /// Blazor Server'da devre (circuit) başına tek örnek yaşaması için
    /// <b>Scoped</b> kaydedilir:
    ///     builder.Services.AddScoped&lt;razor._Shared.tr.Media.FileUploadService&gt;();
    ///
    /// Not: Fiziksel yükleme artık <see cref="api.tr.AzureBlobService"/> +
    /// <see cref="api.tr.Media"/> üzerinden Azure Blob'a yapılır; bu sınıf yalnızca
    /// UI olay yönetiminden (seçim/temizleme/yenileme) sorumludur.
    /// </summary>
    public class FileUploadService
    {
        private readonly object _sync = new();

        // --- Olaylar ---
        public event Action? OnFileUploaded;
        public event Action<data._Galleries.Media>? OnFileSelected;                 // Tekli seçim
        public event Action<IReadOnlyList<data._Galleries.Media>>? OnSelectionChanged; // Çoklu seçim

        private data._Galleries.Media? _selectedFile;
        private readonly List<data._Galleries.Media> _selectedFiles = new();

        /// <summary>Son tekli seçim (varsa).</summary>
        public data._Galleries.Media? SelectedFile
        {
            get { lock (_sync) return _selectedFile; }
        }

        /// <summary>Güncel çoklu seçim listesinin kopyası.</summary>
        public IReadOnlyList<data._Galleries.Media> SelectedFiles
        {
            get { lock (_sync) return _selectedFiles.ToList(); }
        }

        /// <summary>Yeni dosya(lar) yüklendiğinde listenin yenilenmesi için tetiklenir.</summary>
        public void NotifyFileUploaded() => OnFileUploaded?.Invoke();

        // --- Tekli seçim ---
        public void SelectFile(data._Galleries.Media media)
        {
            if (media is null) return;
            lock (_sync) _selectedFile = media;
            OnFileSelected?.Invoke(media);
        }

        // --- Çoklu seçim (ekle/kaldır) ---
        public void ToggleFileSelection(data._Galleries.Media media)
        {
            if (media is null) return;

            IReadOnlyList<data._Galleries.Media> snapshot;
            lock (_sync)
            {
                // Aynı asset'i Id üzerinden benzersiz tut (referans farkı olsa da).
                var existing = _selectedFiles.FirstOrDefault(m => m.Id == media.Id);
                if (existing is not null)
                    _selectedFiles.Remove(existing);
                else
                    _selectedFiles.Add(media);

                snapshot = _selectedFiles.ToList();
            }

            OnSelectionChanged?.Invoke(snapshot);
        }

        /// <summary>Bir asset'in şu an seçili olup olmadığını Id üzerinden döndürür.</summary>
        public bool IsSelected(data._Galleries.Media media)
        {
            if (media is null) return false;
            lock (_sync) return _selectedFiles.Any(m => m.Id == media.Id);
        }

        // --- Seçimi temizle ---
        public void ClearSelection()
        {
            IReadOnlyList<data._Galleries.Media> snapshot;
            lock (_sync)
            {
                _selectedFiles.Clear();
                _selectedFile = null;
                snapshot = _selectedFiles.ToList();
            }
            OnSelectionChanged?.Invoke(snapshot);
        }
    }
}