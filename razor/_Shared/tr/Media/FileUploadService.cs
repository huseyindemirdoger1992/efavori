using Data;
using System;
using System.Collections.Generic;

namespace razor._Shared.tr.Media
{
    public class FileUploadService
    {
        // --- Mevcut Olaylar ---
        public event Action OnFileUploaded;
        public event Action<Data.Media> OnFileSelected; // Tekli seçim için

        // --- Yeni: Çoklu Seçim Olayı ---
        public event Action<List<Data.Media>> OnSelectionChanged;

        private Data.Media _selectedFile;
        // Seçilen dosyaları benzersiz tutmak için bir liste
        private List<Data.Media> _selectedFiles = new List<Data.Media>();

        public void NotifyFileUploaded()
        {
            OnFileUploaded?.Invoke();
        }

        // --- Tekli Seçim Metodu ---
        public void SelectFile(Data.Media media)
        {
            _selectedFile = media;
            OnFileSelected?.Invoke(media);
        }

        // --- Çoklu Seçim Metodu (Ekle/Kaldır Mantığı) ---
        public void ToggleFileSelection(Data.Media media)
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