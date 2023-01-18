 # FileLogger
 ***A simple file logger to be used in .NET projects***
 
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
    private readonly FileLogger fileLogger;
    
    public MyClass()
    {
        // pass the log filename and the desired max. log file size in KB to the constructor:
        fileLogger = new FileLogger("MyProgram.log", 256);   
        LogEvent += fileLogger.LogWriteLine;
    }
    
    public event Action<object, LoggerEventArgs> LogEvent;

    void IFileLogger.OnLogEvent(object o, LoggerEventArgs eventArgs)
    {
        RaiseLogEvent(o, eventArgs);
    }

    protected virtual void RaiseLogEvent(object o, LoggerEventArgs eventArgs)
    {
        LogEvent?.Invoke(o, eventArgs);
    }
}
```
</details>

<details>
<summary><b>Raising a LogEvent - normal message (click to expand)</b></summary>

```
var loggerEventArgs = new LoggerEventArgs(
    "My Message",
    GetType().Name,
    MethodBase.GetCurrentMethod().Name,
    null);
RaiseLogEvent(this, loggerEventArgs);
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
        String.Empty,
        GetType().Name,
        MethodBase.GetCurrentMethod().Name,
        ex);
    RaiseLogEvent(this, loggerEventArgs);
}
```
</details>
