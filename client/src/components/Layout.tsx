import { Outlet } from "react-router-dom";
import Navbar from "./navbar/Navbar";
import { BudgetStateProvider } from "./shared/BudgetContextProvider";

const Layout = () => {
  return (
    // Todo update navbar values
    <div className="content">
      <BudgetStateProvider>
        <Navbar isLoggedIn={true} isLoading={false} racename={"tour"} currentStageLink={"/"} />
        <div className="pageContainer">{<Outlet />}</div>
      </BudgetStateProvider>
    </div >
  );
};

export default Layout;
