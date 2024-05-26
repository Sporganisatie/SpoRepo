import { Outlet } from "react-router-dom";
import Navbar from "./navbar/Navbar";
import { BudgetStateProvider } from "./shared/BudgetContextProvider";

const Layout = () => {
  return (
    <div className="content">
      <BudgetStateProvider>
        <Navbar />
        <div className="pageContainer">{<Outlet />}</div>
      </BudgetStateProvider>
    </div >
  );
};

export default Layout;
