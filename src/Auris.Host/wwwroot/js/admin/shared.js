export function formatDateTime(value) {
    if (!value) {
        return '';
    }

    const date = new Date(value);

    return Number.isNaN(date.getTime())
        ? ''
        : date.toLocaleString();
}

export function escapeHtml(value) {
    return String(value)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#039;');
}

export async function executeWithBusyButton(button, busyText, action) {

    const originalText = button.textContent;
    
    button.disabled = true;
    button.textContent = busyText;

    try {
        return await action();
    } finally {
        button.disabled = false;
        button.textContent = originalText;
    }
}

export function startPolling(action, intervalMilliseconds) {
    let stopped = false;

    async function tick() {
        try {
            await action();
        } finally {
            if (!stopped) {
                setTimeout(tick, intervalMilliseconds);
            }
        }
    }

    void tick();

    return () => {
        stopped = true;
    };
}