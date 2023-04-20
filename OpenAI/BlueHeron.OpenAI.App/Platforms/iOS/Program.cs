using ObjCRuntime;
using UIKit;

namespace BlueHeron.OpenAI;

public class Program
{
    // Main entry point of the application.
    private static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate" you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
