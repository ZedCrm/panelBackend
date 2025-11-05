using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Objects.Base;

namespace App.Object.Base.Users
{
    public class UserStatusService
    {
        private readonly ConcurrentDictionary<int, (UserStatus Status, DateTime? LastSeen)> _userStatuses = new();
        private readonly string _statusFilePath = Path.Combine(Directory.GetCurrentDirectory(), "user-status.json");

        public UserStatusService()
        {
            LoadFromFile();  // لود موقع startup
        }

        public void UpdateStatus(int userId, UserStatus status, DateTime? lastSeen = null)
        {
            _userStatuses[userId] = (status, lastSeen ?? DateTime.Now);
            SaveToFile();  // هر بروزرسانی سیو کن (برای تغییرات زیاد, شاید debounce کن اما ساده)
        }

        public (UserStatus, DateTime?) GetStatus(int userId)
        {
            return _userStatuses.TryGetValue(userId, out var status) ? status : (UserStatus.Offline, null);
        }

        public void CheckInactive()  // periodical call مثلاً با timer
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
                var data = JsonSerializer.Deserialize<Dictionary<int, (UserStatus, DateTime?)>>(json);
                if (data != null)
                    foreach (var kvp in data) _userStatuses[kvp.Key] = kvp.Value;
            }
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_userStatuses.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            File.WriteAllText(_statusFilePath, json);
        }
    }
}