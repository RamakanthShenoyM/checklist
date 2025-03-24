To check if a **class has defined an instance method** using **reflection** in C#, you can use `Type.GetMethod` or `Type.GetMethods` with the right `BindingFlags`.

---

### ✅ Example: Check for an Instance Method

Assume you have the following class:

```csharp
public class MyClass
{
    public void Greet() { }

    private void Hidden() { }

    public static void StaticMethod() { }
}
```

### ✅ Code to Check for a Public Instance Method:

```csharp
using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        Type type = typeof(MyClass);

        // Check for a public instance method called "Greet"
        MethodInfo method = type.GetMethod("Greet", BindingFlags.Instance | BindingFlags.Public);

        if (method != null)
        {
            Console.WriteLine("Method 'Greet' exists and is an instance method.");
        }
        else
        {
            Console.WriteLine("Method 'Greet' not found.");
        }
    }
}
```

---

### 🧠 Explanation:

- `BindingFlags.Instance`: Looks for **instance methods** (not static).
- `BindingFlags.Public`: Only includes **public** methods.
- You can also use `BindingFlags.NonPublic` to look for **private/protected/internal** methods.

---

### ✅ Example: Check for a Private Instance Method

```csharp
MethodInfo privateMethod = type.GetMethod("Hidden", BindingFlags.Instance | BindingFlags.NonPublic);

if (privateMethod != null)
{
    Console.WriteLine("Private instance method 'Hidden' exists.");
}
```

---

Let me know if you're checking for overloads, specific signatures, or inherited vs declared methods!