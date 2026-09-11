# ระบบแจ้งซ่อม (Repair Request)
โปรแจค C# BorntoDev
เว็บแอปพลิเคชันสำหรับแจ้ง ติดตาม มอบหมาย และปิดงานซ่อมอุปกรณ์ พัฒนาด้วย C# ASP.NET Core และ SQL Server

## ภาพรวมระบบ

ผู้ใช้งานสามารถสร้างบัญชีด้วย **ชื่อผู้ใช้และรหัสผ่าน** จากนั้นแจ้งปัญหาอุปกรณ์ ระบุสถานที่และรายละเอียดของปัญหา และติดตามรายการแจ้งซ่อมของตนเองได้ ระบบจัดเก็บรหัสผ่านแบบเข้ารหัสด้วย ASP.NET Core Identity ไม่เก็บรหัสผ่านเป็นข้อความปกติ

## เทคโนโลยี

| ส่วนงาน | เทคโนโลยี |
| --- | --- |
| Web application | C# / ASP.NET Core MVC (.NET 10) |
| UI | Razor Views, Bootstrap |
| Authentication | ASP.NET Core Identity (Username / Password) |
| Data access | Entity Framework Core |
| Database | Microsoft SQL Server (`Repair_db`) |
| Tests | xUnit |

## ฐานข้อมูล

ระบบใช้ SQL Server โดยมีตารางหลักดังนี้

- `AspNetUsers` - บัญชีผู้ใช้งานและรหัสผ่านที่ผ่านการแฮช
- `AspNetRoles` - สิทธิ์ผู้ใช้: `Requester`, `Technician`, `Supervisor`, `Administrator`
- `RepairRequests` - ข้อมูลใบแจ้งซ่อม เช่น หมายเลขคำขอ อุปกรณ์ สถานที่ ความสำคัญ สถานะ และวันเวลาสร้าง
- ตาราง Identity ที่เกี่ยวข้อง เช่น `AspNetUserRoles`, `AspNetUserClaims` และ `AspNetUserTokens`

การเชื่อมต่อฐานข้อมูลเก็บใน .NET User Secrets จึงไม่มีรหัสผ่านอยู่ใน Git หรือ `appsettings.json`

## API / เส้นทางหลัก

| เส้นทาง | วิธีการ | หน้าที่ |
| --- | --- | --- |
| `/Account/Register` | GET, POST | สมัครสมาชิกด้วยชื่อผู้ใช้และรหัสผ่าน |
| `/Account/Login` | GET, POST | เข้าสู่ระบบ |
| `/Account/Logout` | POST | ออกจากระบบ |
| `/RepairRequests` | GET | แสดงรายการแจ้งซ่อมของผู้ใช้ที่เข้าสู่ระบบ |
| `/RepairRequests/Create` | GET, POST | สร้างใบแจ้งซ่อมใหม่ |

> ขณะนี้ระบบเป็น MVC server-rendered routes ไม่ใช่ JSON REST API แบบ public. สามารถเพิ่ม `/api/repair-requests` ภายหลังได้หากต้องการเชื่อม Mobile App หรือระบบอื่น

## เริ่มใช้งาน

```powershell
dotnet restore
dotnet tool restore
dotnet watch --project RepairRequest.Web
```

เปิด `http://localhost:5271` หรือพอร์ตที่แสดงในหน้าจอ แล้วไปที่ `/Account/Register` เพื่อสร้างบัญชีผู้ใช้แรก

---

# Repair Request Web Application

A C# ASP.NET Core web application for submitting, assigning, tracking, and closing equipment repair requests. The application uses SQL Server as its data store.

## Recommended stack

- .NET 10 (ASP.NET Core MVC)
- Entity Framework Core with `Microsoft.EntityFrameworkCore.SqlServer`
- SQL Server
- ASP.NET Core Identity for authentication and role-based access

## Core roles

| Role | Capabilities |
| --- | --- |
| Requester | Create requests, attach photos/documents, and see the status of their own requests. |
| Technician | View assigned work, add work notes, record parts/labor, and update repair status. |
| Supervisor | Assign technicians, set priority, approve completion, and view reports. |
| Administrator | Manage users, roles, equipment, locations, and system settings. |

## Main workflow

1. A requester creates a repair request with equipment, location, problem description, priority, and optional attachments.
2. A supervisor reviews the request and assigns it to a technician.
3. The technician records diagnosis, repair actions, parts used, and work notes.
4. The request moves through `New`, `Assigned`, `In Progress`, `Waiting for Parts`, `Completed`, or `Closed`.
5. The requester or supervisor confirms completion and closes the request.

## Suggested SQL Server tables

- `RepairRequests` - request number, requester, equipment, description, priority, status, assigned technician, and timestamps.
- `RepairRequestNotes` - technician and supervisor comments, status changes, and work logs.
- `RepairRequestAttachments` - attachment metadata and secure file-storage reference.
- `Equipment` - equipment code, name, serial number, location, and active status.
- `Locations` - site, building, area, or production line.
- `AspNetUsers`, `AspNetRoles`, and related Identity tables - application accounts and roles.

## Create the project

```powershell
dotnet new mvc --auth Individual
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

Create an `ApplicationDbContext` containing the Identity tables and the repair-request entities. Register it in `Program.cs` with `UseSqlServer` and create EF Core migrations for the schema.

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## SQL Server configuration

The supplied connection details include a database server and SQL login, but no database name. Confirm the intended database name with the database administrator before running migrations.

Do **not** place the SQL password in `appsettings.json`, source code, or this repository. Store the connection string in .NET user secrets for local development, or in your deployment platform's secret store for production.

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:RepairRequestDb" "Data Source=<SQL_SERVER>;Initial Catalog=<DATABASE_NAME>;User ID=<SQL_USER>;Password=<SQL_PASSWORD>;Encrypt=True;TrustServerCertificate=True;Pooling=False;MultipleActiveResultSets=False;Application Name=RepairRequest;Command Timeout=30"
```

Use the configuration entry in `Program.cs`:

```csharp
var connectionString = builder.Configuration.GetConnectionString("RepairRequestDb")
    ?? throw new InvalidOperationException("Connection string 'RepairRequestDb' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

For production, prefer `TrustServerCertificate=False` and install/use a certificate trusted by the application host. `TrustServerCertificate=True` should only be retained when required by the environment and approved by the security team.

## Security requirements

- Keep database credentials in user secrets, environment variables, Azure Key Vault, or another approved secret manager.
- Restrict the SQL login to the required database and minimum permissions; do not use `sysadmin`.
- Require authenticated users and enforce authorization policies for every request, attachment, and report.
- Validate all form input server-side and use EF Core or parameterized queries only.
- Limit attachment type and size; scan uploads and store them outside the web root.
- Enable HTTPS, antiforgery protection, secure cookies, logging, and error handling suitable for production.
- Rotate the supplied SQL password because it was shared in plaintext.

## Next implementation milestones

1. Create the data model and EF Core migration.
2. Add Identity, roles, and authorization policies.
3. Build the requester submission and request-list screens.
4. Add assignment, technician work-log, and status-update features.
5. Add attachments, notifications, reporting, tests, and deployment configuration.
