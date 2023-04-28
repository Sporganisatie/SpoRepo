import { Dispatch, createContext, useReducer } from "react";

export function BudgetReducer(budget: boolean) {
    return !budget
}

export const BudgetContext = createContext<boolean>(false);
export const BudgetDispatchContext = createContext<Dispatch<any>>(BudgetReducer);

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