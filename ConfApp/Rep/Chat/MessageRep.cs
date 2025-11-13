// ConfApp/Rep/Chat/MessageRep.cs
using System.Linq.Expressions;
using App;
using App.Contracts.Object.Shop.ProductCon;
using App.Object.Chat.MessageApp;
using Domain.Objects.Chat;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;

namespace ConfApp.Rep.Chat
{
    public class MessageRep : IMessageRep
    {
        private readonly MyContext _context;
        private bool _disposed = false;

        public MessageRep(MyContext context)
        {
            _context = context;
        }

        // --- IBaseRep Implementation ---
        public async Task<Message> GetAsync(int id) => 
            await _context.Messages.FindAsync(id) ?? throw new KeyNotFoundException();

        public Message Get(int id) => 
            _context.Messages.Find(id) ?? throw new KeyNotFoundException();

        public async Task<List<Message>> GetAsync() => 
            await _context.Messages.ToListAsync();

        public async Task<List<Message>> GetAsync(Pagination pagination)
        {
            return await _context.Messages
                .OrderBy(m => m.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<List<Message>> GetFilteredAsync(
            Expression<Func<Message, bool>>? filter = null, 
            ProductSearchCriteria? criteria = null)
        {
            var query = filter != null ? _context.Messages.Where(filter) : _context.Messages.AsQueryable();
            return await query.ToListAsync();
        }

        public async Task CreateAsync(Message entity)
        {
            entity.CreateDate = DateTime.Now;
            await _context.Messages.AddAsync(entity);
        }

        public void Delete(Message entity)
        {
            entity.IsDeleted = true;
            entity.UpdateDate = DateTime.Now;
            _context.Messages.Update(entity);
        }

        public void DeleteById(int id)
        {
            var entity = _context.Messages.Find(id);
            if (entity != null) Delete(entity);
        }

        public async Task<int> CountAsync() => 
            await _context.Messages.CountAsync();

        public async Task<int> CountAsync(Expression<Func<Message, bool>> filter) => 
            await _context.Messages.CountAsync(filter);

        public async Task<bool> ExistAsync(Expression<Func<Message, bool>> expression) => 
            await _context.Messages.AnyAsync(expression);

        public async Task<bool> UpdateAsync(Message entity)
        {
            entity.UpdateDate = DateTime.Now;
            _context.Messages.Update(entity);
            return true;
        }

        public async Task SaveChangesAsync() => 
            await _context.SaveChangesAsync();

        public async Task<List<Message>> GetByIdsAsync(List<int> ids) => 
            await _context.Messages.Where(m => ids.Contains(m.Id)).ToListAsync();

        public void DeleteRange(List<Message> entities)
        {
            foreach (var e in entities)
                e.IsDeleted = true;
            _context.Messages.UpdateRange(entities);
        }

        public Task<bool> HasRelationsAsync(Message entity) => 
            Task.FromResult(false); // بعداً پیاده‌سازی

        // --- IMessageRep Specific Methods ---
        public async Task<List<Message>> GetChatHistoryAsync(int userId, int otherUserId, Pagination pagination)
        {
            return await _context.Messages
                .Where(m => 
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int receiverId, int senderId) =>
            await _context.Messages
                .CountAsync(m => 
                    m.ReceiverId == receiverId && 
                    m.SenderId == senderId && 
                    m.SeenAt == null && 
                    !m.IsDeletedForReceiver);

        public async Task MarkAsReadAsync(int receiverId, int senderId)
        {
            var messages = await _context.Messages
                .Where(m => m.ReceiverId == receiverId && m.SenderId == senderId && m.SeenAt == null)
                .ToListAsync();

            foreach (var m in messages)
            {
                m.SeenAt = DateTime.Now;
                m.UpdateDate = DateTime.Now;
            }

            _context.Messages.UpdateRange(messages);
        }

        // --- IDisposable ---
        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}