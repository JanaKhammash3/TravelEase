# 🏨 TravelEase – Hotel Booking Backend API

A backend **RESTful API** for a hotel booking system, built with **C#**, **ASP.NET Core**, and **Entity Framework Core**. It supports user authentication, hotel and room search, secure bookings, and full admin management. Features secure JWT authentication, role-based access control, and full unit testing.

---

## 🛠️ Tech Stack

- **Language:** C#  
- **Framework:** ASP.NET Core  
- **ORM:** Entity Framework Core  
- **Database:** SQL Server  
- **Authentication:** JWT (JSON Web Token)  
- **Testing:** xUnit / MSTest

---

## ✨ Features

### 👤 User
- Register and login
- Search cities and hotels
- Filter by date, price, stars, and amenities
- View hotel details, rooms, and gallery
- Book rooms securely and receive email confirmation

### 🛠️ Admin
- Manage Cities (CRUD)
- Manage Hotels (CRUD + assign city)
- Manage Rooms (CRUD + availability & capacity)
- Admin dashboard with search & filters

---

## 🔒 Security

- JWT authentication
- Role-based access control (Admin/User)
- Input validation & exception handling
- Logging for errors and system monitoring

---

## 🧪 Unit Testing

Includes unit test coverage for:
- HotelService
- BookingService
- RoomService
- Admin CRUD Services
  
---

## 📁 Project Structure
```bash
TravelEase/
├── API/                # Controllers & Startup config
├── Application/        # DTOs, Interfaces, Services
├── Domain/             # Entity models
├── Infrastructure/     # EF DbContext, Repositories
├── Tests/              # Unit Tests
