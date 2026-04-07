using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scriptor;

public abstract class VirtualNode
{
    public string Id { get; set; }
    public List<string> SocketsFromOutputIds { get; set; } = new List<string>();
    public List<string> SocketsFromInputIds { get; set; } = new List<string>();
    public List<string> OutputSocketIds { get; set; } = new List<string>();
    public List<string> InputSocketIds { get; set; } = new List<string>();
    public bool ExecuteOnStart { get; set; }
    public int ExecutionPriority { get; set; }

    public abstract void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered);

    protected VirtualGraphExecutor Executor { get; private set; }

    public void SetExecutor(VirtualGraphExecutor executor)
    {
        Executor = executor;
    }

    protected T GetInputValue<T>(Dictionary<string, object> context, string socketId, T defaultValue = default)
    {
        if (context.TryGetValue(socketId, out var value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка преобразования значения для сокета {socketId}: {ex.Message}");
            }
        }

        return defaultValue;
    }

    protected void SetOutputValue(Dictionary<string, object> context, string socketId, object value)
    {
        context[socketId] = value;
        Debug.Log(context[socketId]);
    }

    protected virtual void ProcessInputs(Dictionary<string, object> context)
    {
        foreach (var inputSocketId in InputSocketIds)
        {
            if (!context.ContainsKey(inputSocketId))
            {
                Debug.LogWarning($"Не найдено значение для входа {inputSocketId} в ноде {GetType().Name}");
            }
        }
    }
}

public class VirtualOnStartNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Нода выполнена при старте! Объект вызова: {caller.name}");
        Executor?.ExecuteConnections(this, context, caller, triggered);
    }
}

public class VirtualOnTriggerEnterNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Нода выполнена после trigger! Объект вызова: {caller.name}");
        Executor?.ExecuteConnections(this, context, caller, triggered);
    }
}

public class VirtualOnCollisionEnterNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Нода выполнена после collision! Объект вызова: {caller.name}");
        Executor?.ExecuteConnections(this, context, caller, triggered);
    }
}

public class VirtualDestroyNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log("Пытаюсь удалить объекты");

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is List<GameObject> gameObj)
            {
                foreach (var item1 in gameObj)
                {
                    if (item1 != null)
                    {
                        NetworkServer.Destroy(item1);
                        Debug.Log($"Объект уничтожен: {item1.name}");
                    }
                }

                Executor?.ExecuteConnections(this, context, caller, triggered);
            }
        }
    }
}

public class VirtualChangeGravityNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log("Пытаюсь поменять isKinematic");

        GameObject gObj = null;
        bool gBool = false;

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is GameObject gameObj)
            {
                gObj = gameObj;
            }

            if (context.TryGetValue(socketsId, out var booling) && booling is bool booled)
            {
                gBool = booled;
            }
        }

        try
        {
            if (gObj != null)
            {
                Rigidbody rb = gObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = gBool;
                    Debug.Log($"Установлен isKinematic={gBool} на объекте {gObj.name}");
                    Executor?.ExecuteConnections(this, context, caller, triggered);
                }
                else
                {
                    Debug.LogWarning($"На объекте {gObj.name} нет Rigidbody");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка VirtualChangeGravityNode: {ex}");
        }
    }
}

public class VirtualDamageNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log("Пытаюсь нанести урон");

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is int uron)
            {
                try
                {
                    if (triggered == null)
                        continue;

                    HealthPlayer healthPlayer = triggered.GetComponent<HealthPlayer>();
                    bool canDamage = serverProperties.instance != null && serverProperties.instance.hp;

                    if (canDamage && healthPlayer != null)
                    {
                        healthPlayer.health -= uron;

                        if (healthPlayer.health <= 0)
                        {
                            healthPlayer.health = 100;
                        }

                        if (healthPlayer.hp != null)
                        {
                            healthPlayer.hp.text = $"{healthPlayer.health} HP";
                        }

                        Debug.Log($"Урон нанесён: {uron}");
                        Executor?.ExecuteConnections(this, context, caller, triggered);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка VirtualDamageNode: {ex}");
                }
            }
        }
    }
}

