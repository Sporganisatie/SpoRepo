import { Outlet } from "react-router-dom";
import Navbar from "./navbar/Navbar";
import { BudgetStateProvider } from "./shared/BudgetContextProvider";
import { RaceStateProvider } from "./shared/RaceContextProvider";

const Layout = () => {
  return (
    <div className="content">
      <BudgetStateProvider>
        <RaceStateProvider>
          <Navbar />
          <div className="pageContainer">{<Outlet />}</div>
        </RaceStateProvider>
      </BudgetStateProvider>
    </div >
  );
};

export default Layout;
