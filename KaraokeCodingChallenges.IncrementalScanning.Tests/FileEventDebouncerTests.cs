using KaraokeCodingChallenges.IncrementalScanning.Services;

namespace KaraokeCodingChallenges.IncrementalScanning.Tests;

public class FileEventDebouncerTests
{
    [Fact]
    public void Constructor_WithPositiveInterval_DoesNotThrow()
    {
        TimeSpan interval = TimeSpan.FromMilliseconds(500);

        FileEventDebouncer debouncer = new FileEventDebouncer(interval);

        Assert.NotNull(debouncer);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_WithInvalidInterval_ThrowsArgumentOutOfRangeException(int milliseconds)
    {
        TimeSpan interval = TimeSpan.FromMilliseconds(milliseconds);

        Assert.Throws<ArgumentOutOfRangeException>(() => new FileEventDebouncer(interval));
    }

    [Fact]
    public async Task DebounceAsync_WithSingleEvent_ExecutesCallback()
    {
        FileEventDebouncer debouncer = new FileEventDebouncer(TimeSpan.FromMilliseconds(50));

        int callbackCount = 0;

        await debouncer.DebounceAsync("song1.mp3", () =>
        {
            callbackCount++;
            return Task.CompletedTask;
        });

        Assert.Equal(1, callbackCount);
    }

    [Fact]
    public async Task DebounceAsync_WithRepeatedEvents_ExecutesOnlyLatestCallback()
    {
        FileEventDebouncer debouncer = new FileEventDebouncer(TimeSpan.FromMilliseconds(100));

        int callbackCount = 0;

        Task firstEvent = debouncer.DebounceAsync("song1.mp3", () =>
        {
            Interlocked.Increment(ref callbackCount);
            return Task.CompletedTask;
        });

        await Task.Delay(20);

        Task secondEvent = debouncer.DebounceAsync("song1.mp3", () =>
        {
            Interlocked.Increment(ref callbackCount);
            return Task.CompletedTask;
        });

        await Task.WhenAll(firstEvent, secondEvent);

        Assert.Equal(1, callbackCount);
    }

    [Fact]
    public async Task DebounceAsync_WithDifferentFiles_ExecutesBothCallbacks()
    {
        FileEventDebouncer debouncer = new FileEventDebouncer(TimeSpan.FromMilliseconds(100));

        int callbackCount = 0;

        Task firstEvent = debouncer.DebounceAsync("song1.mp3", () =>
        {
            Interlocked.Increment(ref callbackCount);
            return Task.CompletedTask;
        });

        await Task.Delay(20);

        Task secondEvent = debouncer.DebounceAsync("song2.mp3", () =>
        {
            Interlocked.Increment(ref callbackCount);
            return Task.CompletedTask;
        });

        await Task.WhenAll(firstEvent, secondEvent);

        Assert.Equal(2, callbackCount);
    }

    [Fact]
    public async Task DebounceAsync_WhenCallbackAlreadyStarted_AllowsCompletion()
    {
        FileEventDebouncer debouncer = new FileEventDebouncer(TimeSpan.FromMilliseconds(50));

        TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);

        TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);

        int callbackCount = 0;

        Task firstEvent = debouncer.DebounceAsync("song1.mp3", async () =>
        {
            started.SetResult();

            await release.Task;

            Interlocked.Increment(ref callbackCount);
        });

        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Task secondEvent = debouncer.DebounceAsync("song1.mp3", () =>
        {
            Interlocked.Increment(ref callbackCount);
            return Task.CompletedTask;
        });

        release.SetResult();

        await Task.WhenAll(firstEvent, secondEvent);

        Assert.Equal(2, callbackCount);
    }
}
