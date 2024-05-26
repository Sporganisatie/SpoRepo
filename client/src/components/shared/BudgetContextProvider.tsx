import { Dispatch, createContext, useContext, useReducer } from "react";

const BudgetContext = createContext<boolean>(false);

const BudgetDispatchContext = createContext<Dispatch<any>>(BudgetReducer);

function BudgetReducer(budget: boolean) {
    return !budget
}

export function useBudgetContext() {
    return useContext(BudgetContext);
}

export function useBudgetDispatch() {
    return useContext(BudgetDispatchContext);
}

export const BudgetStateProvider = (props: { children: React.ReactNode }) => {
    const [budget, dispatch] = useReducer(BudgetReducer, false);
    return (
        <BudgetContext.Provider value={budget}>
            <BudgetDispatchContext.Provider value={dispatch}>
                {props.children}
            </BudgetDispatchContext.Provider>
        </BudgetContext.Provider>
    );
};