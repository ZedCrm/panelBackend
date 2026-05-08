using App.Contracts.Object.Chat;
using App.Object.Base.Users;
using App.utility;
using AutoMapper;
using Domain.Objects.Base;
using Domain.Objects.Chat;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using SixLabors.ImageSharp.Processing;
using ConfApp;
using SixLabors.ImageSharp;

namespace App.Object.Chat
{
    public class ChatApp : IChatApp
    {
        private readonly MyContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly UserStatusService _statusService;

        public ChatApp(MyContext context, IFileService fileService, IMapper mapper, UserStatusService statusService)
        {
            _context = context;
            _fileService = fileService;
            _mapper = mapper;
            _statusService = statusService;
        }

        public async Task<StatusResult> SendMessageAsync(SendMessageDto dto, int senderId)
        {
            var receiver = await _context.Users.FindAsync(dto.ReceiverId);
            if (receiver == null)
                return ResultFactory.Status(ResultStatusEnum.NotFound, "گیرنده یافت نشد.");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                Type = dto.Type,
                SentAt = DateTime.Now,
                DeliveredAt = DateTime.Now
            };
            if (dto.File != null)
            {
                var resize = dto.Type == MessageType.Image ? new ResizeOptions { Size = new Size(800, 800), Mode = ResizeMode.Max } : null;
                message.FileUrl = await _fileService.UploadAsync(dto.File, "uploads/chat", null, resize);
            }
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success, "پیام ارسال شد.");
        }

        public async Task<ListDataResult<MessageView>> GetChatHistoryAsync(int otherUserId, Pagination pagination, int currentUserId)
        {
            var query = _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt);
            var total = await query.CountAsync();
            var messages = await query
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();
            var views = _mapper.Map<List<MessageView>>(messages);
            // پر کردن نام فرستنده
            var currentUser = await _context.Users.FindAsync(currentUserId);
            var otherUser = await _context.Users.FindAsync(otherUserId);
            foreach (var v in views)
            {
                v.SenderName = v.SenderId == currentUserId ? currentUser?.FullName ?? "من" : otherUser?.FullName ?? "کاربر";
                v.IsMine = v.SenderId == currentUserId;
            }
            return ResultFactory.List(ResultStatusEnum.Accepted, views, total, pagination);
        }

        public async Task<ListDataResult<ChatListItem>> GetChatListAsync(int userId)
        {
            var messages = await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .ToListAsync();
            var grouped = messages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g =>
                {
                    var lastMsg = g.OrderByDescending(m => m.SentAt).First();
                    var (status, lastSeen) = _statusService.GetStatus(g.Key);
                    return new ChatListItem
                    {
                        UserId = g.Key,
                        UserName = "کاربر", // در صورت نیاز از جدول Users مقدار نام را بگیرید
                        LastMessage = lastMsg.Content,
                        LastMessageTime = lastMsg.SentAt,
                        UnreadCount = g.Count(m => m.ReceiverId == userId && m.SeenAt == null && !m.IsDeletedForReceiver),
                        IsOnline = status == UserStatus.Online,
                        LastSeen = lastSeen
                    };
                }).ToList();
            return ResultFactory.List(ResultStatusEnum.Success, grouped);
        }

        public async Task<StatusResult> MarkAsReadAsync(int senderId, int receiverId)
        {
            var msgs = await _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && m.SeenAt == null)
                .ToListAsync();
            foreach (var m in msgs)
                m.SeenAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success, "پیام‌ها خوانده شد.");
        }

        public async Task<SingleDataResult<int>> GetTotalUnreadCountAsync(int receiverId)
        {
            var count = await _context.Messages
                .CountAsync(m => m.ReceiverId == receiverId && m.SeenAt == null && !m.IsDeletedForReceiver);
            return ResultFactory.Single(ResultStatusEnum.Success, count);
        }
    }
}