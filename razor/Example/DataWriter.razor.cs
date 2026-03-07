using api;
using data;
using Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using razor._Shared;

namespace razor.Example
{
    /// <summary>
    /// VERİ GİRİŞİ YAPAN COMPONENT
    /// Veri ekleme/güncelleme/silme işlemlerini yapar ve tüm dinleyicilere sinyal gönderir
    /// </summary>
    public partial class DataWriter : ComponentBase, IAsyncDisposable
    {
        [Parameter] public Users? CurrentUser { get; set; }

        #region Services
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        [Inject] protected GlobalStateHub StateHub { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;


        #endregion

        #region State
        private readonly CancellationTokenSource _cts = new();
        private readonly SemaphoreSlim _writeLock = new(1, 1);
        private bool _disposed;
        private bool _isProcessing;
        #endregion

        #region Lifecycle
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            await _cts.CancelAsync();
            _cts.Dispose();
            _writeLock.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region UI Helpers

        private Notification? notificationRef;
        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            if (notificationRef is null) return;
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            await notificationRef.Launch(type, title, text, imageUrl);
        }

        #endregion

        #region CRUD Operations - Veri Giriş İşlemleri

        /// <summary>
        /// YENİ KULLANICI EKLE
        /// İşlem sonrası tüm dinleyicilere broadcast yapar
        /// </summary>
        protected async Task AddUser(Users newUser)
        {
            if (_isProcessing) return;

            await _writeLock.WaitAsync(_cts.Token);
            try
            {
                _isProcessing = true;
                StateHasChanged();

                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                newUser.RegistrationDate = DateTime.UtcNow;

                db.Users.Add(newUser);
                await db.SaveChangesAsync(_cts.Token);

                // ✅ KRITIK: Tüm dinleyicilere "Users tablosu güncellendi" sinyali gönder
                await StateHub.NotifyDataChanged("Users");

                await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
                await JS.InvokeVoidAsync("eval", $"$('#Modal_{null}').modal('hide')");

            }
            catch (Exception ex)
            {
                await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
            }
            finally
            {
                _isProcessing = false;
                _writeLock.Release();
                StateHasChanged();
            }
        }

        /// <summary>
        /// KULLANICI GÜNCELLE
        /// </summary>
        protected async Task UpdateUser(Users updatedUser)
        {
            if (_isProcessing) return;

            await _writeLock.WaitAsync(_cts.Token);
            try
            {
                _isProcessing = true;
                StateHasChanged();

                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                db.Users.Update(updatedUser);
                await db.SaveChangesAsync(_cts.Token);

                // ✅ BROADCAST: Tüm sayfalar güncellensin
                await StateHub.NotifyDataChanged("Users");

                await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
                await JS.InvokeVoidAsync("eval", $"$('#Modal_{null}').modal('hide')");
            }
            catch (Exception ex)
            {
                await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
            }
            finally
            {
                _isProcessing = false;
                _writeLock.Release();
                StateHasChanged();
            }
        }

        /// <summary>
        /// KULLANICI SİL
        /// </summary>
        protected async Task DeleteUser(int userId)
        {
            if (_isProcessing) return;

            await _writeLock.WaitAsync(_cts.Token);
            try
            {
                _isProcessing = true;
                StateHasChanged();

                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                var user = await db.Users.FindAsync(new object[] { userId }, _cts.Token);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync(_cts.Token);

                    // ✅ BROADCAST
                    await StateHub.NotifyDataChanged("Users");

                    await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
                    await JS.InvokeVoidAsync("eval", $"$('#Modal_{null}').modal('hide')");
                }
            }
            catch (Exception ex)
            {
                await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
            }
            finally
            {
                _isProcessing = false;
                _writeLock.Release();
                StateHasChanged();
            }
        }

        /// <summary>
        /// TOPLU VERİ IMPORT - Yüksek Performanslı
        /// 1000 kayıt ekleyip tek bir broadcast yapar
        /// </summary>
        protected async Task BulkImport(List<Users> users)
        {
            if (_isProcessing) return;

            await _writeLock.WaitAsync(_cts.Token);
            try
            {
                _isProcessing = true;
                StateHasChanged();

                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                var now = DateTime.UtcNow;
                foreach (var user in users)
                {
                    user.RegistrationDate = now;
                }

                // Batch insert (çok hızlı)
                db.Users.AddRange(users);
                await db.SaveChangesAsync(_cts.Token);

                // ✅ Tek bir broadcast - 1000 değil, 1 sinyal
                await StateHub.NotifyDataChanged("Users");

                await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
                await JS.InvokeVoidAsync("eval", $"$('#Modal_{null}').modal('hide')");
            }
            catch (Exception ex)
            {
                await ShowNotification("success-warning-danger", "XXX", $"XXXXXXXXXXXXXXXXXXXXXX", null);
            }
            finally
            {
                _isProcessing = false;
                _writeLock.Release();
                StateHasChanged();
            }
        }

        public async Task CopyText(string TextInfo)
        {
            // 1. İşlem devam ediyorsa yeni talebi reddet
            if (_isProcessing) return;

            // 2. Kilit mekanizmasını bekle (AddUser kültürü)
            await _writeLock.WaitAsync(_cts.Token);

            try
            {
                _isProcessing = true;
                StateHasChanged();

                // Metin boşsa kullanıcıyı bilgilendir ve çık
                if (string.IsNullOrEmpty(TextInfo))
                {
                    await ShowNotification("warning", "Uyarı", "Kopyalanacak içerik bulunamadı.", null);
                }
                else
                {
                    // Tarayıcı Clipboard API Entegrasyonu
                    await JS.InvokeVoidAsync("navigator.clipboard.writeText", TextInfo);

                    // Başarı bildirimi
                    await ShowNotification("success", "Başarılı", "İçerik başarıyla kopyalandı.", null);
                }
            }
            catch (Exception ex)
            {
                // Kültürdeki standart hata bildirimi
                await ShowNotification("danger", "Hata", $"Kopyalama işlemi başarısız: {ex.Message}", null);
            }
            finally
            {
                // 3. Kilidi serbest bırak ve UI'ı güncelle
                _isProcessing = false;
                _writeLock.Release();
                StateHasChanged();
            }
        }

        #endregion
    }
}