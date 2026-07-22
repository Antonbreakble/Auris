class ApiError extends Error {
    constructor(status, message) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
    }
}

async function request(url, options = {}) {
    const response = await fetch(url, options);
    const content = await response.text();

    if (!response.ok) {
        throw new ApiError(
            response.status,
            content || `HTTP error ${response.status}`);
    }

    if (!content) {
        return null;
    }

    return JSON.parse(content);
}

function postJson(url, body) {
    return request(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
    });
}

export const audioApi = {
    getStatus() {
        return request('/api/audio/status');
    },

    async stopCurrent() {
        try {
            await request('/api/audio/current', {
                method: 'DELETE'
            });
        } catch (error) {
            if (error.status !== 404) {
                throw error;
            }
        }
    },

    getQueue() {
        return request('/api/audio/queue');
    },

    clearQueue() {
        return request('/api/audio/queue/clear', {
            method: 'DELETE'
        });
    },

    playFile(fileName) {
        return postJson('/api/audio/play', {
            fileName,
            requestedBy: 'admin'
        });
    }
};

export const textToSpeechApi = {
    play(text) {
        return postJson('/api/tts/play', {
            text,
            requestedBy: 'admin'
        });
    }
};