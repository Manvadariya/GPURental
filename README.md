# 🚀 GPURental - Premium GPU Rental Marketplace

[![.NET Core](https://img.shields.io/badge/.NET%20Core-3.1-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)

**GPURental** is a comprehensive cloud-based marketplace that connects GPU owners with users who need high-performance computing power. Our platform enables seamless GPU rental transactions with AI-powered recommendations, real-time monitoring, and secure payment processing.

## 🌟 Project Overview

GPURental revolutionizes access to GPU computing by creating a decentralized marketplace where:
- **GPU Providers** can monetize their idle hardware by listing GPUs for rent
- **Renters** can access high-performance computing power on-demand without massive upfront investments
- **Businesses & Researchers** can scale their AI/ML workloads cost-effectively

The platform features role-based access control, intelligent pricing suggestions, AI-powered GPU matching, and comprehensive analytics dashboards.

## ✨ Key Features

### 🔍 **Smart GPU Discovery**
- **AI-Powered Search**: Describe your task in natural language and let our Gemini AI find the perfect GPU
- **Manual Filtering**: Search by GPU model, VRAM, location, and price range
- **Detailed Specifications**: View comprehensive hardware specs including GPU model, VRAM, RAM, storage, and CPU details

### 💰 **Intelligent Pricing System**
- **AI Price Suggestions**: Machine learning-powered pricing recommendations based on market data
- **Real-time Market Analysis**: Dynamic pricing based on supply and demand
- **Transparent Cost Calculation**: Clear hourly pricing with no hidden fees

### 👥 **Multi-Role Architecture**
- **Renters**: Browse marketplace, rent GPUs, manage active jobs, track spending
- **Providers**: List GPUs, manage availability, monitor earnings, handle disputes
- **Administrators**: Platform oversight, dispute resolution, user management

### 📊 **Comprehensive Dashboards**
- **Provider Analytics**: Earnings distribution, usage statistics, listing performance
- **Renter Insights**: Spending analysis, job history, GPU usage patterns
- **Real-time Monitoring**: Live job status, resource utilization tracking

### 🔒 **Security & Trust**
- **ASP.NET Identity Integration**: Secure user authentication and authorization
- **Role-based Access Control**: Granular permissions for different user types
- **Dispute Resolution System**: Built-in conflict resolution mechanism
- **Secure Payment Processing**: Wallet-based transaction system

### 🎯 **Advanced Features**
- **Review & Rating System**: Community-driven quality assurance
- **Real-time Job Management**: Start, stop, and monitor rental jobs
- **Detailed Analytics**: Interactive charts and performance metrics
- **Image Upload Support**: Visual GPU listing presentations

## 🛠️ Technology Stack

### **Backend**
- **Framework**: ASP.NET Core 3.1 MVC
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server with Entity Framework Core
- **AI Integration**: Google Gemini AI for intelligent recommendations

### **Frontend**
- **UI Framework**: Bootstrap 4 with custom CSS
- **JavaScript**: jQuery for dynamic interactions
- **Charts**: Chart.js for analytics visualization
- **Icons**: Lucide React icon library

### **Architecture Patterns**
- **MVC Architecture**: Clean separation of concerns
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling and testability
- **Service Layer**: Business logic encapsulation

## 🚀 Getting Started

### Prerequisites
- **.NET Core 3.1 SDK** or later
- **SQL Server** (LocalDB/Express/Full version)
- **Visual Studio 2019/2022** or **VS Code**
- **Git** for version control

### Installation Steps

1. **Clone the Repository**

2. **Configure Database Connection**
- Update the connection string in `appsettings.json`:

3. **Set up Gemini AI (Optional)**
- Obtain a Gemini API key from Google AI Studio
   - Add to `appsettings.json`:

4. **Install Dependencies**

5. **Apply Database Migrations**

6. **Run the Application**

7. **Access the Application**
- Open your browser and navigate to `https://localhost:5001`
- Register as a new user or use existing credentials

### 🗄️ Database Setup

The application uses Entity Framework Code-First migrations. The database will be automatically created with:
- **User Management**: Authentication, roles, and profiles
- **GPU Listings**: Hardware specifications and availability
- **Rental Jobs**: Job lifecycle and payment tracking
- **Reviews & Disputes**: Community feedback and conflict resolution
- **Wallet System**: Payment and earnings management

### 🔧 Configuration Options

1. **Email Settings** (for notifications)
2. **Payment Gateway Integration** (for production)
3. **AI Service Configuration** (Gemini API settings)
4. **Logging Configuration** (Application insights, file logging)

## 📱 Usage Guide

### For GPU Renters
1. **Register/Login** to your account
2. **Browse Marketplace** or use AI search to find suitable GPUs
3. **Fund Your Wallet** with sufficient balance
4. **Rent a GPU** by selecting duration and confirming payment
5. **Monitor Your Job** through the dashboard
6. **Leave Reviews** to help the community

### For GPU Providers
1. **Create Provider Account** (upgrade from renter)
2. **List Your GPU** with detailed specifications and pricing
3. **Manage Availability** and respond to rental requests
4. **Monitor Earnings** through comprehensive analytics
5. **Handle Customer Support** and resolve any disputes

### For Administrators
1. **Access Admin Dashboard** with elevated privileges
2. **Monitor Platform Health** and user activities
3. **Resolve Disputes** between renters and providers
4. **Manage User Accounts** and handle platform issues

## 🏗️ Project Structure
