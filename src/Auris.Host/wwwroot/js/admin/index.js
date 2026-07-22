import { initPlayback } from './playback.js';
import { initQueue } from './queue.js';
import { initLibrary } from './library.js';
import { initTextToSpeech } from './text-to-speech.js';

initPlayback();

const queue = initQueue();

initLibrary({
    onFileQueued: queue?.refresh
});

initTextToSpeech();