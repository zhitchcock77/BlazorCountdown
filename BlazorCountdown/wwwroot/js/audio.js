// Audio Context
let audioContext = null;

// Initialize audio context on first user interaction
function initAudioContext() {
    if (!audioContext) {
        audioContext = new (window.AudioContext || window.webkitAudioContext)();
    }
    return audioContext;
}

// Synthesize a beep sound
function synthesizeBeep(frequency, duration, type = 'sine', volume = 0.5) {
    const ctx = initAudioContext();
    const oscillator = ctx.createOscillator();
    const gainNode = ctx.createGain();

    oscillator.type = type;
    oscillator.frequency.setValueAtTime(frequency, ctx.currentTime);
    
    gainNode.gain.setValueAtTime(volume, ctx.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(0.01, ctx.currentTime + duration);

    oscillator.connect(gainNode);
    gainNode.connect(ctx.destination);

    oscillator.start();
    oscillator.stop(ctx.currentTime + duration);
}

// Sound definitions
const sounds = {
    'letter-select': () => {
        // High-pitched short beep for letter selection
        synthesizeBeep(880, 0.1, 'sine', 0.3);
    },
    'word-valid': () => {
        // Success sound - two ascending beeps
        synthesizeBeep(440, 0.1, 'sine', 0.3);
        setTimeout(() => synthesizeBeep(880, 0.1, 'sine', 0.3), 100);
    },
    'word-invalid': () => {
        // Error sound - descending tone
        synthesizeBeep(440, 0.1, 'square', 0.3);
        setTimeout(() => synthesizeBeep(220, 0.1, 'square', 0.3), 100);
    },
    'timer-tick': () => {
        // Clock tick sound
        synthesizeBeep(1000, 0.05, 'sine', 0.2);
    },
    'game-over': () => {
        // Game over fanfare
        synthesizeBeep(440, 0.1, 'sine', 0.3);
        setTimeout(() => synthesizeBeep(554.37, 0.1, 'sine', 0.3), 100);
        setTimeout(() => synthesizeBeep(659.25, 0.2, 'sine', 0.3), 200);
    },
    'round-start': () => {
        // Round start flourish
        synthesizeBeep(523.25, 0.1, 'sine', 0.3);
        setTimeout(() => synthesizeBeep(659.25, 0.1, 'sine', 0.3), 100);
        setTimeout(() => synthesizeBeep(783.99, 0.2, 'sine', 0.3), 200);
    }
};

// Play audio function
window.playAudio = async (soundName) => {
    try {
        if (sounds[soundName]) {
            sounds[soundName]();
        }
    } catch (error) {
        console.warn(`Failed to play sound: ${soundName}`, error);
    }
}; 