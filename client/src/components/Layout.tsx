import { Outlet } from "react-router-dom";
import Navbar from "./navbar/Navbar";

const Layout = () => {
  return (
    <div className="content">
      <Navbar isLoggedIn={true} isAdmin={true} isLoading={false} racename={"tour"} currentStageLink={"/"} />
      <div className="pageContainer">{<Outlet />}</div>
    </div>
  );
};

export default Layout;
