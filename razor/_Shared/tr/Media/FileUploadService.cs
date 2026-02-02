using System;
using System.Collections.Generic;
using System.Text;

namespace razor._Shared.tr.Media
{
    public class FileUploadService
    {
        public event Action OnFileUploaded;

        public void NotifyFileUploaded()
        {
            OnFileUploaded?.Invoke();
        }
    }
}
