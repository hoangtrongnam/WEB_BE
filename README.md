# WEB_BE
WEB_BE
# tạo project 
dotnet new webapi -n WEB_BANHANG_BE
# chuyển đường dẫn đến thư mục project
cd WEB_BANHANG_BE
# chạy project
dotnet run
# url vào ứng dụng có thể là này tùy vào port
http://localhost:5157/swagger/index.html
# cài đặt thư viện Oracle.ManagedDataAccess.Core
dotnet add package Oracle.ManagedDataAccess.Core --version 3.21.90
# cài đặt log
dotnet add package NLog
sử dụng các phương thức của logger để ghi log:
```csharp
logger.Trace("Trace message");
logger.Debug("Debug message");
logger.Info("Info message");
logger.Warn("Warn message");
logger.Error("Error message");
logger.Fatal("Fatal message");
```
# cài đặt Newtonsoft.Json
dotnet add package Newtonsoft.Json
# cài đặt System.Data.SqlClient Tạo một kết nối đến cơ sở dữ liệu hoặc cài đặt Oracle.ManagedDataAccess cho oracle
dotnet add package System.Data.SqlClient hoăc dotnet add package Oracle.ManagedDataAccess
# cài đặt 
dotnet add package Swashbuckle.AspNetCore.Newtonsoft
# cài đặt 
dotnet add package Microsoft.AspNetCore.Mvc.Versioning
# cài đặt
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
