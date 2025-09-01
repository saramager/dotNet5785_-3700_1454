
  # Volunteer Management System for New Mothers

A Windows desktop application built as a mini-project for managing volunteer assistance for new mothers.  
The system allows coordinators to manage volunteers, schedule tasks, and track support activities in an intuitive way.

## Features
- **Volunteer & Task Management** – add, update, and delete volunteers or tasks with validation rules.  
- **Layered Architecture (DAL, BL, UI)** – clear separation of data, business logic, and presentation layers.  
- **WPF User Interface** – modern Windows UI using XAML with multiple screens and data binding.  
- **Advanced UI Triggers** – property and event triggers to enhance responsiveness and interactivity.  
- **Data Storage** – XML-based persistence for managing application data.  
- **Security** – password handling with validation and encryption.  
- **API Integration** – includes distance calculations (by air, driving, or walking) via mapping API.  

## Tech Stack
- **Language:** C#  
- **Framework:** .NET (WPF)  
- **UI Markup:** XAML  
- **Data Storage:** XML  
- **Architecture:** 3-Layered (Data Layer, Business Logic, Presentation)  

## Extra Implementations (Bonuses)
- Custom application icon (window title + taskbar).  
- Conditional button visibility depending on business rules.  
- IMultiValueConverter for advanced binding scenarios.  
- Try-catch/finally error handling in BLTest and DLTest.  
- Strong password validation & secure storage.  

## Getting Started
1. Clone the repository:  
   ```bash
   git clone https://github.com/saramager/dotNet5785_-3700_1454.git

# dotNet5785_-3700_1454
dotNet porject 
Hello welcome!
Hii world! 

**Project Bonus Features and Evaluation Criteria**

* Icon Integration (Window Title and Taskbar)
Adds a custom icon to the window title and displays it in the taskbar.
Evaluation Points: 1

* Use of Property Trigger
Trigger based on property value changes to enhance UI responsiveness.
Evaluation Points: 1

* Use of Event Trigger
Trigger based on specific events to improve application interactivity.
Evaluation Points: 1

* IMultiValueConverter Implementation
Handles complex data binding scenarios by combining multiple values into one.
Evaluation Points: 1

* Conditional Button Display in Call Screen
The delete button is displayed only if the call can be deleted.
Evaluation Points: 2

* Conditional Button Display in Volunteer Screen
Allows deleting a volunteer only if no task has ever been assigned to them.
Evaluation Points: 2

* Try-phase use in BLTest and DLTest
Ensures comprehensive error handling using full try- phase blocks (try-finally or try-catch).
Evaluation Points: 1


* Password Property Addition
Includes a secure property for handling user passwords.
Evaluation Points: 2

* Strong Password Validation
Checks for password strength criteria such as length, special characters, and case sensitivity.
Evaluation Points: 1

* Password Encryption
Implements encryption for storing passwords securely.
Evaluation Points: 2

* Distance Calculations
Provides distance computations by air, driving, or walking routes.
Evaluation Points: 3
