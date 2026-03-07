using api;
using api.tr;
using data;
using data._Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace razor._Shared.tr.Pages.Public.TaskBoard
{
    public partial class CentralSystemTaskBoard
    {
        [Parameter] public Users? use { get; set; }
        [Parameter] public string? TaskCategoriesValue { get; set; }
        [Parameter] public Guid? TaskFrameworkId { get; set; }
        #region Services
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        [Inject] protected NavigationManager Navigation { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;
        [Inject] protected TakeLogs Logger { get; init; } = default!;
        [Inject] protected EmailSender ApiEmailSender { get; init; } = default!;
        [Inject] protected TakeLogs ApiTakeLogs { get; init; } = default!;
        [Inject] protected UserInfos ApiUserInfos { get; init; } = default!;
        #endregion
        #region State Management
        private Notification? notificationRef;
        private readonly CancellationTokenSource _cts = new();
        private readonly SemaphoreSlim _dbLock = new(1, 1);
        private bool _disposed;
        #endregion
        #region Data
        _ApplicationConnectionDb db = new _ApplicationConnectionDb();
        private List<Users> adminUsers = new();
        private List<TaskCategories> taskCategories = new();
        private List<data.TaskStatus> tasks = new();
        private List<TaskNotes> taskNotes = new();
        private Guid? draggedTaskId;
        private bool AddTaskshowModal = false;
        private bool EditTaskshowModal = false;
        private bool showDetailModal = false;
        private bool showNoteEditlModal = false;
        private data.TaskCategories? TaskCategories = new();
        private data.TaskStatus? AddTask;
        private data.TaskStatus? editingTask;
        private data.TaskStatus? selectedTask;
        private data.TaskNotes? Task_Notes;
        private DateTime? targetDate = DateTime.Now.AddDays(7);
        private DateTime? editTargetDate;
        private string CategoryStructure = "";
        private string selectedPersonInCharge = "";
        private string editSelectedPersonInCharge = "";
        private string newNote = "";
        #endregion
        #region Lifecycle
        [Inject] protected GlobalStateHub StateHub { get; init; } = default!;
        private bool _isInitialized;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            // EĞER HERHANGİ BİR MODAL AÇIKSA YENİLEME YAPMA
            if (AddTaskshowModal || EditTaskshowModal || showNoteEditlModal) return;
            StateHub.OnDataChanged += HandleGlobalDataChange;
            // İlk veri yükleme

            _isInitialized = true;
        }
        private async Task HandleGlobalDataChange(string entityName)
        {
            if (_disposed || !_isInitialized) return;
            if (AddTaskshowModal || EditTaskshowModal || showNoteEditlModal) return;


            // Sadece ilgili entity'ler için güncelle
            var shouldRefresh = entityName == "CentralSystemTaskBoard";

            if (shouldRefresh)
            {
                // ✅ InvokeAsync: UI thread-safe güncelleme
                await InvokeAsync(async () =>
                {
                    await LoadData();
                    StateHasChanged(); // UI'ı yenile
                });
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            await _cts.CancelAsync();
            _cts.Dispose();
            _dbLock.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Button Management
        private readonly Dictionary<string, bool> _processingStates = new();
        protected bool IsButtonProcessing(string key) => _processingStates.GetValueOrDefault(key, false);
        private async Task<bool> RunWithStateAsync(string key, Func<Task> action, bool useDbLock = false)
        {
            if (IsButtonProcessing(key)) return false;
            if (useDbLock) await _dbLock.WaitAsync(_cts.Token);
            try
            {
                _processingStates[key] = true;
                StateHasChanged();
                await action();
                return true;
            }
            catch (OperationCanceledException) { return false; }
            catch (Exception ex)
            {
                await LogError(key, ex);
                if (useDbLock) await ShowNotification("error", "Hata", "İşlem başarısız oldu.", null);
                return false;
            }
            finally
            {
                _processingStates[key] = false;
                if (useDbLock) _dbLock.Release();
                StateHasChanged();
            }
        }
        private async Task<bool> ExecuteWithLock(string key, Func<Task> action)
        {
            return await RunWithStateAsync(key, action, useDbLock: true);
        }
        #endregion
        #region Helpers
        private async Task LogError(string action, Exception ex)
        {
            try
            {
                await Logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: GetType().Name,
                    action: action,
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
            catch { }
        }
        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            if (notificationRef is null) return;
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            await notificationRef.Launch(type, title, text, imageUrl);
        }
        private string GetUserName(Guid? userId)
        {
            if (userId == null) return "Atanmamış";
            var user = adminUsers.FirstOrDefault(u => u.Id == userId);
            return user != null ? $"{user.FirstName} {user.LastName}" : "Bilinmiyor";
        }
        private string GetStatusText(string? status)
        {
            return status switch
            {
                "UnSuccessful" => " Başarısız/Ertelenmiş",
                "NewTasks" => "Yeni Görevler",
                "ToBeDone" => "Yapılacak",
                "InProcess" => "İşlemde",
                "InEditing" => "Düzenlemede",
                "Completed" => "Tamamlandı",
                _ => "Bilinmiyor"
            };
        }
        private string GetCategoryText(Guid? id)
        {
            if (id == null) return "Bilinmiyor";
            var category = taskCategories.FirstOrDefault(c => c.Id == id);
            return category?.Title ?? "Bilinmiyor";
        }
        private string GetStatusBadgeClass(string? status)
        {
            return status switch
            {
                "ToBeDone" => "bg-light text-light",
                "InProcess" => "bg-primary text-dark",
                "InEditing" => "bg-info text-dark",
                "Completed" => "bg-success text-light",
                _ => "bg-light  text-light"
            };
        }
        private string GetPriorityBadgeClass(string? priority)
        {
            return priority switch
            {
                "Yüksek" => "bg-danger text-dark",
                "Orta" => "bg-warning text-dark",
                "Düşük" => "bg-info text-dark",
                _ => "bg-secondary"
            };
        }
        private int GetTaskNotesCount(Guid taskId)
        {
            return taskNotes.Count(n => n.TaskStatusId == taskId &&
                                       (n.IsDeleted == null || n.IsDeleted.IsDeletedStatu != true));
        }
        private int IsOkGetTaskNotesCount(Guid taskId)
        {
            return taskNotes.Count(n => n.TaskStatusId == taskId &&
                                       (n.IsDeleted == null || n.IsDeleted.IsDeletedStatu != true) &&
                                        n.IsTheNoteOk == true);
        }
        private int IsNotOkGetTaskNotesCount(Guid taskId)
        {
            return taskNotes.Count(n => n.TaskStatusId == taskId &&
                                       (n.IsDeleted == null || n.IsDeleted.IsDeletedStatu != true) &&
                                        n.IsTheNoteOk != true);
        }
        #endregion
        #region Data Operations

        private string? _SearchText;
        private CancellationTokenSource? _searchDebounce;

        private void OnSearchInput(ChangeEventArgs e)
        {
            _SearchText = e.Value?.ToString();
            StateHasChanged(); // UI'ı anında güncelle, yazma gecikmesi olmaz

            _searchDebounce?.Cancel();
            _searchDebounce?.Dispose();
            _searchDebounce = new CancellationTokenSource();
            var token = _searchDebounce.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(1000, token);
                    if (!token.IsCancellationRequested)
                    {
                        await InvokeAsync(async () =>
                        {
                            await LoadData();
                            StateHasChanged();
                        });
                    }
                }
                catch (TaskCanceledException) { }
            });
        }
        protected async Task LoadData()
        {
            try
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                // Kullanıcı tipi kontrolü
                bool isAdminOrSuperAdmin = use?.UsersType == "Admin" || use?.UsersType == "SuperAdmin";
                bool isEmployee = use?.UsersType == "Employee";

                if (use == null || use.Id == Guid.Empty)
                    return;

                // Temel TaskStatus sorgusu - IsDeleted filtresi
                var query = db.TaskStatus
                    .Where(t => t.IsDeleted == null || t.IsDeleted.IsDeletedStatu != true);

                // Kullanıcı tipine göre TaskStatus bazlı filtreleme
                if (isAdminOrSuperAdmin)
                    query = query.Where(t => t.AssignedByUserId == use.Id);
                else if (isEmployee)
                    query = query.Where(t => t.PersonInChargeUserId == use.Id);

                // TaskCategories için temel filtre (her zaman use.Id ile kısıtlanır)
                var baseCategoryFilter = db.TaskCategories
                    .Where(c =>
                        (c.IsDeleted == null || c.IsDeleted.IsDeletedStatu != true) &&
                        c.UserId == use.Id &&
                        // Bağlı TaskFramework silinmemişse listele
                        (c.TaskFrameworkId == null ||
                         db.TaskFramework
                             .Any(f => f.Id == c.TaskFrameworkId &&
                                       (f.IsDeleted == null || f.IsDeleted.IsDeletedStatu != true))));

                if (TaskFrameworkId.HasValue && TaskFrameworkId.Value != Guid.Empty)
                {
                    taskCategories = await baseCategoryFilter
                        .Where(c => c.TaskFrameworkId == TaskFrameworkId)
                        .OrderByDescending(t => t.CreatedAt)
                        .AsNoTracking()
                        .ToListAsync(_cts.Token);

                    var tasksQuery = query
                        .AsNoTracking()
                        .Where(t => baseCategoryFilter
                            .Any(c => c.Id == t.TaskCategoriesId &&
                                      c.TaskFrameworkId == TaskFrameworkId));

                    if (!string.IsNullOrWhiteSpace(_SearchText))
                        tasksQuery = tasksQuery.Where(t =>
                            t.TaskTitle.Contains(_SearchText) ||
                            t.TaskDescription.Contains(_SearchText));

                    tasks = await tasksQuery
                        .OrderByDescending(t => t.DateCreatedAt)
                        .ToListAsync(_cts.Token);
                }
                else
                {
                    // CategoryStructure filtresini belirle (null → "All" gibi davranır)
                    bool filterByStructure = TaskCategoriesValue != "All" && !string.IsNullOrEmpty(TaskCategoriesValue);

                    taskCategories = await baseCategoryFilter
                        .Where(c => !filterByStructure || c.CategoryStructure == TaskCategoriesValue)
                        .OrderByDescending(t => t.CreatedAt)
                        .AsNoTracking()
                        .ToListAsync(_cts.Token);

                    var tasksQuery = query
                        .AsNoTracking()
                        .Where(t => baseCategoryFilter
                            .Any(c => c.Id == t.TaskCategoriesId &&
                                      (!filterByStructure || c.CategoryStructure == TaskCategoriesValue)));

                    if (!string.IsNullOrWhiteSpace(_SearchText))
                        tasksQuery = tasksQuery.Where(t =>
                            t.TaskTitle.Contains(_SearchText) ||
                            t.TaskDescription.Contains(_SearchText));

                    tasks = await tasksQuery
                        .OrderByDescending(t => t.DateCreatedAt)
                        .ToListAsync(_cts.Token);
                }

                taskNotes = await db.TaskNotes
                    .AsNoTracking()
                    .Where(n => n.IsDeleted == null || n.IsDeleted.IsDeletedStatu != true)
                    .OrderByDescending(t => t.NoteCreatedAt)
                    .ToListAsync(_cts.Token);

                await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                await LogError(nameof(LoadData), ex);
            }
        }

        private List<data.TaskStatus> GetTasksByStatus(string status, Guid? TaskCategoriesId, Guid? TaskFramewokId)
        {
            if (TaskCategoriesId != null)
            {
                return tasks.Where(t => t.Status == status && t.TaskCategoriesId == TaskCategoriesId).ToList();
            }
            else
            {
                return tasks.Where(t => t.Status == status).ToList();
            }
        }
        private int GetTaskCountByStatus(string status, Guid? TaskCategoriesId)
        {
            if (TaskCategoriesId == null)
            {
                return tasks.Count(t => t.Status == status);
            }
            else
            {
                return tasks.Count(t => t.Status == status && t.TaskCategoriesId == TaskCategoriesId);
            }
        }
        #endregion
        #region Drag and Drop
        private void HandleDragStart(Guid taskId)
        {
            draggedTaskId = taskId;
        }
        private async Task HandleDrop(string newStatus)
        {
            if (use != null && (use.UsersType == "Employee" || use.UsersType == "Admin" || use.UsersType == "SuperAdmin"))
            {
                if (draggedTaskId == null) return;
                await ExecuteWithLock(nameof(HandleDrop), async () =>
                {
                    await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                    var task = await db.TaskStatus.FirstOrDefaultAsync(t => t.Id == draggedTaskId, _cts.Token);
                    if (task == null) return;
                    if (task.Status != newStatus)
                    {
                        task.Status = newStatus;
                        switch (newStatus)
                        {
                            case "NewTasks":
                                task.DateToBeDone = DateTime.Now;
                                break;
                            case "ToBeDone":
                                task.DateToBeDone = DateTime.Now;
                                break;
                            case "InProcess":
                                task.DateInProcess = DateTime.Now;
                                break;
                            case "InEditing":
                                task.DateInEditing = DateTime.Now;
                                break;
                            case "Completed":
                                task.DateCompleted = DateTime.Now;
                                break;
                        }
                        db.TaskStatus.Update(task);
                        await db.SaveChangesAsync(_cts.Token);
                        await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                        await ShowNotification("success", "Başarılı", $"Görev '{GetStatusText(newStatus)}' durumuna taşındı.", null);
                        await LoadData();
                    }

                });
            }
            else
            {
                await ShowNotification("danger", "Hata", $"Görev yetki dışı olduğundan gerçekleştirilmedi", null);
            }
            draggedTaskId = null;
        }
        private async Task HandleDropCate(Guid NewCate)
        {
            if (use != null && (use.UsersType == "Employee" || use.UsersType == "Admin" || use.UsersType == "SuperAdmin"))
            {
                if (draggedTaskId == null) return;
                await ExecuteWithLock(nameof(HandleDrop), async () =>
                {
                    await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                    var task = await db.TaskStatus.FirstOrDefaultAsync(t => t.Id == draggedTaskId, _cts.Token);
                    if (task == null) return;
                    if (task.TaskCategoriesId != NewCate)
                    {
                        var CateName = await db.TaskCategories.FirstOrDefaultAsync(t => t.Id == NewCate, _cts.Token);

                        task.TaskCategoriesId = NewCate;
                        db.TaskStatus.Update(task);
                        await db.SaveChangesAsync(_cts.Token);
                        await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                        await ShowNotification("success", "Başarılı", $"Görev '{CateName.Title}' kategorisine taşındı.", null);
                        await LoadData();
                    }

                });
            }
            else
            {
                await ShowNotification("danger", "Hata", $"Görev yetki dışı olduğundan gerçekleştirilmedi", null);
            }
            draggedTaskId = null;
        }
        private async Task HandleDropPri(string NewPri)
        {
            if (use != null && (use.UsersType == "Employee" || use.UsersType == "Admin" || use.UsersType == "SuperAdmin"))
            {
                if (draggedTaskId == null) return;
                await ExecuteWithLock(nameof(HandleDrop), async () =>
                {
                    await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                    var task = await db.TaskStatus.FirstOrDefaultAsync(t => t.Id == draggedTaskId, _cts.Token);
                    if (task == null) return;
                    if (task.Priority != NewPri)
                    {
                        task.Priority = NewPri;
                        db.TaskStatus.Update(task);
                        await db.SaveChangesAsync(_cts.Token);
                        await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                        await ShowNotification("success", "Başarılı", $"Görev önceliği '{NewPri}' olarak değiştirildi.", null);
                        // await LoadData();
                    }

                });
            }
            else
            {
                await ShowNotification("danger", "Hata", $"Görev yetki dışı olduğundan gerçekleştirilmedi", null);
            }
            draggedTaskId = null;
        }
        #endregion
        #region Modal Operations
        private void ShowAddTaskModal()
        {
            AddTask = new data.TaskStatus
            {
                Id = Guid.NewGuid(),
                Status = "NewTasks",
                Priority = "Orta",
                AssignedByUserId = use?.Id,
                PersonInChargeUserId = use?.Id,
                DateCreatedAt = DateTime.Now,
                DateToBeDone = DateTime.Now
            };
            targetDate = DateTime.Now.AddDays(7);
            selectedPersonInCharge = "";
            AddTaskshowModal = true;
        }
        private void EditTask(data.TaskStatus task)
        {
            editingTask = new data.TaskStatus
            {
                Id = task.Id,
                TaskTitle = task.TaskTitle,
                TaskDescription = task.TaskDescription,
                Priority = task.Priority,
                Status = task.Status,
                TaskCategoriesId = task.TaskCategoriesId, // Bunu da ekleyin
                TargetDate = task.TargetDate,
                PersonInChargeUserId = task.PersonInChargeUserId,
                AssignedByUserId = task.AssignedByUserId,
                DateCreatedAt = task.DateCreatedAt,
                DateToBeDone = task.DateToBeDone,
                DateInProcess = task.DateInProcess,
                DateInEditing = task.DateInEditing,
                DateCompleted = task.DateCompleted,

                // ✅ Email bildirim flag'lerini ekleyin
                IsNew = task.IsNew,
                IsToBeDone = task.IsToBeDone,
                IsInProgress = task.IsInProgress,
                IsInEditing = task.IsInEditing,
                IsCompleted = task.IsCompleted
            };

            editTargetDate = task.TargetDate ?? DateTime.Now.AddDays(7);
            editSelectedPersonInCharge = task.PersonInChargeUserId?.ToString() ?? "";
            showDetailModal = false;
            EditTaskshowModal = true;
        }
        private void CloseAddModal()
        {
            AddTaskshowModal = false;
            AddTask = null;
        }
        private void CloseEditModal()
        {
            EditTaskshowModal = false;
            editingTask = null;
        }
        private void CloseModal()
        {
            CloseAddModal();
            CloseEditModal();
        }
        private async Task ShowTaskDetail(data.TaskStatus task)
        {
            selectedTask = task;
            await LoadTaskNotes(task.Id);
            showDetailModal = true;
        }
        private void CloseDetailModal()
        {
            showDetailModal = false;
            selectedTask = null;
            newNote = "";
        }
        private async Task LoadTaskNotes(Guid taskId)
        {
            try
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                taskNotes = await db.TaskNotes
                    .AsNoTracking()
                    .Where(n => n.TaskStatusId == taskId && (n.IsDeleted == null || n.IsDeleted.IsDeletedStatu != true))
                    .ToListAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                await LogError(nameof(LoadTaskNotes), ex);
            }
        }
        #endregion
        #region CRUD Operations

        private TaskFramework tf = new TaskFramework();
        private async Task SaveNewTask()
        {
            if (AddTask == null) return;
            await ExecuteWithLock("save", async () =>
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                if (!string.IsNullOrEmpty(selectedPersonInCharge) && Guid.TryParse(selectedPersonInCharge, out var personId))
                {
                    AddTask.PersonInChargeUserId = personId;
                }
                if (targetDate.HasValue)
                {
                    AddTask.TargetDate = targetDate.Value;
                }
                if (string.IsNullOrEmpty(AddTask.TaskTitle) || string.IsNullOrEmpty(AddTask.TaskDescription) || AddTask.TaskCategoriesId == null)
                {
                    await ShowNotification("danger", "Hata", "Boş alan bırakmayınız.", null);
                }
                else
                {
                    db.TaskStatus.Add(AddTask);
                    await db.SaveChangesAsync(_cts.Token);
                    await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                    await ShowNotification("success", "Başarılı", "Görev oluşturuldu.", null);
                    await LoadData();
                    CloseAddModal();
                }
            });
        }
        private async Task SaveEditTask()
        {
            if (editingTask == null) return;
            await ExecuteWithLock("edit", async () =>
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                var existingTask = await db.TaskStatus.FirstOrDefaultAsync(t => t.Id == editingTask.Id, _cts.Token);
                if (existingTask != null)
                {
                    existingTask.TaskTitle = editingTask.TaskTitle;
                    existingTask.TaskDescription = editingTask.TaskDescription;
                    existingTask.Priority = editingTask.Priority;
                    existingTask.Status = editingTask.Status;
                    if (!string.IsNullOrEmpty(editSelectedPersonInCharge) && Guid.TryParse(editSelectedPersonInCharge, out var personId))
                    {
                        existingTask.PersonInChargeUserId = personId;
                    }
                    if (editTargetDate.HasValue)
                    {
                        existingTask.TargetDate = editTargetDate.Value;
                    }
                    switch (editingTask.Status)
                    {
                        case "NewTasks":
                            if (existingTask.DateToBeDone == null)
                                existingTask.DateToBeDone = DateTime.Now;
                            break;
                        case "ToBeDone":
                            if (existingTask.DateToBeDone == null)
                                existingTask.DateToBeDone = DateTime.Now;
                            break;
                        case "InProcess":
                            if (existingTask.DateInProcess == null)
                                existingTask.DateInProcess = DateTime.Now;
                            break;
                        case "InEditing":
                            if (existingTask.DateInEditing == null)
                                existingTask.DateInEditing = DateTime.Now;
                            break;
                        case "Completed":
                            if (existingTask.DateCompleted == null)
                                existingTask.DateCompleted = DateTime.Now;
                            break;
                    }
                    existingTask.IsNew = editingTask.IsNew ?? false;
                    existingTask.IsToBeDone = editingTask.IsToBeDone ?? false;
                    existingTask.IsInProgress = editingTask.IsInProgress ?? false;
                    existingTask.IsInEditing = editingTask.IsInEditing ?? false;
                    existingTask.IsCompleted = editingTask.IsCompleted ?? false;
                    existingTask.TaskCategoriesId = editingTask.TaskCategoriesId;
                    db.TaskStatus.Update(existingTask);
                    await db.SaveChangesAsync(_cts.Token);
                    await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                    await ShowNotification("success", "Başarılı", "Görev güncellendi.", null);
                    await LoadData();
                    CloseEditModal();
                }
            });
        }
        private async Task DeleteTask()
        {
            if (selectedTask == null) return;
            await ExecuteWithLock("delete", async () =>
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                var task = await db.TaskStatus.FirstOrDefaultAsync(t => t.Id == selectedTask.Id, _cts.Token);
                if (task != null)
                {
                    // Soft delete - görev
                    if (task.IsDeleted == null)
                    {
                        task.IsDeleted = new IsDeleted();
                    }
                    task.IsDeleted.IsDeletedStatu = true;
                    task.IsDeleted.DeletedAtDate = DateTime.Now;
                    // Soft delete - ilişkili notlar
                    var notes = await db.TaskNotes.Where(n => n.TaskStatusId == task.Id).ToListAsync(_cts.Token);
                    foreach (var note in notes)
                    {
                        if (note.IsDeleted == null)
                        {
                            note.IsDeleted = new IsDeleted();
                        }
                        note.IsDeleted.IsDeletedStatu = true;
                        note.IsDeleted.DeletedAtDate = DateTime.Now;
                    }
                    db.TaskStatus.Update(task);
                    db.TaskNotes.UpdateRange(notes);
                    await db.SaveChangesAsync(_cts.Token);
                    await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                    await ShowNotification("success", "Başarılı", "Görev silindi.", null);
                    await LoadData();
                    CloseDetailModal();
                }
            });
        }
        private async Task AddNote()
        {
            if (selectedTask == null) return;
            await ExecuteWithLock("addNote", async () =>
            {
                if (string.IsNullOrEmpty(newNote) || string.IsNullOrWhiteSpace(newNote))
                {
                    await ShowNotification("danger", "Hata", "Not alanı boş olduğu için eklenemedi.", null);
                }
                else
                {
                    await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                    var note = new TaskNotes
                    {
                        Id = Guid.NewGuid(),
                        TaskStatusId = selectedTask.Id,
                        UserId = use?.Id ?? Guid.NewGuid(),
                        Note = newNote,
                        NoteCreatedAt = DateTime.Now,
                        IsTheNoteOk = false
                    };
                    db.TaskNotes.Add(note);
                    await db.SaveChangesAsync(_cts.Token);
                    await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                    await ShowNotification("success", "Başarılı", "Not eklendi.", null);
                    await LoadTaskNotes(selectedTask.Id);
                    newNote = "";
                }
            });
        }
        private async Task DeleteNote(Guid noteId)
        {
            await ExecuteWithLock("deleteNote", async () =>
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                var note = await db.TaskNotes.FirstOrDefaultAsync(n => n.Id == noteId, _cts.Token);
                if (note != null)
                {
                    // Soft delete işlemi
                    if (note.IsDeleted == null)
                    {
                        note.IsDeleted = new IsDeleted();
                    }
                    note.IsDeleted.IsDeletedStatu = true;
                    note.IsDeleted.DeletedAtDate = DateTime.Now;
                    db.TaskNotes.Update(note);
                    await db.SaveChangesAsync(_cts.Token);
                    await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                    await ShowNotification("success", "Başarılı", "Not silindi.", null);
                    if (selectedTask != null)
                    {
                        await LoadTaskNotes(selectedTask.Id);
                    }
                }
            });
        }
        private async Task ToggleNoteCompletion(Guid noteId, bool isCompleted)
        {
            // Önce yerel listeyi güncelle (UI sırası bozulmasın)
            var localNote = taskNotes.FirstOrDefault(n => n.Id == noteId);
            if (localNote != null)
            {
                localNote.IsTheNoteOk = isCompleted;
            }

            // Sonra DB'ye yaz (listeyi yeniden yükleme, StateHub tetikleme yok)
            await ExecuteWithLock("toggleNote", async () =>
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                var note = await db.TaskNotes.FirstOrDefaultAsync(n => n.Id == noteId, _cts.Token);
                if (note != null)
                {
                    note.IsTheNoteOk = isCompleted;
                    db.TaskNotes.Update(note);
                    await db.SaveChangesAsync(_cts.Token);
                }
            });
        }

        public async Task AddNewCategori()
        {
            await ExecuteWithLock("AddNewCategori", async () =>
            {
                // Null kontrolü ve validasyon
                if (TaskCategories == null ||
                    string.IsNullOrWhiteSpace(TaskCategories.Title) ||
                    string.IsNullOrWhiteSpace(TaskCategories.Description))
                {
                    await ShowNotification("danger", "Hata", "Kategori bilgileri eksik.", null);
                    return;
                }
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                TaskCategories.UserId = use?.Id;
                TaskCategories.CreatedAt = DateTime.UtcNow;
                db.TaskCategories.Add(TaskCategories);
                await db.SaveChangesAsync(_cts.Token);
                await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                await ShowNotification("success", "Başarılı", "Yeni kategori eklendi.", null);
                await JS.InvokeVoidAsync("eval", "$('#AddNewCategoryModal').modal('hide')");
                // Formu temizle
                TaskCategories = new();
            });
        }
        public async Task EditCategori(TaskCategories tce)
        {
            await ExecuteWithLock("EditCategori", async () =>
            {
                if (!string.IsNullOrEmpty(tce.Title) &&
                !string.IsNullOrWhiteSpace(tce.Title) &&
                !string.IsNullOrEmpty(CategoryStructure) &&
                !string.IsNullOrEmpty(tce.Description) &&
                !string.IsNullOrWhiteSpace(tce.Description))
                {
                    await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                    tce.CategoryStructure = CategoryStructure;
                    tce.TaskFrameworkId = TaskFrameworkId;
                    db.TaskCategories.Update(tce);
                    await db.SaveChangesAsync(_cts.Token);
                    await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                    await ShowNotification("success", "Başarılı", "Kategori bilgileri güncellendi.", null);
                    await JSRuntime.InvokeVoidAsync("eval", $"$('#edit_{tce.Id}').modal('hide')");
                }
                else
                {
                    await ShowNotification("danger", "Hata", "Lütfen boş alan bırakmayınız", null);
                }
            });
        }
        public async Task DeleteCategori(TaskCategories tce)
        {
            await ExecuteWithLock("EditCategori", async () =>
            {
                await using _ApplicationConnectionDb? db = await DbFactory.CreateDbContextAsync(_cts.Token);
                if (tce.IsDeleted == null)
                {
                    tce.IsDeleted = new IsDeleted();
                }
                tce.IsDeleted.IsDeletedStatu = true;
                tce.IsDeleted.DeletedAtDate = DateTime.UtcNow;
                db.TaskCategories.Update(tce);
                await db.SaveChangesAsync(_cts.Token);
                await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                await ShowNotification("success", "Başarılı", "Kategori bilgileri silindi.", null);
                await JSRuntime.InvokeVoidAsync("eval", $"$('#delete_{tce.Id}').modal('hide')");
                await JSRuntime.InvokeVoidAsync("eval", $"$('delete_{tce.Id}').modal('hide')");
            });
        }
        public int GetTaskCount(TaskCategories tc)
        {
            // 1. null kontrolü
            if (tc == null) return 0;
            // 2. DbContext'i senkron olarak oluşturun
            using var db = DbFactory.CreateDbContext();
            // 3. Senkron Count() kullanarak sonucu döndürün
            return db.TaskStatus.Count(x => x.TaskCategoriesId == tc.Id && x.IsDeleted.IsDeletedStatu != true);
        }
        public string IsEmailNotificationEnabled(bool? Statu)
        {
            return (Statu ?? false) ? "checked" : "";
        }
        public void EditNoteModalStatu(TaskNotes? tn)
        {
            showNoteEditlModal = !showNoteEditlModal;
            if (tn != null)
            {
                Task_Notes = new TaskNotes
                {
                    Id = tn.Id,
                    TaskStatusId = tn.TaskStatusId,
                    UserId = tn.UserId,
                    Note = tn.Note,
                    NoteCreatedAt = tn.NoteCreatedAt,
                    IsTheNoteOk = tn.IsTheNoteOk,
                    IsDeleted = tn.IsDeleted
                };
            }
        }
        public async Task SaveEditNoteTask()
        {
            if (Task_Notes == null) return;
            if (string.IsNullOrEmpty(Task_Notes.Note))
            {
                await ShowNotification("danger", "Hata", "Not alanı boş olamaz.", null);
                return;
            }
            await ExecuteWithLock("editNote", async () =>
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                var existingNote = await db.TaskNotes.FirstOrDefaultAsync(n => n.Id == Task_Notes.Id, _cts.Token);
                if (existingNote != null)
                {
                    existingNote.Note = Task_Notes.Note;
                    db.TaskNotes.Update(existingNote);
                    await db.SaveChangesAsync(_cts.Token);
                    await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
                    await ShowNotification("success", "Başarılı", "Not güncellendi.", null);
                    if (selectedTask != null)
                    {
                        await LoadTaskNotes(selectedTask.Id);
                    }
                    showNoteEditlModal = false;
                }
            });
        }


        public async Task AddNewTaskFramewok()
        {
            await ExecuteWithLock("AddNewTaskFramewok", async () =>
            {
                // Null kontrolü ve validasyon
                if (
                    string.IsNullOrWhiteSpace(tf.Icon) ||
                    string.IsNullOrWhiteSpace(tf.Title) ||
                    string.IsNullOrWhiteSpace(tf.Description))
                {
                    await ShowNotification("danger", "Hata", "İlgili bilgiler eksik.", null);
                    return;
                }
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                tf.UserId = use.Id;
                tf.CreatedAt = DateTime.UtcNow;
                db.TaskFramework.Add(tf);
                await db.SaveChangesAsync(_cts.Token);
                await StateHub.NotifyDataChanged("AddNewTaskFramewok");
                await ShowNotification("success", "Başarılı", "Yeni bölüm eklendi.", null);
                await JS.InvokeVoidAsync("eval", "$('#AddNewEpisodeModal').modal('hide')");
                // Formu temizle
                tf = new TaskFramework();
            });
        }

        public async Task EditFramework(Guid? _TaskFrameworkId)
        {
            // 1. Önce veritabanı işlemini kilitle yapın
            bool success = false;
            await ExecuteWithLock("EditFramework", async () =>
            {
                try
                {
                    await using var dbCtx = await DbFactory.CreateDbContextAsync(_cts.Token);
                    var existing = await dbCtx.TaskFramework.FirstOrDefaultAsync(x => x.Id == _TaskFrameworkId, _cts.Token);
                    if (existing != null)
                    {
                        existing.Icon = db.TaskFramework.FirstOrDefault(x => x.Id == TaskFrameworkId).Icon;
                        existing.Title = db.TaskFramework.FirstOrDefault(x => x.Id == TaskFrameworkId).Title;
                        existing.Description = db.TaskFramework.FirstOrDefault(x => x.Id == TaskFrameworkId).Description;
                        dbCtx.TaskFramework.Update(existing);
                        await dbCtx.SaveChangesAsync(_cts.Token);
                        success = true;
                    }
                    else
                    {
                        await ShowNotification("error", "Hata", "Güncellenecek kayıt bulunamadı.", null);
                    }
                }
                catch (Exception ex)
                {
                    await ShowNotification("error", "Hata", "Kaydedilirken bir sorun oluştu: " + ex.Message, null);
                }
            });

            // 2. Kilit (Lock) dışına çıktıktan sonra UI ve Hub işlemlerini yapın
            if (success)
            {
                await JS.InvokeVoidAsync("eval", "$('#EditFramework').modal('hide')");
                await ShowNotification("success", "Başarılı", "Bölüm başarıyla güncellendi.", null);

                // Diğer kullanıcılara bildir
                await StateHub.NotifyDataChanged("CentralSystemTaskBoard");

                // Bu bileşeni manuel yenile (Gerekirse)
                StateHasChanged();
            }
        }
        public async Task TrashFramework(Guid? _TaskFrameworkId)
        {
            bool isSuccess = false;

            await ExecuteWithLock("TrashFramework", async () =>
            {
                try
                {
                    await using var dbCtx = await DbFactory.CreateDbContextAsync(_cts.Token);
                    var trash = await dbCtx.TaskFramework.FirstOrDefaultAsync(x => x.Id == _TaskFrameworkId, _cts.Token);
                    if (trash != null)
                    {
                        if (trash.IsDeleted == null)
                        {
                            trash.IsDeleted = new IsDeleted();
                        }
                        trash.IsDeleted.IsDeletedStatu = true;
                        trash.IsDeleted.DeletedAtDate = DateTime.UtcNow;
                        dbCtx.TaskFramework.Update(trash);
                        await dbCtx.SaveChangesAsync(_cts.Token);
                        isSuccess = true; // Veritabanı işi bitti, kilit dışına çıkabiliriz
                    }
                }
                catch (Exception ex)
                {
                    await ShowNotification("error", "Hata", "Silinirken bir sorun oluştu: " + ex.Message, null);
                }
            });

            // Kilit (Lock) mekanizmasının DIŞINDA UI ve State işlemlerini yapıyoruz
            if (isSuccess)
            {
                // Formu temizle
                tf = new TaskFramework();

                // Modal kapat ve bildirim ver
                await JS.InvokeVoidAsync("eval", "$('#TrashFramework').modal('hide')");
                await ShowNotification("success", "Başarılı", "Bölüm başarıyla silindi.", null);

                // DİKKAT: En son tetiklemeyi yap ki render başladığında DB meşgul olmasın
                await StateHub.NotifyDataChanged("CentralSystemTaskBoard");
            }
        }

        public async Task CopyText(string DescText)
        {
            bool isSuccess = false;
            bool isEmpty = false;

            // 1. İşlem devam ediyorsa veya kilit mekanizması aktifse çalıştır
            await ExecuteWithLock("CopyText", async () =>
            {
                try
                {
                    // Metin kontrolü
                    if (string.IsNullOrEmpty(DescText))
                    {
                        isEmpty = true;
                        return;
                    }

                    // Tarayıcı Clipboard API Entegrasyonu (JS Interop kilit içinde güvenli)
                    await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", DescText);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    // Hata durumunda bildirimi burada (kilit içinde veya dışında) verebiliriz
                    await ShowNotification("danger", "Hata", $"Kopyalama işlemi başarısız: {ex.Message}", null);
                }
            });

            // 2. Kilit (Lock) mekanizmasının DIŞINDA UI ve Bildirim işlemleri
            if (isEmpty)
            {
                await ShowNotification("warning", "Uyarı", "Kopyalanacak içerik bulunamadı.", null);
            }
            else if (isSuccess)
            {
                // Başarı bildirimi
                await ShowNotification("success", "Başarılı", "İçerik başarıyla kopyalandı.", null);

                // Eğer UI üzerinde bir değişiklik gerekiyorsa (örneğin kopyalandı ikonu göstermek gibi)
                StateHasChanged();
            }
        }
        #endregion

    }
}
