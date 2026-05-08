using System.Collections.Concurrent;
using System.Text.Json;
using Domain.Objects.Base;

namespace App.Object.Base.Users
{
    public class UserStatusService
    {
        private readonly ConcurrentDictionary<int, (UserStatus Status, DateTime? LastSeen)> _userStatuses = new();
        private readonly string _statusFilePath;

        public UserStatusService()
        {
            _statusFilePath = Path.Combine(Directory.GetCurrentDirectory(), "user-status.json");
            LoadFromFile();  // اگر فایل نباشه، LoadFromFile خودش ایجاد می‌کنه
        }

        public void UpdateStatus(int userId, UserStatus status, DateTime? lastSeen = null)
        {
            _userStatuses[userId] = (status, lastSeen ?? DateTime.Now);
            SaveToFile();
        }

        public (UserStatus, DateTime?) GetStatus(int userId)
        {
            return _userStatuses.TryGetValue(userId, out var status) ? status : (UserStatus.Offline, null);
        }

        public void CheckInactive()
        {
            var now = DateTime.Now;
            foreach (var kvp in _userStatuses)
            {
                if (kvp.Value.LastSeen.HasValue && (now - kvp.Value.LastSeen.Value).TotalMinutes > 5 && kvp.Value.Status == UserStatus.Online)
                {
                    UpdateStatus(kvp.Key, UserStatus.Inactive, kvp.Value.LastSeen);
                }
            }
        }

        private void LoadFromFile()
        {
            if (File.Exists(_statusFilePath))
            {
                var json = File.ReadAllText(_statusFilePath);
                var data = JsonSerializer.Deserialize<Dictionary<int, (UserStatus Status, DateTime? LastSeen)>>(json);
                if (data != null)
                {
                    foreach (var kvp in data)
                        _userStatuses[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                // فایل وجود ندارد → یک فایل خالی ایجاد کن
                SaveToFile();
            }
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_userStatuses.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            File.WriteAllText(_statusFilePath, json);
        }
    }
}