public class VirtualShowMessageNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log("Пытаюсь показать сообщение игроку");

        string txt = "null";
        float time = 0f;

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var fl) && fl is float ti)
            {
                time = ti;
            }

            if (context.TryGetValue(socketsId, out var str) && str is string text)
            {
                txt = text;
            }
        }

        try
        {
            if (triggered != null)
            {
                var usc = triggered.GetComponent<userSettingNotCam>();
                if (usc != null && usc.messageController != null)
                {
                    usc.messageController.ShowMessage(txt, time);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка VirtualShowMessageNode: {ex}");
        }
    }
}

public class VirtualTpNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log("Пытаюсь телепортировать объект");

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is Vector3 position)
            {
                try
                {
                    if (triggered != null)
                    {
                        triggered.transform.position = position;
                        Debug.Log($"Объект телепортирован в {position}");
                        Executor?.ExecuteConnections(this, context, caller, triggered);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка телепорта: {ex}");
                }
            }
        }
    }
}

public class VirtualFloatNode : VirtualNode
{
    public float Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"FloatNode записал {Value} в {socketsId}");
        }
    }
}

public class VirtualVectorNode : VirtualNode
{
    private float x;
    private float y;
    private float z;

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        try
        {
            if (SocketsFromInputIds.Count > 0 &&
                context.TryGetValue(SocketsFromInputIds[0], out var fl0) && fl0 is float ti0)
            {
                x = ti0;
            }

            if (SocketsFromInputIds.Count > 1 &&
                context.TryGetValue(SocketsFromInputIds[1], out var fl1) && fl1 is float ti1)
            {
                y = ti1;
            }

            if (SocketsFromInputIds.Count > 2 &&
                context.TryGetValue(SocketsFromInputIds[2], out var fl2) && fl2 is float ti2)
            {
                z = ti2;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка VirtualVectorNode: {ex}");
        }

        Vector3 vector = new Vector3(x, y, z);

        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, vector);
            Debug.Log($"VectorNode записал {vector} в {socketsId}");
        }
    }
}

public class VirtualBoolNode : VirtualNode
{
    public bool Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"BoolNode записал {Value} в {socketsId}");
        }
    }
}

public class VirtualBooledNode : VirtualNode
{
    public int OpType { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        object x = null;
        object y = null;
        bool result = false;

        try
        {
            if (SocketsFromInputIds.Count > 0 && context.TryGetValue(SocketsFromInputIds[0], out var val1))
            {
                x = val1;
            }

            if (SocketsFromInputIds.Count > 1 && context.TryGetValue(SocketsFromInputIds[1], out var val2))
            {
                y = val2;
            }

            result = PerformOperation(x, y, OpType);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка в VirtualBooledNode: {ex.Message}");
            result = false;
        }

        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, result);
            Debug.Log($"BooledNode записал {result} в {socketsId}");
        }
    }

    private bool PerformOperation(object x, object y, int opType)
    {
        if (x is string strX && y is string strY)
        {
            return PerformStringOperation(strX, strY, opType);
        }
        else if (IsNumber(x) && IsNumber(y))
        {
            return PerformNumberOperation(x, y, opType);
        }
        else
        {
            return PerformUniversalOperation(x, y, opType);
        }
    }

    private bool PerformStringOperation(string x, string y, int opType)
    {
        switch (opType)
        {
            case 0:
                return string.Equals(x, y, StringComparison.Ordinal);
            case 1:
                return string.Compare(x, y, StringComparison.Ordinal) > 0;
            case 2:
                return string.Compare(x, y, StringComparison.Ordinal) < 0;
            case 3:
                return !string.Equals(x, y, StringComparison.Ordinal);
            default:
                Debug.LogWarning($"Неизвестный тип операции для строк: {opType}");
                return false;
        }
    }

    private bool PerformNumberOperation(object x, object y, int opType)
    {
        double numX = Convert.ToDouble(x);
        double numY = Convert.ToDouble(y);

        switch (opType)
        {
            case 0:
                return Math.Abs(numX - numY) < 0.000001;
            case 1:
                return numX > numY;
            case 2:
                return numX < numY;
            case 3:
                return Math.Abs(numX - numY) > 0.000001;
            default:
                Debug.LogWarning($"Неизвестный тип операции для чисел: {opType}");
                return false;
        }
    }

    private bool PerformUniversalOperation(object x, object y, int opType)
    {
        switch (opType)
        {
            case 0:
                return Equals(x, y);
            case 1:
                try
                {
                    double numX = Convert.ToDouble(x);
                    double numY = Convert.ToDouble(y);
                    return numX > numY;
                }
                catch
                {
                    Debug.LogWarning("Не удалось сравнить значения для операции >");
                    return false;
                }
            case 2:
                try
                {
                    double numX = Convert.ToDouble(x);
                    double numY = Convert.ToDouble(y);
                    return numX < numY;
                }
                catch
                {
                    Debug.LogWarning("Не удалось сравнить значения для операции <");
                    return false;
                }
            case 3:
                return !Equals(x, y);
            default:
                Debug.LogWarning($"Неизвестный тип операции: {opType}");
                return false;
        }
    }

    private bool IsNumber(object value)
    {
        return value is int || value is float || value is double || value is decimal ||
               value is long || value is short || value is byte;
    }
}

