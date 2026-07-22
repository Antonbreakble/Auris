import { textToSpeechApi } from './api.js';
import { executeWithBusyButton } from './shared.js';

export function initTextToSpeech() {
    const textInput = document.getElementById('tts-text');

    if (!textInput) {
        return null;
    }

    const elements = {
        textInput,
        playButton:
            document.getElementById('tts-play-button'),
        textLength:
            document.getElementById('tts-text-length'),
        message:
            document.getElementById('tts-action-message')
    };

    if (Object.values(elements).some(element => element === null)) {
        console.error(
            'Не удалось инициализировать модуль синтеза речи.',
            elements);

        return null;
    }

    elements.playButton.addEventListener('click', async () => {
        await enqueueTextToSpeech(elements);
    });

    elements.textInput.addEventListener('input', () => {
        updateTextLength(elements);
    });

    elements.textInput.addEventListener('keydown', async event => {
        if (!event.ctrlKey || event.key !== 'Enter') {
            return;
        }

        event.preventDefault();

        await enqueueTextToSpeech(elements);
    });

    updateTextLength(elements);

    return {};
}

async function enqueueTextToSpeech(elements) {
    const text = elements.textInput.value.trim();

    if (!text) {
        showMessage(
            elements.message,
            'Введите текст сообщения.',
            'warning');

        return;
    }

    try {
        await executeWithBusyButton(
            elements.playButton,
            'Добавление...',
            () => textToSpeechApi.play(text));

        showMessage(
            elements.message,
            'Текст добавлен в очередь на озвучивание.',
            'success');

        elements.textInput.value = '';

        updateTextLength(elements);
    } catch (error) {
        showMessage(
            elements.message,
            getErrorMessage(
                error,
                'Не удалось добавить текст в очередь.'),
            'warning');
    }
}

function updateTextLength(elements) {
    const length = elements.textInput.value.length;

    elements.textLength.textContent =
        `${length} символов`;
}

function showMessage(messageElement, message, type) {
    messageElement.textContent = message;

    messageElement.classList.remove(
        'd-none',
        'alert-success',
        'alert-warning');

    messageElement.classList.add(`alert-${type}`);
}

function getErrorMessage(error, defaultMessage) {
    if (error instanceof Error && error.message) {
        return error.message;
    }

    return defaultMessage;
}