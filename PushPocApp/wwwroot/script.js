const logEl = document.getElementById('log');

function log(...args) {
    console.log(...args);
    logEl.textContent += '[' + new Date().toLocaleTimeString() + '] ' + args.map(a => (typeof a === 'object' ? JSON.stringify(a) : a)).join(' ') + '\n';
    logEl.scrollTop = logEl.scrollHeight;
}

function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding).replace(/\-/g, '+').replace(/_/g, '/');
    const raw = window.atob(base64);
    const output = new Uint8Array(raw.length);
    for (let i = 0; i < raw.length; ++i) output[i] = raw.charCodeAt(i);
    return output;
}

// Remove existing service worker (if any) and register a fresh one
async function registerServiceWorker(forceReinstall = false) {
    if (!('serviceWorker' in navigator)) {
        log('Service workers not supported in this browser');
        return null;
    }

    // Unregister existing SW if forced
    if (forceReinstall && navigator.serviceWorker.controller) {
        const registrations = await navigator.serviceWorker.getRegistrations();
        for (const reg of registrations) {
            try {
                await reg.unregister();
                log('Unregistered service worker', reg);
            } catch (err) {
                log('Error unregistering', err);
            }
        }
    }

    try {
        const reg = await navigator.serviceWorker.register('/service_worker.js', { scope: '/' });
        log('Service worker registered with scope', reg.scope);
        return reg;
    } catch (err) {
        log('SW register failed', err);
        throw err;
    }
}

async function getVapidPublicKey() {
    const r = await fetch('/api/push/vapidPublicKey');
    if (!r.ok) throw new Error('Failed to fetch VAPID public key: ' + r.status);
    const key = await r.text();
    log('Fetched VAPID public key (short):', key.substring(0, 20) + '...');
    return key;
}

async function subscribeForPush() {
    try {
        const permission = await Notification.requestPermission();
        log('Notification permission:', permission);
        if (permission !== 'granted') {
            log('Permission not granted. Aborting subscription.');
            return;
        }

        const reg = await registerServiceWorker(false);
        if (!reg) return;

        // If already subscribed, unsubscribe first (cleanup to ensure fresh keys)
        const existing = await reg.pushManager.getSubscription();
        if (existing) {
            log('Existing subscription found - unsubscribing to force fresh subscription');
            await existing.unsubscribe();
        }

        const publicKey = await getVapidPublicKey();
        const subscription = await reg.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: urlBase64ToUint8Array(publicKey)
        });

        log('New subscription created', subscription);

        // Send subscription to backend
        const res = await fetch('/api/push/subscribe', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(subscription)
        });

        const json = await res.json();
        log('Subscribe API response:', json);
    } catch (err) {
        log('subscribeForPush error', err);
    }
}

async function unsubscribe() {
    try {
        const reg = await navigator.serviceWorker.getRegistration();
        if (!reg) {
            log('No service worker registration found');
            return;
        }

        const sub = await reg.pushManager.getSubscription();
        if (!sub) {
            log('No push subscription found to unsubscribe');
            return;
        }

        // Tell server to remove it (best-effort)
        await fetch('/api/push/unsubscribe', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(sub)
        });

        const ok = await sub.unsubscribe();
        log('Unsubscribed locally:', ok);
    } catch (err) {
        log('unsubscribe error', err);
    }
}

async function sendTestNotification() {
    try {
        const title = document.getElementById('title').value;
        const body = document.getElementById('body').value;

        const payload = {
            Title: title,
            Body: body,
            Icon: '/icon.png',
            Url: '/'
        };

        const r = await fetch('/api/push/send', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const json = await r.json();
        log('Send API response', json);
    } catch (err) {
        log('sendTestNotification error', err);
    }
}

document.getElementById('btnRegister').addEventListener('click', async () => {
    await subscribeForPush();
});

document.getElementById('btnUnsubscribe').addEventListener('click', async () => {
    await unsubscribe();
});

document.getElementById('btnSend').addEventListener('click', async () => {
    await sendTestNotification();
});

// On load, show basic info
(async () => {
    log('Client script loaded. Browser:', navigator.userAgent);
    if ('serviceWorker' in navigator) {
        const reg = await navigator.serviceWorker.getRegistration();
        log('Existing Service Worker registration:', !!reg);
    } else {
        log('Service workers not supported in this browser');
    }
})();
