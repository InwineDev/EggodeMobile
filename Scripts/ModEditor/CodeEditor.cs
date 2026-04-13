using UnityEngine;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

public class CodeEditor : MonoBehaviour
{
    private string code = @"
// Напишите здесь C# код
using UnityEngine;

public class RuntimeScript : MonoBehaviour
{
    void Start()
    {
        // Создаём куб в центре сцены
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);
        cube.GetComponent<Renderer>().material.color = Color.red;
    }

    void Update()
    {
        // Вращаем объект, на котором висит скрипт
        transform.Rotate(Vector3.up * Time.deltaTime * 50f);
    }
}";

    private Vector2 scrollPosition;
    private string compilationMessage = "";

    void OnGUI()
    {
        // Стили для UI
        GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
        textAreaStyle.fontSize = 14;
        textAreaStyle.font = Font.CreateDynamicFontFromOSFont("Consolas", 14); // Моноширинный шрифт

        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Runtime C# Code Editor", GUI.skin.button);

        // Поле для ввода кода
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));
        code = GUILayout.TextArea(code, textAreaStyle, GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();

        // Кнопка для компиляции и выполнения
        if (GUILayout.Button("Compile and Run", GUILayout.Height(40)))
        {
            CompileAndRun();
        }

        // Вывод сообщений компилятора
        if (!string.IsNullOrEmpty(compilationMessage))
        {
            GUILayout.Label(compilationMessage, GUI.skin.label);
        }

        GUILayout.EndVertical();
    }

    private void CompileAndRun()
    {
        // Здесь происходит компиляция кода с помощью Roslyn
        // Для этого нужно добавить в проект Roslyn-компилятор

        // Пример интеграции (требует установки Roslyn пакетов):
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

        // Указываем ссылки на необходимые сборки (включая Unity)
        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(UnityEngine.GameObject).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(UnityEngine.Component).Assembly.Location),
            MetadataReference.CreateFromFile(System.Reflection.Assembly.Load("netstandard, Version=2.0.0.0").Location),
             MetadataReference.CreateFromFile(System.Reflection.Assembly.Load("System.Runtime, Version=4.0.0.0").Location)
        };

        CSharpCompilation compilation = CSharpCompilation.Create(
            "RuntimeAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using (var ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                // Выводим ошибки компиляции
                compilationMessage = "Compilation failed!\n";
                foreach (var diagnostic in result.Diagnostics)
                {
                    compilationMessage += diagnostic.GetMessage() + "\n";
                }
            }
            else
            {
                compilationMessage = "Compilation successful!";
                // Загружаем скомпилированную сборку
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                // Находим скомпилированный класс и добавляем его как компонент
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(MonoBehaviour).IsAssignableFrom(type))
                    {
                        // Создаём новый GameObject и добавляем на него наш скрипт
                        GameObject runtimeObject = new GameObject("RuntimeObject");
                        runtimeObject.AddComponent(type);
                        break;
                    }
                }
            }
        }
    }
}