public class VirtualIntNode : VirtualNode
{
    public int Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"IntNode записал {Value} в {socketsId}");
        }
    }
}

public class VirtualStringNode : VirtualNode
{
    public string Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"StringNode записал {Value} в {socketsId}");
        }
    }
}

public class VirtualGetValueNode : VirtualNode
{
    public string Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        try
        {
            List<GameObject> obj = null;

            foreach (var socketsId in SocketsFromInputIds)
            {
                if (context.TryGetValue(socketsId, out var fl) && fl is List<GameObject> ti)
                {
                    obj = ti;
                }
            }

            if (obj == null || obj.Count == 0 || obj[0] == null)
                return;

            VarDictionary varDictionary = obj[0].GetComponent<VarDictionary>();
            if (varDictionary == null || varDictionary.values == null || !varDictionary.values.ContainsKey(Value))
                return;

            var outVar = varDictionary.values[Value];

            foreach (var socketsId in SocketsFromOutputIds)
            {
                SetOutputValue(context, socketsId, outVar);
                Debug.Log($"GetValueNode записал {outVar} в {socketsId}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GetValueNode ошибка: {e}");
        }
    }
}

public class VirtualSetValueNode : VirtualNode
{
    public string Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        try
        {
            List<GameObject> obj = null;
            object newVar = null;

            foreach (var socketsId in SocketsFromInputIds)
            {
                if (context.TryGetValue(socketsId, out var fl) && fl is List<GameObject> ti)
                {
                    obj = ti;
                }

                if (context.TryGetValue(socketsId, out var outVar))
                {
                    newVar = outVar;
                }
            }

            if (obj == null)
                return;

            foreach (var item1 in obj)
            {
                if (item1 == null)
                    continue;

                VarDictionary varDictionary = item1.GetComponent<VarDictionary>();
                if (varDictionary == null || varDictionary.values == null)
                    continue;

                varDictionary.values[Value] = newVar;
                Debug.Log($"SetValueNode: ключ {Value}, значение {newVar}, объект {item1.name}");
            }

            Executor?.ExecuteConnections(this, context, caller, triggered);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка VirtualSetValueNode: {ex}");
        }
    }
}

public class VirtualIntRandomNode : VirtualNode
{
    public int ValueFrom { get; set; }
    public int ValueTo { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            int newValue = UnityEngine.Random.Range(ValueFrom, ValueTo);
            SetOutputValue(context, socketsId, newValue);
            Debug.Log($"IntRandomNode записал {newValue} в {socketsId}");
        }
    }
}

