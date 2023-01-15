 # FileLogger
 ***A simple file logger to be used in .NET projects***
 
 Used in my other project [ToggleHypervisor](https://github.com/q-g-j/ToggleHypervisor)
 
 ### Features
 - provides a simple IFileLogger interface
 - trims the log file to a specified size
 - takes simultaneous file access into account
 
 ### Usage
 - download the repo
 - add the project to your VS solution
 - add a reference to this project in the reference manager
 - a class can now inherit from IFileLogger.  Here is a sample implementation:
```
public class MyClass : IFileLogger
{
    private readonly FileLogger fileLogger;
    
    public MyClass()
    {
        fileLogger = new FileLogger();
        LogEvent += fileLogger.LogWriteLine;
    }
    
    public event Action<object, LoggerEventArgs> LogEvent;

    void IFileLogger.OnLogEvent(object o, LoggerEventArgs eventArgs)
    {
        RaiseLogEvent(o, eventArgs);
    }

    LoggerEventArgs IFileLogger.GetEventArgs(string message, string className, string methodName, Exception e)
    {
        return GetLoggerEventArgs(message, className, methodName, e);
    }

    protected virtual void RaiseLogEvent(object o, LoggerEventArgs eventArgs)
    {
        LogEvent?.Invoke(o, eventArgs);
    }

    protected virtual LoggerEventArgs GetLoggerEventArgs(string message, string className, string methodName, Exception e)
    {
        // the second parameter is the desired maximal size of the log file in KB:
        return new LoggerEventArgs("MyProgram.log", 4, message, className, methodName, e);
    }
}
```

Now you can run ```RaiseLogEvent(object o, LoggerEventArgs eventArgs)``` in the implementing class like this:
```
try
{
    // some code
}
catch (Exception ex)
{
    LoggerEventArgs loggerEventArgs = GetLoggerEventArgs(
        String.Empty,
        GetType().Name,
        MethodBase.GetCurrentMethod().Name,
        ex
        );
    RaiseLogEvent(this, loggerEventArgs);
}
```
