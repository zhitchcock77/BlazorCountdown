using Microsoft.JSInterop;

namespace BlazorCountdown.Services
{
    public interface IAudioService
    {
        Task PlayLetterSelectSound();
        Task PlayWordSubmitSound(bool isValid);
        Task PlayTimerTickSound();
        Task PlayGameOverSound();
        Task PlayRoundStartSound();
    }

    public class AudioService : IAudioService
    {
        private readonly IJSRuntime _jsRuntime;

        public AudioService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task PlayLetterSelectSound()
        {
            await _jsRuntime.InvokeVoidAsync("playAudio", "letter-select");
        }

        public async Task PlayWordSubmitSound(bool isValid)
        {
            await _jsRuntime.InvokeVoidAsync("playAudio", isValid ? "word-valid" : "word-invalid");
        }

        public async Task PlayTimerTickSound()
        {
            await _jsRuntime.InvokeVoidAsync("playAudio", "timer-tick");
        }

        public async Task PlayGameOverSound()
        {
            await _jsRuntime.InvokeVoidAsync("playAudio", "game-over");
        }

        public async Task PlayRoundStartSound()
        {
            await _jsRuntime.InvokeVoidAsync("playAudio", "round-start");
        }
    }
} 