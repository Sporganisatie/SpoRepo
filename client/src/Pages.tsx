import { Route } from "react-router-dom";
import Home from "./components/Home";
import DesignSandbox from "./pages/DesignSandbox";
import Testing from "./components/Testing";
import UserProfile from "./pages/UserProfile";
import Login from "./pages/Login";
import Teamselection from "./pages/teamselection/Teamselection";
import RaceRouter from "./pages/RaceOverview/RaceRouter";
import JoinRace from "./pages/RaceOverview/JoinRace";
import Stage from "./pages/Stage/Stage";

const Pages: JSX.Element[] = [
    <Route key="home" index element={<Home />} />,
    <Route key="login" path="login" element={<Login />} />,
    <Route key="profile" path="profile" element={<UserProfile />} />,
    <Route key="designsandbox" path="designsandbox" element={<DesignSandbox />} />,
    <Route key="teamselection" path="/teamselection/:raceId" element={<Teamselection />} />,
    <Route key="raceRouter" path="race/:raceId" element={<RaceRouter />} />,
    <Route key="stage" path="stage/:raceId/:stagenr" element={<Stage />} />,
    <Route key="joinRace" path="joinrace/:raceId" element={<JoinRace />} />,
    <Route key="testing" path="testing" element={<Testing />} />
    // TODO zorg dat al het overige naar home gaat
];

export default Pages;
