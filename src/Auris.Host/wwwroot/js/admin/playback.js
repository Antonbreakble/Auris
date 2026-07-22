import { audioApi } from './api.js';
import {
    executeWithBusyButton,
    formatDateTime,
    startPolling
} from './shared.js';

const refreshInterval = 1000;

export function initPlayback() {
    const elements = {
        badge: document.getElementById('playback-status-badge'),
        currentFile: document.getElementById('playback-current-file'),
        meta: document.getElementById('playback-current-meta'),
        stopButton: document.getElementById('stop-playback-button')
    };

    if (Object.values(elements).some(element => !element)) {
        return;
    }

    async function refresh() {
        try {
            const status = await audioApi.getStatus();
            renderStatus(elements, status);
        } catch {
            renderError(elements);
        }
    }

    elements.stopButton.addEventListener('click', () => {
        void executeWithBusyButton(
            elements.stopButton,
            'Остановка...',
            async () => {
                await audioApi.stopCurrent();
                await refresh();
            });
    });

    startPolling(refresh, refreshInterval);
}

function renderStatus(elements, status) {
    const isPlaying = status.isPlaying ?? status.IsPlaying;
    const state = status.state ?? status.State ?? 'Idle';
    const fileName = status.fileName ?? status.FileName;
    const currentItemId = status.currentItemId ?? status.CurrentItemId;
    const startedAt = status.startedAt ?? status.StartedAt;

    elements.badge.textContent = state;

    if (!isPlaying) {
        elements.badge.className = 'badge text-bg-secondary';
        elements.currentFile.textContent = 'Сейчас ничего не воспроизводится.';
        elements.meta.textContent = '';
        elements.stopButton.classList.add('d-none');
        return;
    }

    elements.badge.className = 'badge text-bg-success';
    elements.currentFile.textContent = fileName ?? 'Файл воспроизводится';
    
    elements.stopButton.classList.remove('d-none');

    const meta = [];

    if (currentItemId) {
        meta.push(`ItemId: ${currentItemId}`);
    }

    if (startedAt) {
        meta.push(`Started: ${formatDateTime(startedAt)}`);
    }

    elements.meta.textContent = meta.join(' · ');
}

function renderError(elements) {
    elements.badge.textContent = 'Unknown';
    elements.badge.className = 'badge text-bg-warning';

    elements.currentFile.textContent = 'Не удалось получить статус воспроизведения.';

    elements.meta.textContent = '';
    elements.stopButton.classList.add('d-none');
}