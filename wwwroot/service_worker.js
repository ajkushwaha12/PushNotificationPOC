self.addEventListener('push', function(event) {
  console.log('[sw] push event received', event);
  let data = { title: 'No payload', body: 'Empty body', icon: '/icon.png', url: '/' };
  try {
    if (event.data) {
      data = event.data.json();
    }
  } catch (err) {
    console.error('[sw] error parsing push data', err);
  }
  console.log('data recieved from send api', data);
  const options = {
    body: data.body || '',
    icon: data.icon || '/icon.png',
    data: data.url || '/',
    badge: data.icon || '/icon.png', // badge for some systems
    requireInteraction: false // allow auto-dismiss
  };

  event.waitUntil(self.registration.showNotification(data.title || 'Notification', options));
});

self.addEventListener('notificationclick', function(event) {
  event.notification.close();
  const url = event.notification.data || '/';
  event.waitUntil(
    clients.matchAll({ type: 'window', includeUncontrolled: true }).then(windowClients => {
      for (let client of windowClients) {
        if (client.url === url && 'focus' in client) {
          return client.focus();
        }
      }
      if (clients.openWindow) return clients.openWindow(url);
    })
  );
});
