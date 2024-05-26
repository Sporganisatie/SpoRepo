import { Dispatch, createContext, useContext, useReducer } from "react";

const RaceContext = createContext<number>(0);

const RaceDispatchContext = createContext<Dispatch<any> | undefined>(undefined);

function RaceReducer(_: any, race: number) {
    return race
}

export function useRaceContext() {
    return useContext(RaceContext);
}

export function useRaceDispatch(): Dispatch<number> {
    const context = useContext(RaceDispatchContext);
    if (context === undefined) {
        throw new Error('useRaceDispatch must be used within a RaceStateProvider');
    }
    return context;
}

export const RaceStateProvider = (props: { children: React.ReactNode }) => {
    const [race, dispatch] = useReducer(RaceReducer, 0);
    return (
        <RaceContext.Provider value={race}>
            <RaceDispatchContext.Provider value={dispatch}>
                {props.children}
            </RaceDispatchContext.Provider>
        </RaceContext.Provider>
    );
};