# سیستم رزرو منابع شرکت

## تکنولوژی‌ها
- ASP.NET Core 9 Web API
- Entity Framework Core (Code First)
- SQL Server
- Angular 17

## چالش فنی انتخاب‌شده: گزینه B – بهینه‌سازی جستجوی SQL
برای یافتن نوبت‌های آزاد در بازه یک هفته از Stored Procedure با ایندکس‌گذاری بهینه استفاده شده.
ایندکس ترکیبی `IX_Reservation_Resource_Time_Status` روی جدول Reservations تعریف شده تا جستجو با ۱۰۰۰+ رکورد سریع باشد.

## ساختار پروژه
- `Core` – Entities، DTOs، Interfaces
- `Infrastructure` – EF Core، Repositories، UnitOfWork
- `API` – Controllers، Services، Middleware

## فرضیات
- احراز هویت پیاده‌سازی نشده؛ از UserId=1 به عنوان کاربر پیش‌فرض استفاده می‌شود
- ساعت کاری ۸ تا ۱۸ فرض شده
- تنها endpoint های موردنیاز در فرم رزرو پیاده شده و باقی endpoint های خواسته شده  تمرین، از طریق swagger قابل تست و مشاهده است

## اجرا
1. `appsettings.json` را با connection string خود تنظیم کنید
2. `dotnet run` در پوشه API
3. `ng serve` در پوشه Angular
4. `swagger link` : http://localhost:5190/swagger/index.html