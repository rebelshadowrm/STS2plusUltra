using System;
using System.Linq;
using System.Reflection;

namespace STS2Plus.Reflection;

internal static class RuntimeTypeResolver
{
	public static Type? FindType(string fullName)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			try
			{
				Type type = assembly.GetType(fullName, throwOnError: false);
				if (type != null)
				{
					return type;
				}
			}
			catch
			{
			}
		}
		return null;
	}

	public static Type? FindTypeByName(string simpleName)
	{
		string simpleName2 = simpleName;
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			try
			{
				Type type = assembly.GetTypes().FirstOrDefault((Type candidate) => candidate?.Name == simpleName2);
				if (type != null)
				{
					return type;
				}
			}
			catch (ReflectionTypeLoadException ex)
			{
				Type type2 = ex.Types.FirstOrDefault((Type candidate) => candidate?.Name == simpleName2);
				if (type2 != null)
				{
					return type2;
				}
			}
			catch
			{
			}
		}
		return null;
	}
}
