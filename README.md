# 🏨 TravelEase – Hotel Booking Backend API

A backend **RESTful API** for a hotel and accommodation booking platform, built with **ASP.NET Core** and **Entity Framework Core**. It enables user authentication, hotel and room search, secure bookings, and full CRUD operations for cities, hotels, and rooms — all with robust unit testing.

---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core  
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

Run tests with:

```bash
dotnet test

## 📁 Project Structure
TravelEase/
├── API/                # Controllers & Startup config
├── Application/        # DTOs, Interfaces, Services
├── Domain/             # Entity models
├── Infrastructure/     # EF DbContext, Repositories
├── Tests/              # Unit Tests
