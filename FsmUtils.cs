using HutongGames.PlayMaker;
using Mono.Posix;

internal static class FsmUtils
{
    public static void Init()
    {
        PatchParse.RegisterToString<FsmState>(ParseFsmState);
        PatchParse.RegisterToString<PlayMakerFSM>(ParsePlayMakerFSM);
        PatchParse.RegisterToString<FsmEvent>(ParseFsmEvent);
        PatchParse.RegisterToString<NamedVariable>(ParseNamedVariable);
        PatchParse.RegisterToString<FsmTransition>(ParseFsmTransition);
        PatchParse.Register(typeof(FsmEvent), FsmEvent.GetFsmEvent, SimpleParseFsmEvent);
    }
    internal static string ParseFsmState(object state)
    {
        return "<color=grey>Fsm State: </color><color=green>" + ((FsmState)state).name + "</color>";
    }
    internal static string ParseFsmEvent(object ev)
    {
        return "<color=grey>Fsm Event: </color><color=green>" + ((FsmEvent)ev).Name + "</color>";
    }
    internal static string ParseNamedVariable(object v)
    {
        NamedVariable nv = (NamedVariable)v;
        return string.Format("<color=grey>Fsm Variable(</color><color=green>{0}</color><color=grey>): </color><color=green>{1}</color> | ", nv.VariableType, nv.Name) +
        UniverseLib.Utility.ToStringUtility.ToStringWithType(nv.RawValue, typeof(object), true);
    }
    internal static string ParsePlayMakerFSM(object fsm)
    {
        return "<color=grey>PlayMakerFSM: </color><color=green>" + ((PlayMakerFSM)fsm).name + "</color>" + "<color=grey>(</color><color=#2df7b2>" + ((PlayMakerFSM)fsm).FsmName + "</color><color=grey>)</color>";
    }
    internal static string ParseFsmTransition(object t)
    {
        if (t is FsmTransition tran)
            return "<color=grey>Fsm Transition: </color><color=green>" + (tran.FsmEvent?.Name ?? tran.EventName) + "</color><color=grey> -> </color><color=green>" + (tran.ToFsmState?.Name ?? tran.ToState) + "</color>";
        return null;
    }
    internal static string SimpleParseFsmEvent(object ev)
    {
        return ((FsmEvent)ev).Name;
    }

}