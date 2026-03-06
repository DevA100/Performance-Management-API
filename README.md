# Performance Management App
## Deployment Note

The original system was deployed within an on-premise enterprise environment.

Sensitive data and integrations have been mocked for this public repository.

**Automated Performance Survey & Appraisal System for Organizations**

---

## Overview
The **Performance Management App** is a robust platform designed to **evaluate, monitor, and report employee and management performance** across departments and executive committees. It streamlines survey-based appraisals, tracks progress, and provides detailed audit reports of quarterly performance.

The app is **role-based and secure**, providing administrators with flexible control over surveys, questions, and departmental setups, while ensuring staff can participate efficiently and transparently.

---

## Key Features

- **Performance Surveys**: Collect feedback from staff and executives across multiple departments.  
- **Flexible Question Management**: Admin can create, modify, and scale survey questions.  
- **Department & User Management**: Administrators can create departments and assign users.  
- **Audit Reports**: Generate detailed reports on performance trends and survey outcomes.  
- **Role-Based Access Control**: Authentication and authorization using JWT tokens for secure access.  
- **Notifications System**: Alerts and reminders for users to participate in surveys.  
- **Middleware Security**: Multiple middlewares implemented to mitigate vulnerabilities.  
- **Dockerized Deployment**: Easy setup and consistent environment across development and production.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend API | .NET Web API, Entity Framework |
| Frontend | React (consumes the API separately) |
| Database | PostgreSQL (development), SQL Server (production) |
| Authentication | JWT Token with role-based access |
| Deployment | Docker containers |
| Security | Multiple middleware for vulnerability mitigation |

---

## Problem → Action → Result

**Problem:** Manual performance appraisals were time-consuming, error-prone, and lacked structured reporting across departments.  

**Action:** Developed a **web API with React front-end** to automate surveys, manage departments and roles, secure sensitive endpoints, and provide robust reporting dashboards.  

**Result:**  
- Reduced performance appraisal cycle from **weeks to days**.  
- Enabled **secure and scalable survey management** across departments.  
- Improved **auditability and reporting transparency** for management.  

---

## Screenshots / Demo


!PerformanceSurvey\PerformanceSurvey\Docs


**Live Demo (Mock / Dockerized Version):** [ link here]

---

## How to Run Locally

1. Clone the repository:

```bash
git clone https://github.com/yourusername/pPerformance-Management-API
.git
cd performance-survey-app
