# 🚀 Web Push Notifications POC (.NET + VAPID)

This is a **Proof of Concept (POC)** project that demonstrates how to implement **Web Push Notifications** in a browser using **ASP.NET Core (Controller-based API)**, **WebPush (tpeczek)**, and **VAPID authentication**.  
It shows how a client (browser) can **subscribe** to push notifications and how the backend can **send messages** to those subscriptions.

---

## 📌 Features
- Generate **VAPID keys** (for secure push authentication).
- Register **Service Worker** to handle push events in the browser.
- Subscribe browser clients using the **Push API**.
- Store client subscription objects in `subscriptions.json`.
- Send push notifications with **WebPush (tpeczek)** library.
- Display system notifications (tested on **macOS**, works on Windows/Linux too).
- Includes full working frontend (`index.html`, `script.js`, `service-worker.js`).

---

## 📂 Project Structure

PushPoCApp/
│
├── Controllers/
│ └── PushController.cs # API for subscription + sending notifications
│
├── wwwroot/
│ ├── index.html # UI page with Subscribe button
│ ├── script.js # Handles Push API + Service Worker
│ ├── service-worker.js # Background worker to receive notifications
│ └── icon.png # Notification icon
│
├── vapid.json # Generated VAPID keys (⚠️ DO NOT COMMIT)
├── subscriptions.json # Stores client subscriptions (runtime file)
├── Program.cs
├── PushPoCApp.csproj
└── README.md

yaml
Copy code

---

## ⚙️ Setup Instructions

### 1. Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [VS Code](https://code.visualstudio.com/) or any IDE
- Modern browser (Chrome, Firefox, Edge — **Safari excluded** for now)

---

### 2. Clone & Build

```bash
git clone https://github.com/your-username/PushNotificationPOC.git
cd PushNotificationPOC
dotnet restore
3. Generate VAPID Keys
Run the project once to auto-generate vapid.json:

bash
Copy code
dotnet run
Example output:

pgsql
Copy code
Generated VAPID keys and saved to vapid.json
Now listening on: http://localhost:5087
⚠️ Important: Never commit vapid.json (private key must remain secret).

4. Run the App
bash
Copy code
dotnet run
Then open http://localhost:5087 in your browser.

5. Subscribe & Test Push
Click the "Subscribe" button in the UI.

The browser will ask for Notification Permission → Allow.

A subscription object will be stored in subscriptions.json.

To send a push notification:

http
Copy code
GET http://localhost:5087/api/push/send
You should now see a system notification on your OS.

⚠️ Notes & Limitations
Requires HTTPS in production (service workers + push API need SSL).

Works on Chrome, Firefox, Edge.

Safari uses APNs (Apple Push Notification Service) — not covered here.

Notifications may behave differently across OS (auto-dismiss vs. sticky).

Subscriptions should be stored in a database (this demo uses subscriptions.json).

📚 References
[MDN: Push API](https://developer.mozilla.org/en-US/docs/Web/API/Push_API?utm_source=chatgpt.com)

[MDN: Notifications API](https://developer.mozilla.org/en-US/docs/Web/API/Notifications_API)

[WebPush .NET (tpeczek)](https://github.com/web-push-libs/web-push-csharp)

[Google Web Fundamentals - Push Notifications](https://web.dev/push-notifications-overview/)