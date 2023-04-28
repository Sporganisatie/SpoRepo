import { Outlet } from "react-router-dom";
import Navbar from "./navbar/Navbar";
import { useReducer } from "react";
import { BudgetContext, BudgetDispatchContext, BudgetReducer } from "./navbar/BudgetSwitch/BudgetContext";

const Layout = () => {
  const [budget, dispatch] = useReducer(BudgetReducer, false);
  return (
    <div className="content">
      <BudgetContext.Provider value={budget}>
        <BudgetDispatchContext.Provider value={dispatch}>
          <Navbar isLoggedIn={true} isAdmin={true} isLoading={false} racename={"tour"} currentStageLink={"/"} />
          <div className="pageContainer">{<Outlet />}</div>
        </BudgetDispatchContext.Provider>
      </BudgetContext.Provider>
    </div >
  );
};

export default Layout;
