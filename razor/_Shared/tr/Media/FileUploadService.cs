using System;
using System.Collections.Generic;

namespace razor._Shared.tr.Media
{
    public class FileUploadService
    {
        // --- Mevcut Olaylar ---
        public event Action OnFileUploaded;
        public event Action<data._Galleries.Media> OnFileSelected; // Tekli seçim için

        // --- Yeni: Çoklu Seçim Olayı ---
        public event Action<List<data._Galleries.Media>> OnSelectionChanged;

        private data._Galleries.Media _selectedFile;
        // Seçilen dosyaları benzersiz tutmak için bir liste
        private List<data._Galleries.Media> _selectedFiles = new List<data._Galleries.Media>();

        public void NotifyFileUploaded()
        {
            OnFileUploaded?.Invoke();
        }

        // --- Tekli Seçim Metodu ---
        public void SelectFile(data._Galleries.Media media)
        {
            _selectedFile = media;
            OnFileSelected?.Invoke(media);
        }

        // --- Çoklu Seçim Metodu (Ekle/Kaldır Mantığı) ---
        public void ToggleFileSelection(data._Galleries.Media media)
        {
            if (_selectedFiles.Contains(media))
            {
                _selectedFiles.Remove(media);
            }
            else
            {
                _selectedFiles.Add(media);
            }

            // Dinleyenlere güncel listeyi gönder
            OnSelectionChanged?.Invoke(_selectedFiles);
        }

        // Seçilenleri temizlemek istersen:
        public void ClearSelection()
        {
            _selectedFiles.Clear();
            OnSelectionChanged?.Invoke(_selectedFiles);
        }
    }
}