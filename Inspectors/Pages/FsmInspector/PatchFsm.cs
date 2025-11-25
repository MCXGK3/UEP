using System;
using System.Collections.Generic;
using HarmonyLib;
using HutongGames.PlayMaker;

[HarmonyPatch]
public static class FsmWatcher
{
    class FsmInfo
    {
        internal FsmInfo(OperationBlock block)
        {
            writer = block;
        }
        internal bool is_event = false;
        internal bool global_event = false;
        internal string set_state_name = "";
        internal OperationBlock writer;
        internal string event_name = "";
        internal string event_state_name = "";
        internal string last_state_name = "";



    }
    static readonly Dictionary<PlayMakerFSM, FsmInfo> fsms = new();

    public static void Register(PlayMakerFSM fsm, OperationBlock block)
    {
        if (block == null) return;
        if (!fsms.ContainsKey(fsm))
        {
            fsms.Add(fsm, new(block));
        }
    }
    public static void Unregister(PlayMakerFSM fsm)
    {
        fsms.Remove(fsm);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fsm), "DoTransition", [typeof(FsmTransition), typeof(bool)])]
    public static bool PrefixDoTransition(Fsm __instance, FsmTransition transition, bool isGlobal)
    {
        try
        {
            if (transition.toFsmState == null) return true;
            if (fsms.TryGetValue(__instance.FsmComponent, out FsmInfo info))
            {
                info.is_event = true;
                info.event_name = transition.EventName;
                info.global_event = isGlobal;
                info.event_state_name = transition.toState;
                info.last_state_name = __instance.activeStateName;
            }
        }
        catch (Exception)
        {

        }
        return true;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fsm), "SetState", [typeof(string)])]
    public static bool PrefixSetState(Fsm __instance, string stateName)
    {
        try
        {
            if (fsms.TryGetValue(__instance.FsmComponent, out FsmInfo info))
            {
                info.is_event = false;
                info.set_state_name = stateName;
                info.last_state_name = __instance.activeStateName;
            }
        }
        catch
        {

        }
        return true;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fsm), "EnterState", MethodType.Normal)]
    public static bool PrefixEnterState(Fsm __instance, FsmState state)
    {
        try
        {
            if (state == null) return true;
            if (fsms.TryGetValue(__instance.FsmComponent, out FsmInfo info))
            {
                if (state.loopCount >= __instance.MaxLoopCount)
                {

                }
                else
                {
                    if (info.is_event && (info.event_state_name == state.Name))
                    {
                        info.writer.AddEventRecord(info.global_event, info.last_state_name, info.event_name, info.event_state_name);
                    }
                    else if (!info.is_event && info.set_state_name == state.Name)
                    {
                        info.writer.AddSetStateRecord(info.set_state_name);
                    }
                    else
                    {
                        info.writer.AddErrorRecord("UnRecorded EnterState: " + state.Name);
                    }
                }
            }
        }
        catch
        {

        }
        return true;
    }



}