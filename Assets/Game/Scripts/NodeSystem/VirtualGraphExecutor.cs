using Mirror;
using RuntimeNodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static scriptor;

public class VirtualGraphExecutor
{
    private RuntimeNodeEditor.GraphData _graphData;
    private Dictionary<string, VirtualNode> _nodes = new Dictionary<string, VirtualNode>();
    private Dictionary<string, string> _socketToNodeMap = new Dictionary<string, string>();
    private List<RuntimeNodeEditor.ConnectionData> connectionDatas = new List<RuntimeNodeEditor.ConnectionData>();
    private Dictionary<string, object> context = new Dictionary<string, object>();

    public void Load(string json, GameObject caller, GameObject triggered)
    {
        if (!NetworkServer.active)
        {
            Debug.LogError("VirtualGraphExecutor should only be used on the server!");
            return;
        }

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Received null or empty JSON for graph execution");
            return;
        }

        try
        {
            _graphData = JsonUtility.FromJson<RuntimeNodeEditor.GraphData>(json);
            if (_graphData == null)
            {
                Debug.LogError("Failed to deserialize graph data");
                return;
            }

            _nodes.Clear();
            _socketToNodeMap.Clear();
            connectionDatas.Clear();
            context.Clear();

            CreateNodes(caller, triggered);
            MapSocketsToNodes();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading graph: {e}");
            throw;
        }
    }

    private void CreateNodes(GameObject caller, GameObject triggered)
    {
        if (_graphData == null || _graphData.nodes == null)
            return;

        if (_graphData.connections != null)
        {
            foreach (var conn in _graphData.connections)
            {
                connectionDatas.Add(conn);
            }
        }

        foreach (var nodeData in _graphData.nodes)
        {
            try
            {
                VirtualNode node = nodeData.path switch
                {
                    "Nodes/OnStartNode" => new VirtualOnStartNode(),
                    "Nodes/OnTriggerEnterNode" => new VirtualOnTriggerEnterNode(),
                    "Nodes/OnCollisionEnterNode" => new VirtualOnCollisionEnterNode(),

                    "Nodes/BooledNode" => new VirtualBooledNode
                    {
                        OpType = int.Parse(nodeData.values.First(v => v.key == "opType").value),
                        ExecuteOnStart = true
                    },

                    "Nodes/FloatNode" => new VirtualFloatNode
                    {
                        Value = GetValue(nodeData.values, "floatValue", 0f),
                        ExecuteOnStart = true
                    },

                    "Nodes/Vector3Node" => new VirtualVectorNode
                    {
                        ExecuteOnStart = true,
                        ExecutionPriority = 6
                    },

                    "Nodes/StringNode" => new VirtualStringNode
                    {
                        Value = GetValue(nodeData.values, "stringValue", "NULL"),
                        ExecuteOnStart = true
                    },

                    "Nodes/GetVarNode" => new VirtualGetValueNode
                    {
                        Value = GetValue(nodeData.values, "keyGet", "NULL"),
                        ExecuteOnStart = true,
                        ExecutionPriority = 5
                    },

                    "Nodes/SetVarNode" => new VirtualSetValueNode
                    {
                        Value = GetValue(nodeData.values, "keySet", "NULL"),
                        ExecutionPriority = 2
                    },

                    "Nodes/IntNode" => new VirtualIntNode
                    {
                        Value = GetValue(nodeData.values, "intValue", 0),
                        ExecuteOnStart = true
                    },

                    "Nodes/BoolNode" => new VirtualBoolNode
                    {
                        Value = GetValue(nodeData.values, "boolValue", false),
                        ExecuteOnStart = true
                    },

                    "Nodes/IntRandomNode" => new VirtualIntRandomNode
                    {
                        ValueFrom = GetValue(nodeData.values, "intValueFrom", 0),
                        ValueTo = GetValue(nodeData.values, "intValueTo", 0),
                        ExecuteOnStart = true
                    },

                    "Nodes/FloatRandomNode" => new VirtualFloatRandomNode
                    {
                        ValueFrom = GetValue(nodeData.values, "floatValueFrom", 0f),
                        ValueTo = GetValue(nodeData.values, "floatValueTo", 0f),
                        ExecuteOnStart = true
                    },

                    "Nodes/DestroyNode" => new VirtualDestroyNode(),
                    "Nodes/DamageNode" => new VirtualDamageNode(),
                    "Nodes/TpNode" => new VirtualTpNode(),
                    "Nodes/ShowMessageNode" => new VirtualShowMessageNode(),
                    "Nodes/ChangeGravityNode" => new VirtualChangeGravityNode(),

                    "Nodes/ThisObjectNode" => new VirtualThisObjectNode
                    {
                        ExecuteOnStart = true
                    },

                    "Nodes/ObjectFromIdNode" => new VirtualObjectFromIdNode
                    {
                        Value = GetValue(nodeData.values, "idValue", ""),
                        ExecuteOnStart = true
                    },

                    "Nodes/TimeNode" => new VirtualTimeSleepNode(),
                    "Nodes/IfNode" => new VirtualIfNode(),
                    "Nodes/ForNode" => new VirtualForNode(),

                    _ => null
                };

                if (node != null)
                {
                    node.Id = nodeData.id;
                    node.OutputSocketIds = nodeData.outputSocketIds ?? new List<string>();
                    node.InputSocketIds = nodeData.inputSocketIds ?? new List<string>();

                    foreach (var item1 in connectionDatas)
                    {
                        if (nodeData.outputSocketIds != null && nodeData.outputSocketIds.Contains(item1.outputSocketId))
                            node.SocketsFromOutputIds.Add(item1.id);

                        if (nodeData.inputSocketIds != null && nodeData.inputSocketIds.Contains(item1.inputSocketId))
                            node.SocketsFromInputIds.Add(item1.id);
                    }

                    node.SetExecutor(this);
                    _nodes.Add(node.Id, node);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating node {nodeData.id}: {e}");
            }
        }
    }

    private T GetValue<T>(SerializedValue[] values, string key, T defaultValue = default)
    {
        var value = values?.FirstOrDefault(v => v.key == key);
        if (value == null)
            return defaultValue;

        try
        {
            return (T)Convert.ChangeType(value.value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    private void MapSocketsToNodes()
    {
        if (_graphData == null || _graphData.nodes == null)
            return;

        foreach (var node in _graphData.nodes)
        {
            if (node.inputSocketIds != null)
            {
                foreach (var socketId in node.inputSocketIds)
                    _socketToNodeMap[socketId] = node.id;
            }

            if (node.outputSocketIds != null)
            {
                foreach (var socketId in node.outputSocketIds)
                    _socketToNodeMap[socketId] = node.id;
            }
        }
    }

    public void Execute(GameObject caller, string startNode, GameObject triggered)
    {
        if (!NetworkServer.active || _graphData == null || _nodes.Count == 0)
            return;

        var startNodes = _graphData.nodes?
            .Where(n => n.path == startNode)
            .Select(n => _nodes.TryGetValue(n.id, out var node) ? node : null)
            .Where(n => n != null);

        if (startNodes == null)
            return;

        foreach (var node in startNodes)
        {
            try
            {
                var nodesToExecute = _nodes.Values.OrderBy(n => n.ExecutionPriority);
                foreach (var node1 in nodesToExecute)
                {
                    if (node1.ExecuteOnStart)
                        node1.Execute(context, caller, triggered);
                }

                node.Execute(context, caller, triggered);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error executing node {node.Id}: {e}");
            }
        }
    }

    public void ExecuteConnections(VirtualNode node, Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var outputSocketId in node.OutputSocketIds)
        {
            var connections = _graphData.connections.Where(c => c.outputSocketId == outputSocketId);

            foreach (var conn in connections)
            {
                if (_socketToNodeMap.TryGetValue(conn.inputSocketId, out var nextNodeId))
                {
                    _nodes[nextNodeId].Execute(context, caller, triggered);
                }
            }
        }
    }

    public void ExecuteConnections(VirtualNode node, Dictionary<string, object> context, GameObject caller, GameObject triggered, int specificOutputSocketId)
    {
        if (node.OutputSocketIds == null || specificOutputSocketId < 0 || specificOutputSocketId >= node.OutputSocketIds.Count)
            return;

        var connections = _graphData.connections.Where(c => c.outputSocketId == node.OutputSocketIds[specificOutputSocketId]);

        foreach (var conn in connections)
        {
            if (_socketToNodeMap.TryGetValue(conn.inputSocketId, out var nextNodeId))
            {
                _nodes[nextNodeId].Execute(context, caller, triggered);
            }
        }
    }
}