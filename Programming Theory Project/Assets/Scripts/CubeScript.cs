using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ABSTRACTION
public class CubeScript
{
    public delegate void ProcessSymbol(string symbol);
    public delegate void CommandExecutor(CommandContext context);

    Dictionary<string, ProcessSymbol> Parser;
    CommandContext m_CommandContext;
    public CommandExecutor OnCommandComplete;

    Queue<CommandContext> m_CurrentQueue;

    public CubeScript()
    {
        Parser = new Dictionary<string, ProcessSymbol>();
        Parser.Add("R", new ProcessSymbol(OnPlaneSymbol));
        Parser.Add("L", new ProcessSymbol(OnPlaneSymbol));
        Parser.Add("U", new ProcessSymbol(OnPlaneSymbol));
        Parser.Add("D", new ProcessSymbol(OnPlaneSymbol));
        Parser.Add("F", new ProcessSymbol(OnPlaneSymbol));
        Parser.Add("B", new ProcessSymbol(OnPlaneSymbol));
        Parser.Add("'", new ProcessSymbol(OnDirectionSymbol));
        Parser.Add("2", new ProcessSymbol(OnDoubleSymbol));
        Parser.Add("EOT", new ProcessSymbol(OnTokenEnd));
    }

    public void Parse(string input) {
        ParseInput(input.Trim());
    }

    public Queue<CommandContext> ParseIntoQueue(string input) {
        m_CurrentQueue = new Queue<CommandContext>();
        OnCommandComplete += EnqueueCommandContext;
        return null;
    }

    private void EnqueueCommandContext(CommandContext context) {
        // default command complete handler
    }

    void InvokeHandler()
    {
        OnCommandComplete?.Invoke(new CommandContext {
            Plane = m_CommandContext.Plane,
            RotationDirection = m_CommandContext.RotationDirection,
            IsExecutedTwice = m_CommandContext.IsExecutedTwice
        });
    }

    void OnTokenEnd(string symbol)
    {
        if (OnCommandComplete == null) {
            Debug.Log("OnTokenEnd: no execution delegate is set!");
            return;
        }
        InvokeHandler();
        if (m_CommandContext.IsExecutedTwice)
            InvokeHandler();
    } 

    void OnPlaneSymbol(string symbol)
    {
        switch(symbol){
            case "R":
                m_CommandContext.Plane = Vector3.right;
                break;
            case "L":
                m_CommandContext.Plane = Vector3.left;
                break;
            case "U":
                m_CommandContext.Plane = Vector3.up;
                break;
            case "D":
                m_CommandContext.Plane = Vector3.down;
                break;
            case "F":
                m_CommandContext.Plane = Vector3.back;
                break;
            case "B":
                m_CommandContext.Plane = Vector3.forward;
                break;
            default:
                Debug.Log("Unknown plane symbol found: " + symbol);
                break;
        }
    }

    void OnDirectionSymbol(string symbol)
    {
        if (m_CommandContext.Plane == null) {
            Debug.Log("Direction Symbol without preceeding plane found.");
            return;
        }
        m_CommandContext.RotationDirection = Direction.CCW;
    }

    void OnDoubleSymbol(string symbol)
    {
        m_CommandContext.IsExecutedTwice = true;
    }

    void OnToken(string token)
    {
        foreach(char c in token.ToCharArray()) {
            if (!Parser.ContainsKey(c+"")) {
                Debug.Log("Unknown char found: \"" + c + "\", token: " + token);
                return;
            }
            Parser[c + ""].Invoke(""+ c);
        }
        Parser["EOT"].Invoke(token);
        m_CommandContext = new CommandContext();
    }

    void ParseInput(string input)
    {
        m_CommandContext = new CommandContext();
        System.StringSplitOptions options = System.StringSplitOptions.RemoveEmptyEntries;
        foreach (string token in input.Split(new char[]{' '}, options)) {
            OnToken(token);
        }
    }

    public class CommandContext {
        public Vector3 Plane;
        public Direction RotationDirection = Direction.CW;
        public bool IsExecutedTwice;
        // POLYMORPHISM
        public override string ToString()
        {
            return $"CommandContext: Plane = {Plane}; RotationDirection = {RotationDirection.ToString()}; IsExecutedTwice = {IsExecutedTwice}";
        }
    }
}