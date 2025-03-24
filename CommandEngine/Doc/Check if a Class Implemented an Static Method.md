To invoke a **static method** in a class using **reflection** in C#, you can follow these steps:

### ✅ Example:

Assume you have the following class:

```csharp
public class MyClass
{
    public static void SayHello(string name)
    {
        Console.WriteLine($"Hello, {name}!");
    }
}
```

### ✅ Using Reflection to Call the Static Method:

```csharp
using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        // Get the Type object for the class
        Type type = typeof(MyClass);

        // Get MethodInfo for the static method
        MethodInfo method = type.GetMethod("SayHello", BindingFlags.Public | BindingFlags.Static);

        if (method != null)
        {
            // Invoke the static method (null for the instance since it's static)
            method.Invoke(null, new object[] { "Alice" });
        }
    }
}
```

---

### 🔑 Key Points:

- `typeof(MyClass)` gets the `Type` object for the class.
- `BindingFlags.Public | BindingFlags.Static` is used to get **public static** methods.
- Use `null` for the **object instance** parameter in `Invoke`, since static methods don't need an instance.
- Parameters for the method are passed as an `object[]`.

Let me know if you're trying to invoke a private static method or one with different parameters—happy to tailor it!