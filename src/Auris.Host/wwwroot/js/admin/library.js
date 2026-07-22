import { audioApi } from './api.js';
import { executeWithBusyButton } from './shared.js';

const messageHideDelayMilliseconds = 1000;

let messageHideTimerId;

export function initLibrary() {
    const messageElement = document.getElementById('library-action-message');

    if (!messageElement) {
        return null;
    }

    const playButtons = document.querySelectorAll('.js-play-library-file');

    for (const button of playButtons) {
        button.addEventListener('click', async () => {
            await playLibraryFile(button, messageElement);
        });
    }

    return {};
}

async function playLibraryFile(button, messageElement) {
    const fileName = button.dataset.audioFileName;

    if (!fileName) {
        showMessage(
            messageElement,
            'Не указано имя файла.',
            'warning');

        return;
    }

    try {
        await executeWithBusyButton(
            button,
            'Добавление...',
            () => audioApi.playFile(fileName));

        showMessage(
            messageElement,
            `Файл "${fileName}" добавлен в очередь.`,
            'success');
    } catch (error) {
        showMessage(
            messageElement,
            getErrorMessage(
                error,
                'Не удалось добавить файл в очередь.'),
            'warning');
    }
}

function showMessage(messageElement, message, type) {
    window.clearTimeout(messageHideTimerId);

    messageElement.textContent = message;

    messageElement.classList.remove(
        'd-none',
        'alert-success',
        'alert-warning');

    messageElement.classList.add(`alert-${type}`);

    messageHideTimerId = window.setTimeout(() => {
        messageElement.classList.add('d-none');
    }, messageHideDelayMilliseconds);
}

function getErrorMessage(error, defaultMessage) {
    if (error instanceof Error && error.message) {
        return error.message;
    }

    return defaultMessage;
}