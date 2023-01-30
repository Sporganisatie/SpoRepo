import { Route } from "react-router-dom";
import Home from "./components/Home";
import DesignSandbox from "./pages/DesignSandbox";
import Testing from "./components/Testing";
import UserProfile from "./pages/UserProfile";
import Login from "./pages/Login";
import StageResultTemp from "./pages/StageResultTemp";
import Teamselection from "./pages/teamselection/Teamselection";

const Pages: JSX.Element[] = [
    <Route key="home" index element={<Home />} />,
    <Route key="login" path="login" element={<Login />} />,
    <Route key="profile" path="profile" element={<UserProfile />} />,
    <Route key="designsandbox" path="designsandbox" element={<DesignSandbox />} />,
    <Route key="teamselection" path=":raceid/teamselection" element={<Teamselection />} />,
    <Route key="stage" path="stage/:raceid/:stagenr" element={<StageResultTemp />} />,
    <Route key="testing" path="testing" element={<Testing />} />
    // TODO zorg dat al het overige naar home gaat
];

export default Pages;
