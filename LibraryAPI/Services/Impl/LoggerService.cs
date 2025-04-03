namespace LibraryAPI.Services;

public class LoggerService : ILoggerService {
    public void Log(string message){
        Console.WriteLine($"[LoggerService] {DateTime.Now}: {message}");
    }

}