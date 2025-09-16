ººWeb Push Notifications POC (.NET 8 + VAPID)ºº

This is a Proof of Concept (POC) project demonstrating how to implement Web Push Notifications using ASP.NET Core Web API, WebPush (tpeczek)
, and VAPID authentication.

It shows how a browser client can subscribe to push notifications, and how the backend can send messages to those subscriptions.

📌 Features

	🔑 Generate VAPID keys (for secure push authentication).

	🛠️ Service Worker to handle background push events.

	🌐 Push API subscription from the browser.

	💾 Store client subscriptions in subscriptions.json (demo only).

	📡 Send push notifications using the WebPush library.

	💻 Tested on macOS, also works on Windows/Linux (Chrome, Firefox, Edge).

📂 Project Structure

	PushPocApp.sln
	global.json                # Locks SDK to .NET 8.0.405
	.gitignore                 # Ignores bin/, obj/, .vs/, etc.
	README.md                  # Project documentation
	PushPocApp/
	│
	├── PushPocApp.csproj      # Project file
	├── Program.cs             # Entry point (minimal API setup)
	│
	├── Controllers/
	│   └── PushController.cs  # API for subscription + sending notifications
 	│
	├── Models/
 	│   ├── NotificationPayload.cs  # API Models that contain payload for send notification
	│   └── SubscriptionDto.cs      # API Model that contain subscribed push notifications objects
  
	│
	├── wwwroot/
	│   ├── index.html         # UI with Subscribe button
	│   ├── script.js          # Handles Push API + Service Worker
	│   ├── service-worker.js  # Background push handler
	│   └── icon.png           # Notification icon
	│
	├── vapid.json             # Generated VAPID keys (⚠️ DO NOT COMMIT)
	└── subscriptions.json     # Stores client subscriptions (runtime file)


⚙️ Setup Instructions

1. Prerequisites

	1. .NET 8 SDK (pinned via global.json)

	2. Visual Studio Code or Visual Studio 2022

	3. A modern browser (Chrome, Firefox, Edge — Safari not supported)
 
2. Clone & Build


		git clone https://github.com/ajkushwaha12/PushNotificationPOC.git
		cd PushNotificationPOC
		dotnet restore


3. Generate VAPID Keys

	Run the project once — it will auto-generate vapid.json:

		dotnet run --project PushPocApp


	Example output:

		Generated VAPID keys and saved to vapid.json
		Now listening on: http://localhost:5087

4. Run the App

		dotnet run --project PushPocApp


	Then open your browser at:
			
		 👉 http://localhost:5087

5. Subscribe & Test Push

	1. Open the app in your browser.

	2. Click Subscribe → grant notification permission.

	3. A subscription object will be stored in subscriptions.json.

	4. Send a push notification via API:

			POST http://localhost:5087/api/push/send


You should now see a system notification 🚀


📚 References

RFC 8030 – HTTP Web Push
		
	https://datatracker.ietf.org/doc/html/rfc8030

RFC 8291 – Message Encryption for Web Push

	https://datatracker.ietf.org/doc/html/rfc8291

RFC 8292 – VAPID (Voluntary Application Server Identification)

	https://datatracker.ietf.org/doc/html/rfc8292

Push API – W3C

	https://www.w3.org/TR/push-api/

