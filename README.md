# 🚀 GPURental - Premium GPU Rental Marketplace

[![.NET Core](https://img.shields.io/badge/.NET%20Core-3.1-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)

**GPURental** is an innovative platform that connects GPU owners with users who need high-performance computing power. It streamlines GPU sharing through intelligent recommendations and real-time performance monitoring, making access to powerful hardware simple and efficient.

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
- **AI Price Suggestions**: AI-powered pricing recommendations based on market data
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

## 📂 Project File Structure

```
GPURental/
├── Controllers/
├── Models/
├── Views/
├── wwwroot/
├── Data/
├── Services/
├── Migrations/
├── Properties/
├── GPURental.csproj
├── appsettings.json
├── Program.cs
├── Startup.cs
└── README.md
```

## 👥 Contributors

- **Man Vadariya** - Design work flow and implementation of controllers
- **Zeel Javia** - Implementation of models and DB
- **Shreyas Patel** - Implementation of views and UI
