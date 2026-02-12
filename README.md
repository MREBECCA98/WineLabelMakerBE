# WINE LABEL MAKER - BACK END  
  
## DESCRIPTION  
Wine Label Maker backend handles user accounts, wine label requests, and email notifications for the Wine Label Maker application.  
It exposes RESTful APIs for the frontend to interact with the database, manage users and requests, and send status update emails.  
  
## FEATURES  
  
### USER  
**User Authentication:** registration and login handled via Identity, with JWT-based secure tokens for session management  
**Request Management:**  
-Users can create wine label requests with detailed information  
-Users can edit or delete their requests only if the status is Pending  
-Users can view only their own requests  
  
### ADMIN  
**Administrator Management:**  
-Admins can view all requests submitted by all users  
-Admins can update the status of any request  
-Admins cannot delete or edit requests  
  
**Email Notifications:**  
-Automatic emails are sent when request statuses change  
-Two emails are handled manually: Quote and Completed  
-For Completed requests, the email includes the image of the final label  
  
**Secure Access:** protected routes ensure that users can only access their own requests, and admin functions are restricted  
  
## TECHNOLOGIES USED  
-**.NET 8 / ASP.NET CORE** - backendframework for building RESTful APIs  
-**Entity Framework Core** - ORM for database management  
-**SQL Server** - database  
-**ASP.NET Identity** - authentication and authorization management  
-**JWT (JSON Web Tokens)** -secure authentication and session management  
-**FluentEmail** - email  
-**C#** -programming language  
  
##  GETTING STARTED - VISUAL STUDIO  
1 - **Clone the repository**  
cd DESKTOP  
git clone https://github.com/MREBECCA98/WineLabelMakerBE.git  
2 - **Open the solution**  
Open the WineLabelMakerBE folder and open the WineLabelMakerBE.sln file in Visual Studio  
3 - **Create configuration file**  
In the main root of the project, create a new file **appsettings.Development.json** (right-click on the main root, add, new item)  
Copy this code and insert it into appsettings.Development.json:  
**------------------------------------------------------------------------------------------------------------------------------------------------------------------------**  
{  
  "Logging": {  
    "LogLevel": {  
      "Default": "Information",  
      "Microsoft.AspNetCore": "Warning"  
    }  
  },  
    
  //Stringa di connessione al database SQL Server  
  "AllowedHosts": "*",  
  "ConnectionStrings": {  
    "DefaultConnection": "Data Source=**YOUR SERVER NAME**;Database=**YOUR DATABASE NAME**;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"  
  },  
    
  //Email - FLUENT EMAIL  
  "FluentEmail": {  
    "FromEmail": "**YOUR EMAIL**",  
    "FromName": "Wine Label Maker",  
    "Smtp": {  
      "Host": "smtp.gmail.com",  
      "Port": 587,  
      "Username": "**YOUR EMAIL**",  
      "Password": "**YOUR APP PASSWORD HERE (generete app password from google)**",  
      "EnableSsl": true  
    }  
  },  
    
  //Admin  
  "Seeder": {  
    "Admin": {  
      "Email": "**YOUR EMAIL**",  
      "Password": "**YOUR PASSWORD**",  
      "Name": "**YOUR NAME**",  
      "Surname": "**YOUR SURNAME**",  
      "CompanyName": "Wine Label Maker"  
    }  
  },  
    
  //Token    
  "Jwt": {  
    "SecurityKey": "**SECURITY KEY JWT - 256 bits (https://jwtsecrets.com/)**",  
    "Issuer": "WineLabelMakerBE",  
    "Audience": "WineLabelMakerFE"  
  }  
}  
**------------------------------------------------------------------------------------------------------------------------------------------------------------------------**  
  
**CREATE A PASSWORD FOR THE GMAIL APP**    
-Sign in to your Gmail account  
-Go to Manage your Google Account > Security > App Passwords  
-Google will generate a 16-character password  
-Copy it and enter it in the "Password" field in the appsettings.Development.json file only for Email  
  
**If you don’t have a Google account, create one**  
  
## RUNNING THE PROJECT  
-Open WineLabelMakerBE.sln in Visual Studio  
-Ensure your appsettings.Development.json file is correctly configured  
-Press F5 (or click Start Debugging) to run the project  
  
The backend will start on **https://localhost:7046/swagger/index.html** by default  
You can open this URL in your browser to access Swagger UI and test all the APIs securely via HTTPS  

