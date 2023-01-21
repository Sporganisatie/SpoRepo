import { Route } from "react-router-dom";
import Home from "./components/Home";
import Testing from "./components/Testing";
import Login from "./pages/Login";
import StageResultTemp from "./pages/StageResultTemp";

const Pages: JSX.Element[] = [
    <Route key="home" index element={<Home />} />,
    <Route key="login" path="login" element={<Login />} />,
    <Route key="stage" path="stage/:raceid/:stagenr" element={<StageResultTemp />} />,
    <Route key="testing" path="testing" element={<Testing />} />
    // TODO zorg dat al het overige naar home gaat
];

export default Pages;
