To check if a class has **implemented an interface** using **reflection** in C#, you can use the `Type.GetInterfaces()` method or `Type.IsAssignableFrom()`.

---

### ✅ Example:

Assume you have this interface and class:

```csharp
public interface IMyInterface
{
    void DoSomething();
}

public class MyClass : IMyInterface
{
    public void DoSomething() { }
}
```

---

### ✅ Method 1: Using `GetInterfaces()`

```csharp
using System;

class Program
{
    static void Main()
    {
        Type type = typeof(MyClass);

        // Get all interfaces implemented by MyClass
        Type[] interfaces = type.GetInterfaces();

        foreach (Type iface in interfaces)
        {
            if (iface == typeof(IMyInterface))
            {
                Console.WriteLine("MyClass implements IMyInterface.");
            }
        }
    }
}
```

---

### ✅ Method 2: Using `IsAssignableFrom()`

```csharp
Type type = typeof(MyClass);
bool implements = typeof(IMyInterface).IsAssignableFrom(type);

if (implements)
{
    Console.WriteLine("MyClass implements IMyInterface.");
}
```

> 🔹 `IsAssignableFrom()` is useful when you have a reference to the interface type and want to see if a class can be assigned to it (i.e. implements it).

---

Let me know if you want to check for **generic interfaces**, **explicit implementations**, or **inherited interface implementations** too!