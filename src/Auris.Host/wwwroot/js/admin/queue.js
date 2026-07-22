import { audioApi } from './api.js';
import {
    escapeHtml,
    formatDateTime,
    startPolling
} from './shared.js';

const refreshInterval = 1000;

export function initQueue() {
    const elements = {
        count: document.getElementById('queue-count'),
        capacity: document.getElementById('queue-capacity'),
        body: document.getElementById('queue-table-body'),
        empty: document.getElementById('queue-empty'),
        table: document.getElementById('queue-table-wrapper'),
        error: document.getElementById('queue-error'),
        refreshButton: document.getElementById('refresh-queue-button'),
        clearButton: document.getElementById('clear-queue-button')
    };

    if (Object.values(elements).some(element => element === null)) {
        console.error('Не удалось инициализировать модуль очереди:', elements);
    return null;
}

    async function refresh() {
        try {
            const queue = await audioApi.getQueue();

            renderQueue(elements, {
                count: queue.count ?? queue.Count ?? 0,
                capacity: queue.capacity ?? queue.Capacity ?? 0,
                items: queue.items ?? queue.Items ?? []
            });

            hideError(elements);
        } catch {
            showError(elements);
        }
    }

    async function clear() {
        try {
            await audioApi.clearQueue();
            await refresh();
        } catch {
            showError(elements);
        }
    }

    elements.refreshButton?.addEventListener(
        'click',
        () => void refresh());

    elements.clearButton?.addEventListener(
        'click',
        () => void clear());

    startPolling(refresh, refreshInterval);

    return {
        refresh
    };
}

function renderQueue(elements, queue) {
    elements.count.textContent = queue.count;
    elements.capacity.textContent = queue.capacity;

    if (queue.items.length === 0) {
        elements.body.innerHTML = '';
        elements.empty.classList.remove('d-none');
        elements.table.classList.add('d-none');

        return;
    }

    elements.empty.classList.add('d-none');
    elements.table.classList.remove('d-none');

    elements.body.innerHTML =
        queue.items.map(renderQueueRow).join('');
}

function renderQueueRow(item) {
    const id = item.id ?? item.Id ?? '';
    const fileName = item.fileName ?? item.FileName ?? '';
    const createdAt = item.createdAt ?? item.CreatedAt;
    const createdBy = item.createdBy ?? item.CreatedBy ?? '';

    return `
        <tr data-queue-item-id="${escapeHtml(id)}">
            <td>${escapeHtml(fileName)}</td>
            <td>${formatDateTime(createdAt)}</td>
            <td>${escapeHtml(createdBy)}</td>
        </tr>`;
}

function showError(errorElement) {
    errorElement.classList.remove('d-none');
}

function hideError(errorElement) {
    errorElement.classList.add('d-none');
}