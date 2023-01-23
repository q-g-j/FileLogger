 # FileLogger
 ***A simple event based file logger to be used in .NET projects***
 
 Copyright 2023 Jann Emken
 
 Used in my other project [ToggleHypervisor](https://github.com/q-g-j/ToggleHypervisor)
 
 ### Features
 - provides an **IFileLogger** interface
 - trims the log file to a specified size (by removing old entries)
 - takes simultaneous file access into account
 
 ### Usage
 - download the repo
 - add the project to your VS solution
 - add a reference to this project in the reference manager
 - any class can now inherit from **IFileLogger**.  Here is a sample implementation:
 
<details>
<summary><b>Interface implementation (click to expand)</b></summary>

```
public class MyClass : IFileLogger
{    
    public MyClass()
    {
        LogEvent += FileLogger.LogWriteLine;
    }

    public event Action<string, int, LoggerEventArgs> LogEvent;

    void IFileLogger.OnLogEvent(string logFileFullPath, int maxLogFileSize, LoggerEventArgs eventArgs)
    {
        RaiseLogEvent(logFileFullPath, maxLogFileSize, eventArgs);
    }

    protected virtual void RaiseLogEvent(string logFileFullPath, int maxLogFileSize, LoggerEventArgs eventArgs)
    {
        LogEvent?.Invoke(logFileFullPath, maxLogFileSize, eventArgs);
    }
}
```
</details>

<details>
<summary><b>Raising a LogEvent - normal message (click to expand)</b></summary>

```
var loggerEventArgs = new LoggerEventArgs(
    "My Message",
    GetType().Name,  // returns the current class name
    MethodBase.GetCurrentMethod().Name,  // returns the calling method's name
    null);
RaiseLogEvent("MyProgram.log", 256, loggerEventArgs);
```
</details>

<details>
<summary><b>Raising a LogEvent - exception message from a catch block (click to expand)</b></summary>

```
try
{
    // some code
}
catch (Exception ex)
{
    var loggerEventArgs = new LoggerEventArgs(
        String.Empty,  // will not be processed, if the last parameter (Exception ex) is not null
        GetType().Name,  // returns the current class name
        MethodBase.GetCurrentMethod().Name,  // returns the calling method's name
        ex);  // only the exception type and the Exception.Message property will be used
    RaiseLogEvent("MyProgram.log", 256, loggerEventArgs);
}
```
</details>

### Example Log Message (when passing an exception):
```
 20.01.2023 17:08:25 - Error in class SettingsFileWriter, method Write: UnauthorizedAccessException: Access to the path "C:\Users\User\AppData\Roaming\ToggleHypervisor\Settings.json" is denied.
 ```
