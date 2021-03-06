﻿using FabricAdcHub.User.Machinery.Building;

namespace FabricAdcHub.User.Machinery.ObjectOriented
{
    public static class StateMachineExtensions
    {
        public static IStateBuilder<TState, TEvent, TEventParameter>
            ConfigureState<TState, TEvent, TEventParameter>(
                this StateMachine<TState, TEvent, TEventParameter> stateMachine,
                StateBase<TState, TEvent, TEventParameter> state)
        {
            return stateMachine.ConfigureState(state.State)
                .OnEntry(state.OnEntry)
                .OnExit(state.OnExit);
        }

        public static IStateBuilder<TState, TEvent, TEventParameter>
            ConfigureTransition<TState, TEvent, TEventParameter>(
                this IStateBuilder<TState, TEvent, TEventParameter> stateBuilder,
                TransitionBase<TState, TEvent, TEventParameter> transition)
        {
            return stateBuilder.SwitchTo(transition.Destination, transition.Trigger, transition.Guard, transition.Effect);
        }

        public static IStateBuilder<TState, TEvent, TEventParameter>
            ConfigureIfChoice<TState, TEvent, TEventParameter>(
                this IStateBuilder<TState, TEvent, TEventParameter> stateBuilder,
                IfChoiceBase<TState, TEvent, TEventParameter> ifChoice)
        {
            stateBuilder
                .ChoiceSwitchTo(ifChoice.Trigger)
                .SwitchTo(ifChoice.IfDestination, ifChoice.Guard, ifChoice.IfEffect)
                .ElseSwitchTo(ifChoice.ElseDestination, ifChoice.ElseEffect);
            return stateBuilder;
        }

        public static IStateBuilder<TState, TEvent, TEventParameter>
            ConfigureElseTransition<TState, TEvent, TEventParameter>(
                this IStateBuilder<TState, TEvent, TEventParameter> stateBuilder,
                TransitionBase<TState, TEvent, TEventParameter> transition)
        {
            return stateBuilder.ElseSwitchTo(transition.Destination, transition.Effect);
        }
    }
}
