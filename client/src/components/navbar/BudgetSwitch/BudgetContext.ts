import { Dispatch, createContext } from 'react';

export function BudgetReducer(budget: boolean, action: any = null) {
    return !budget
}
export const BudgetContext = createContext<boolean>(false);
export const BudgetDispatchContext = createContext<Dispatch<any>>(BudgetReducer);
