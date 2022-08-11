// TODO change to TS, add model and type to the array
import { Route } from "react-router-dom";
import Home from "./components/Home";
import NewPage from "./components/NewPage";
import Testing from "./components/Testing";
import Login from "./pages/Login";

const Pages: JSX.Element[] = [
    <Route key="home" index element={<Home />} />,
    <Route key="newpage" path="newpage" element={<NewPage />} />,
    <Route key="login" path="login" element={<Login />} />,
    <Route key="testing" path="testing" element={<Testing />} />
];

export default Pages;
