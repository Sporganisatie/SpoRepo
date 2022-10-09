import { Route } from "react-router-dom";
import Home from "./components/Home";
import DesignSandbox from "./components/DesignSandbox";
import Testing from "./components/Testing";
import UserProfile from "./components/UserProfile";
import Login from "./pages/Login";
import StageResultTest from "./pages/StageResultTest";

const Pages: JSX.Element[] = [
    <Route key="home" index element={<Home />} />,
    <Route key="login" path="login" element={<Login />} />,
    <Route key="profile" path="profile" element={<UserProfile />} />,
    <Route key="designsandbox" path="designsandbox" element={<DesignSandbox />} />,
    <Route key="stage" path="stage/:raceid/:stagenr" element={<StageResultTest />} />,
    <Route key="testing" path="testing" element={<Testing />} />
    // TODO zorg dat al het overige naar home gaat
];

export default Pages;
