# StarKNET Logging

A lightweight, thread-safe logging library for .NET applications that provides file-based logging with console output redirection capabilities.

## Features

- **File-based logging** with automatic directory creation
- **Console output redirection** - capture all console output to log files
- **Thread-safe operations** with lock-based synchronization
- **Configurable timestamping** for log entries
- **Dual output support** - write to both file and console simultaneously
- **Automatic log cleanup** - removes old log files on startup
- **Exception logging** with stack trace capture

## Quick Start

### Basic Usage

```csharp
using StarKNET.Logging;

// Create logger with default configuration
var logger = new Logger();

// Log a simple message
logger.Log("Application started");

// Log an error with exception
try 
{
    // Some operation that might fail
    throw new InvalidOperationException("Something went wrong");
}
catch (Exception ex)
{
    logger.LogError("Operation failed", ex);
}

// Clean up
logger.Dispose();
```

### Console Output Redirection

```csharp
var logger = new Logger();

// Start capturing all console output
logger.CaptureConsoleOutput();

// All Console.WriteLine calls will now be logged to file
Console.WriteLine("This will be captured in the log file");
Console.WriteLine("So will this message");

// Stop capturing (optional - automatically done on dispose)
logger.StopCaptureConsoleOutput();

logger.Dispose();
```

### Custom Configuration

```csharp
var config = new LogConfiguration
{
    LogDirectory = @"C:\MyApp\Logs",
    LogFile = "myapp",
    IncludeTimestamps = true,
    WriteToConsole = false  // Only write to file, not console
};

var logger = new Logger(config);
logger.Log("Custom configured message");
```

## Configuration Options

The `LogConfiguration` class provides the following options:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LogDirectory` | `string` | `{AppDirectory}/log` | Directory where log files will be stored |
| `LogFile` | `string` | `"application"` | Base name for log files (`.log` extension added automatically) |
| `IncludeTimestamps` | `bool` | `true` | Whether to prepend timestamps to log entries |
| `WriteToConsole` | `bool` | `true` | Whether to also output log messages to console |

## API Reference

### Logger Class

#### Constructors

```csharp
Logger()                          // Uses default configuration
Logger(LogConfiguration config)   // Uses custom configuration
```

#### Methods

```csharp
void Log(string message)                        // Log a message
void LogError(string message, Exception ex)    // Log an error with optional exception
void CaptureConsoleOutput()                     // Start capturing console output
void StopCaptureConsoleOutput()                 // Stop capturing console output  
void Dispose()                                  // Clean up resources
```

### LogConfiguration Class

```csharp
public class LogConfiguration
{
    public string LogDirectory { get; set; }
    public string LogFile { get; set; }
    public bool IncludeTimestamps { get; set; }
    public bool WriteToConsole { get; set; }
}
```

## Log Format

### Standard Log Entry
```
[2024-05-23 14:30:25] Your log message here
```

### Console Output Entry
```
[2024-05-23 14:30:25] [Console] Console output captured here
```

### Error Entry
```
[2024-05-23 14:30:25] ERROR: Operation failed - Exception: Something went wrong
StackTrace: at MyApp.Program.Main() in Program.cs:line 15
```

## File Behavior

- **Automatic directory creation**: Creates the log directory if it doesn't exist
- **Log file replacement**: Deletes existing log file on startup for fresh logs
- **Thread-safe writing**: Multiple threads can safely write to the same logger instance
- **Automatic cleanup**: Properly disposes resources when the logger is disposed

## Best Practices

### Using with Dependency Injection

```csharp
// In your DI container setup
services.AddSingleton<ILogger>(provider => 
{
    var config = provider.GetService<LogConfiguration>();
    var logger = new Logger(config);
    logger.CaptureConsoleOutput();
    return logger;
});
```

### Using with `using` Statement

```csharp
using (var logger = new Logger())
{
    logger.CaptureConsoleOutput();
    
    // Your application logic here
    logger.Log("Application running");
    
    // Automatic cleanup when exiting using block
}
```

### Exception Handling

```csharp
try
{
    // Risky operation
    PerformOperation();
}
catch (Exception ex)
{
    logger.LogError("Failed to perform operation", ex);
    // Handle or rethrow as needed
}
```

## Thread Safety

The logger is fully thread-safe and can be used from multiple threads simultaneously. All logging operations are protected by internal locking mechanisms.

## Requirements

- .NET Framework 4.5+ or .NET Core 2.0+
- Write permissions to the specified log directory

## Error Handling

The logger handles common errors gracefully:

- **Directory creation failures**: Throws exception with details
- **File access issues**: Logs error to console and continues
- **Null parameter protection**: Validates input parameters

## License

This project is part of the StarKNET framework.