public class VirtualFloatRandomNode : VirtualNode
{
    public float ValueFrom { get; set; }
    public float ValueTo { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            float newValue = UnityEngine.Random.Range(ValueFrom, ValueTo);
            SetOutputValue(context, socketsId, newValue);
            Debug.Log($"FloatRandomNode записал {newValue} в {socketsId}");
        }
    }
}

public class VirtualThisObjectNode : VirtualNode
{
    public float Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        GameObject gameObjectCaller = caller as GameObject;
        List<GameObject> list = new List<GameObject>();

        if (gameObjectCaller == null)
        {
            Debug.LogError("VirtualThisObjectNode: caller is not a GameObject");
            return;
        }

        list.Add(gameObjectCaller);

        foreach (var outputSocketId in SocketsFromOutputIds)
        {
            context[outputSocketId] = list;
            Debug.Log($"VirtualThisObjectNode записал ссылку на {gameObjectCaller.name} в {outputSocketId}");
        }
    }
}

public class VirtualTimeSleepNode : VirtualNode
{
    public float DelaySeconds { get; set; } = 0f;

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is float floatValue)
            {
                try
                {
                    DelaySeconds = Convert.ToSingle(floatValue);
                }
                catch
                {
                    Debug.Log($"Ошибка преобразования DelaySeconds: {floatValue}");
                }
            }
        }

        Debug.Log($"Задержка: {DelaySeconds}");

        if (ModLoader.instance != null)
        {
            ModLoader.instance.StartCoroutine(DelayCoroutine(context, caller, triggered));
        }
        else
        {
            Debug.LogError("ModLoader.instance == null");
        }
    }

    private IEnumerator DelayCoroutine(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        yield return new WaitForSeconds(DelaySeconds);
        Debug.Log("TimeSleepNode завершил ожидание");
        Executor?.ExecuteConnections(this, context, caller, triggered);
    }
}

public class VirtualIfNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        bool ifval = false;

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var fl) && fl is bool ti)
            {
                ifval = ti;
            }
        }

        if (ifval)
        {
            Executor?.ExecuteConnections(this, context, caller, triggered, 0);
        }
        else
        {
            Executor?.ExecuteConnections(this, context, caller, triggered, 1);
        }
    }
}

public class VirtualForNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        int howMany = 0;

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var fl) && fl is int ti)
            {
                howMany = ti;
            }
        }

        for (int i = 0; i < howMany; i++)
        {
            Executor?.ExecuteConnections(this, context, caller, triggered, 0);
        }

        Executor?.ExecuteConnections(this, context, caller, triggered, 1);
    }
}

public class VirtualMathNode : VirtualNode
{
    public enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public Operation OpType;

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        if (context.TryGetValue("inputA", out var aObj) &&
            context.TryGetValue("inputB", out var bObj))
        {
            float a = Convert.ToSingle(aObj);
            float b = Convert.ToSingle(bObj);

            float result = OpType switch
            {
                Operation.Add => a + b,
                Operation.Subtract => a - b,
                Operation.Multiply => a * b,
                Operation.Divide => b != 0 ? a / b : 0,
                _ => 0
            };

            Debug.Log($"Результат: {result} (объект {caller.name})");
            context["output"] = result;
        }
    }
}

public class VirtualObjectFromIdNode : VirtualNode
{
    public string Value { get; set; }
    public name24 fromId { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        List<GameObject> fromIds = new List<GameObject>();

        if (serverProperties.instance == null || serverProperties.instance.allBlocks == null)
        {
            Debug.LogError("serverProperties.instance или allBlocks == null");
            return;
        }

        foreach (var item1 in serverProperties.instance.allBlocks)
        {
            if (item1 != null && item1.id == Value)
            {
                fromId = item1;
                fromIds.Add(item1.gameObject);
            }
        }

        if (fromId == null)
        {
            Debug.LogError("VirtualObjectFromIdNode: объект с таким id не найден");
            return;
        }

        foreach (var outputSocketId in SocketsFromOutputIds)
        {
            context[outputSocketId] = fromIds;
            Debug.Log($"VirtualObjectFromIdNode записал {fromIds.Count} объектов в {outputSocketId}");
        }
    }
}