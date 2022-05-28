using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyGenerator
{
    public static class ProxyGenerator
    {
        public static T Create<T>(T obj, IInterception interception)
        {
            var type = obj.GetType();
            var interfaces = type.GetInterfaces();

            var assemblyName = new AssemblyName("ProxyAssembly");

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
            assemblyName,
            AssemblyBuilderAccess.RunAndSave
                );

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(
                assemblyName.Name,
                $"{assemblyName.Name}.dll"
                );

            var typeBuilder = moduleBuilder.DefineType(
                $"{type.Name}Proxy",
                type.Attributes,
                typeof(object),
                interfaces
                );

            var wrappedFieldBuilder = typeBuilder.DefineField(
                "wrapped",
                type,
                FieldAttributes.Private | FieldAttributes.InitOnly
                );

            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                new[] { type }
                );

            {
                var il = ctorBuilder.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // load this pointer
                il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)); // constructor of base

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);

                il.Emit(OpCodes.Stfld, wrappedFieldBuilder);

                il.Emit(OpCodes.Ret);
            }

            foreach (var interfaceType in interfaces)
            {
                foreach (var methodInfo in interfaceType.GetMethods())
                {
                    // ------- FETCH METHODS FROM INTERCEPTOR
                    var beforeMethod = interception.GetType().GetMethod("Before");
                    var afterMethod = interception.GetType().GetMethod("After");
                    // -------

                    var parameterTypes = methodInfo.GetParameters()
                        .Select(x => x.ParameterType)
                        .ToArray();

                    var instanceMethod = type.GetMethod(methodInfo.Name, parameterTypes);

                    var methodBuilder = typeBuilder.DefineMethod(
                            instanceMethod.Name,
                            instanceMethod.Attributes,
                            instanceMethod.ReturnType,
                            parameterTypes
                        );

                    var il = methodBuilder.GetILGenerator();

                    // ------- ADD BEFORE METHOD
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, beforeMethod);
                    // -------

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, wrappedFieldBuilder);

                    for (int i = 1; i <= parameterTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg, i);
                    }

                    il.Emit(OpCodes.Callvirt, instanceMethod);

                    // ------- ADD AFTER METHOD
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, afterMethod);
                    // -------

                    il.Emit(OpCodes.Ret);
                }
            }

            var proxyType = typeBuilder.CreateType();
            assemblyBuilder.Save($"{assemblyName.Name}.dll");

            return (T)Activator.CreateInstance(proxyType, obj);
        }
    }
}
