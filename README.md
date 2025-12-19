# نمونه پروژه CRM: بک‌اند لایه‌ای حرفه‌ای با .NET 8 🚀

![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet?style=flat-square)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-brightgreen?style=flat-square)
![SignalR](https://img.shields.io/badge/SignalR-Realtime-orange?style=flat-square)
![EF Core](https://img.shields.io/badge/Entity_Framework_Core-Code_First-blue?style=flat-square)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean_Onion-informational?style=flat-square)

> یک CRM ساده اما کامل و حرفه‌ای که برای نمایش سبک کدزنی من ساخته شده.  
> هدف این پروژه اینه که کارفرماها و همکارها بتونن سریع ببینن من چطور کد می‌زنم: تمیز، لایه‌ای، با رعایت SOLID و بهترین Practices مدرن .NET.  
> اینجا یک **تور هدایت‌شده** از پروژه‌ست – همراهم بیاید تا نشون بدم چطور فکر می‌کنم و کد می‌نویسم.

## فهرست مطالب 📑
- [معماری پروژه](#معماری-پروژه-🏗️)
- [تور ویژگی‌های کلیدی](#تور-ویژگی‌های-کلیدی-🔍)
  - [چت realtime با SignalR](#چت-realtime-با-signalr-💬)
  - [احراز هویت و مجوزهای پیشرفته](#احراز-هویت-و-مجوزهای-پیشرفته-🔒)
  - [مدیریت فروشگاه و انبار](#مدیریت-فروشگاه-و-انبار-🛒)
  - [مدیریت کاربران و نقش‌ها](#مدیریت-کاربران-و-نقش‌ها-👥)
  - [ابزارهای پایه و Utilityها](#ابزارهای-پایه-و-utilityها-🧰)
  - [دیتابیس و Migrations](#دیتابیس-و-migrations-📊)
- [نحوه اجرا](#نحوه-اجرا-⚙️)

## معماری پروژه 🏗️
پروژه بر اساس **Clean Architecture (Onion Style)** لایه‌بندی شده:  
`Domain` در مرکز قرار داره و هیچ وابستگی خارجی نداره. وابستگی‌ها فقط به سمت داخل اشاره می‌کنن.

لایه‌ها:
- **Domain** → Entityهای خالص
- **App** → منطق بیزینس، Interfaces و DTOها
- **ConfApp** → تنظیمات EF، Repositoryها و Mappingها
- **API** → Controllerها، Hubها و Middlewareها
- **Infrastructure** → Migrationها و Seeding
- **MyFrameWork** → ابزارهای reusable مثل ApiResult، Pagination و Validatorها

این ساختار باعث می‌شه کد Maintainable، Testable و Scalable باشه.

## تور ویژگی‌های کلیدی 🔍
بیا قدم‌به‌قدم کدهای مهم رو ببینیم. هر بخش لینک مستقیم به فایل داره تا خودت کد رو بخونی و ببینی چطور نوشتم.

### چت realtime با SignalR 💬
چت دو نفره کامل با پشتیبانی از متن، عکس و فایل + Resizing خودکار عکس + Unread Count + Mark as Read + وضعیت آنلاین/آفلاین کاربران.

**نکات برجسته:**
- Hub ساده و آماده Scale
- Integration کامل با وضعیت کاربران
- Resizing تصاویر موقع آپلود با ImageSharp

**کدهای کلیدی:**
- [ChatHub.cs](https://github.com/yourusername/CRM-Sample/blob/main/API/Hubs/ChatHub/ChatHub.cs)
- [ChatApp.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Object/Chat/ChatApp.cs)
- [SendMessageDto.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Contracts/Object/Chat/SendMessageDto.cs)
- [MessageMapping.cs](https://github.com/yourusername/CRM-Sample/blob/main/ConfApp/Mapping/Chat/MessageMapping.cs)

### احراز هویت و مجوزهای پیشرفته 🔒
JWT + RBAC با Permissionهای پویا (اسکن خودکار Attributeها در Startup) + KeepAlive برای وضعیت آنلاین + Cache وضعیت کاربران.

**نکات برجسته:**
- Permissionها به صورت خودکار از Attributeهای Controllerها استخراج و Seed می‌شن
- وضعیت آنلاین/آفلاین/غیرفعال با ConcurrentDictionary و ذخیره در فایل JSON
- Middleware سفارشی برای اعتبارسنجی توکن

**کدهای کلیدی:**
- [RequirePermissionAttribute.cs](https://github.com/yourusername/CRM-Sample/blob/main/API/Attributes/RequirePermissionAttribute.cs)
- [UserStatusService.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Object/Base/Users/UserStatusService.cs)
- [PermissionSeeder.cs](https://github.com/yourusername/CRM-Sample/blob/main/Infrastructure/data/seed/PermissionSeeder.cs)
- [TokenApp.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Object/Base/auth/TokenApp.cs)

### مدیریت فروشگاه و انبار 🛒
CRUD کامل برای محصولات، واحدهای شمارش و انبارها + جستجوی پیشرفته (نام + محدوده قیمت) + Pagination و Sorting دینامیک.

**نکات برجسته:**
- Queryهای بهینه با Expression Trees
- Sorting و Filtering دینامیک بدون N+1

**کدهای کلیدی:**
- [ProductController.cs](https://github.com/yourusername/CRM-Sample/blob/main/API/Controllers/Shop/ProductController.cs)
- [ProductView.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Contracts/Object/Shop/ProductCon/ProductView.cs)
- [BaseRep.cs](https://github.com/yourusername/CRM-Sample/blob/main/ConfApp/Rep/BaseRep.cs) → Dynamic OrderBy و Pagination

### مدیریت کاربران و نقش‌ها 👥
CRUD کاربران با آپلود عکس پروفایل (با Resizing) + تخصیص چند Role + Hashing امن با BCrypt.

**نکات برجسته:**
- Override هوشمند از CrudService ژنریک
- اعتبارسنجی Uniqueness برای Username و Email

**کدهای کلیدی:**
- [UsersApp.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Object/Base/Users/UsersApp.cs)
- [UsersDto.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Contracts/Object/Base/Users/UsersDto.cs)

### ابزارهای پایه و Utilityها 🧰
- Responseهای استاندارد و یکدست با Paging
- CrudService ژنریک برای کاهش تکرار کد
- FileService با Resizing و آپلود امن

**کدهای کلیدی:**
- [ApiResult.cs](https://github.com/yourusername/CRM-Sample/blob/main/MyFrameWork/AppTool/ApiResult.cs)
- [CrudService.cs](https://github.com/yourusername/CRM-Sample/blob/main/App/Object/Base/CrudService.cs)
- [Pagination.cs](https://github.com/yourusername/CRM-Sample/blob/main/MyFrameWork/AppTool/Pagination.cs)
- [FileService.cs](https://github.com/yourusername/CRM-Sample/blob/main/API/utility/FileService.cs)

### دیتابیس و Migrations 📊
Code-First + Soft Delete در تمام Entityها + Seeding خودکار Permissionها و کاربران اولیه.

**کدهای کلیدی:**
- [MyContext.cs](https://github.com/yourusername/CRM-Sample/blob/main/ConfApp/MyContext.cs)
- [Migration اولیه](https://github.com/yourusername/CRM-Sample/blob/main/Infrastructure/Migrations/20251107184623_init.cs)

## نحوه اجرا ⚙️
1. پیش‌نیازها: .NET 8 SDK + SQL Server
2. Connection String رو در `appsettings.json` تنظیم کنید
3. Migrationها رو اعمال کنید:
   ```bash
   dotnet ef database update
   
4. پروژه رو ران کنید
  ```bash
   dotnet run

   اگر این سبک کدزنی رو دوست داشتی ⭐ ستاره بزن!
سوال یا پیشنهادی داشتی Issue باز کن. منتظر فیدبکتون هستم 🙌