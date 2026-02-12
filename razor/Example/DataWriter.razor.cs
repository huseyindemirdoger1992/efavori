using api;
using data;
using Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

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
        private async Task ShowSuccess(string message)
        {
            await JS.InvokeVoidAsync("alert", message);
        }

        private async Task ShowError(string message)
        {
            await JS.InvokeVoidAsync("alert", message);
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

                newUser.CreatedAt = DateTime.UtcNow;
                newUser.UpdatedAt = DateTime.UtcNow;

                db.Users.Add(newUser);
                await db.SaveChangesAsync(_cts.Token);

                // ✅ KRITIK: Tüm dinleyicilere "Users tablosu güncellendi" sinyali gönder
                await StateHub.NotifyDataChanged("Users");

                await ShowSuccess("Kullanıcı başarıyla eklendi");
            }
            catch (Exception ex)
            {
                await ShowError($"Hata: {ex.Message}");
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

                updatedUser.UpdatedAt = DateTime.UtcNow;

                db.Users.Update(updatedUser);
                await db.SaveChangesAsync(_cts.Token);

                // ✅ BROADCAST: Tüm sayfalar güncellensin
                await StateHub.NotifyDataChanged("Users");

                await ShowSuccess("Kullanıcı güncellendi");
            }
            catch (Exception ex)
            {
                await ShowError($"Hata: {ex.Message}");
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

                    await ShowSuccess("Kullanıcı silindi");
                }
            }
            catch (Exception ex)
            {
                await ShowError($"Hata: {ex.Message}");
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

                await ShowSuccess($"{users.Count} kullanıcı eklendi");
            }
            catch (Exception ex)
            {
                await ShowError($"Hata: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                _writeLock.Release();
                StateHasChanged();
            }
        }

        #endregion
    }
}