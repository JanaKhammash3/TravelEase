# TravelEase – Hotel Booking Backend API

A backend **RESTful API** for a hotel booking system, built with **C#**, **ASP.NET Core**, and **Entity Framework Core**. It supports user authentication, hotel and room search, secure bookings, image uploads, Stripe payments, and admin management. Features secure JWT authentication, role-based access control, third-party integrations, and full unit testing.

---

## 🛠️ Tech Stack

- **Language:** C#  
- **Framework:** ASP.NET Core  
- **ORM:** Entity Framework Core  
- **Database:** SQL Server  
- **Authentication:** JWT (JSON Web Token)  
- **Testing:** xUnit / Moq  
- **CI/CD:** GitHub Actions  
- **Email:** Mailtrap SMTP  
- **Image Hosting:** Cloudinary  
- **Payment Gateway:** Stripe

---

## ✨ Features

### 👤 User
- Register and login securely
- Search cities and hotels
- Filter by date, price, star rating, and amenities
- View hotel details with rooms and gallery
- Book rooms securely with email confirmation
- Pay using integrated Stripe API

### 🛠️ Admin
- Manage Cities (Add, Edit, Delete)
- Manage Hotels (CRUD + city assignment)
- Manage Rooms (CRUD + capacity & availability)
- Upload hotel and room images to Cloudinary
- Dashboard to manage bookings and data

---

## 🔐 Security

- Secure JWT Authentication
- Role-based access control (User / Admin)
- Input validation and exception handling
- Logging for errors and activity monitoring

---

## 🌐 API Integrations

| Feature     | Integration     | Purpose                                 |
|-------------|------------------|------------------------------------------|
| 📧 Email    | Mailtrap SMTP     | Send booking confirmation to users       |
| 🖼️ Image    | Cloudinary         | Upload and serve hotel/room images       |
| 💳 Payment  | Stripe            | Secure card payment processing           |

---

## ✅ Unit Testing

Unit tests were written using **xUnit** and **Moq**, and are automatically executed via **GitHub Actions** as part of the CI workflow.

Test coverage includes:

- **Core Services:**
  - `HotelService`
  - `BookingService`
  - `RoomService`
  - `AuthService`
  - `CityService`
  - `ReviewService`

- **Utility/Integration Services:**
  - `CloudinaryImageService`
  - `EmailService`
  - `PdfReceiptGenerator`

- **And Other Controllers.**
---

## 📁 Project Structure

```bash
TravelEase/
├── API/                # Controllers, Program.cs, JWT setup
├── Application/        # DTOs, Interfaces, Business Logic
├── Domain/             # Entity Models (Hotel, Room, Booking, User)
├── Infrastructure/     # DbContext, Repositories, Email, Cloudinary
├── Tests/              # Unit Tests (xUnit + Moq)
