# EventEase | Enterprise Event Management System

**Project Code:** CLDV6211-Part-2-2026-POE  
**Student ID:** ST10441951  
**Author:** Joshua Marc Lourens  
**Academic Institution:** Emeris Westville - School of Computer Science  

---

## 🌟 Project Vision
**EventEase Elite** is a professional-grade administrative portal designed for university-scale resource management. This application facilitates the seamless coordination of physical venues, complex event scheduling, and high-precision booking management. 

By transitioning from a local relational foundation (Part 1) to a cloud-integrated infrastructure (Part 2), EventEase Elite ensures data durability through Azure Blob Storage and system stability via advanced conflict detection algorithms.

---

## 🚀 Evolution: From Part 1 to Part 2

### Phase 1: Relational Architecture (The Foundation)
In the initial development phase, the system focused on establishing a robust relational schema using **Entity Framework Core** and **SQL Server**:
* **Normalized Database:** Implementation of a three-table structure consisting of `Venues`, `Events`, and `Bookings`.
* **Administrative CRUD:** Full Create, Read, Update, and Delete capabilities for all modules.
* **Navigation Properties:** Utilizing Eager Loading (`.Include()`) to maintain relational links between events and their physical locations.

### Phase 2: Cloud Integration & Hardened Logic (The Upgrade)
Part 2 introduces enterprise-level features and cloud scaling:
* **Azure Blob Storage:** Migration of venue imagery from local server storage to the cloud. The system utilizes the **Azurite emulator** and **Azure Storage Explorer** for localized cloud development.
* **Inclusive Conflict Detection:** Hardened scheduling logic in the `BookingsController` that blocks overlapping reservations using the inclusive formula `(StartA <= EndB) && (EndA >= StartB)`.
* **Referential Integrity Gates:** Implementation of "Specialist Checks" to prevent orphaned data (e.g., blocking the deletion of an Event if active Bookings are linked).
* **Enhanced Search Ledger:** A consolidated administrative view allowing specialists to filter the global ledger by **Booking ID** or **Event Name**.

---

## 🛠 Tech Stack

| Component | Technology |
| :--- | :--- |
| **Framework** | ASP.NET Core 10.0 (MVC) |
| **Language** | C# |
| **Database** | SQL Server via Entity Framework Core |
| **Cloud Infrastructure** | Azure Blob Storage (via Azurite Emulator) |
| **Frontend** | Bootstrap 5, Nunito Typography, CSS3 |
| **Development** | Visual Studio 2022, Azure Storage Explorer |

---

## 📂 System Architecture & Logic

### 1. Cloud-Based Venue Management
Each venue record acts as the "Infrastructure" for the system. Images are no longer stored in the database but are uploaded to **Azure Blob Storage**. The application generates a unique URI for every image, ensuring high-speed delivery and storage efficiency.

### 2. Event Definition & Constraints
Events represent the scheduled operations of the university. To maintain data integrity, every event must be linked to an existing **Venue**. The system prevents the creation of "orphaned" events that have no physical location.

### 3. Booking Ledger & Conflict Prevention
The core administrative engine. The system enforces strict scheduling rules:
* **Double-Booking Prevention:** The controller scans the database for any overlapping timestamps at the selected venue.
* **Verification Status:** Bookings progress through a lifecycle of `Pending`, `Confirmed`, and `Cancelled`.
* **Eager Loading:** The search ledger consolidates data from all three tables into a single, searchable administrative view.

---

## ⚙️ Installation & Setup

### Prerequisites
* .NET SDK 10.0 or higher
* SQL Server Express / LocalDB
* [Azurite Storage Emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite)
* [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage-explorer/)

### Steps
1.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/](https://github.com/)[Your-Username]/EventEase-Elite.git
    ```
2.  **Initialize Azure Emulation:**
    * Launch Azurite (or the VS Code extension).
    * Ensure Blob Service is running on port `10000`.
3.  **Database Configuration:**
    * Update the connection string in `appsettings.json` to point to your local SQL instance.
    * Run `Update-Database` in the Package Manager Console.
4.  **Launch:**
    * Open the solution in Visual Studio 2022 and press `F5`.

---

## 📚 Technical References & Citations

* **Azure Architecture Center.** (2026). *Managing Concurrency in Cloud-Based Scheduling Systems.*
* **Microsoft Learn.** (2026). *Azure Blob Storage client library for .NET.*
* **Bootstrap.** (2026). *v5.3 Documentation: Component Layouts and Grid Systems.*
* **Claude AI.** (2026). *Logic Auditing and Architectural Pattern Consulting.*

---

**© 2026 Joshua Marc Lourens | ST10441951** *Westville, KwaZulu-Natal, South Africa*
