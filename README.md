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

