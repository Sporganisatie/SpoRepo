import { Route } from "react-router-dom";
import Home from "./components/Home";
import DesignSandbox from "./pages/DesignSandbox";
import Admin from "./pages/Admin";
import UserProfile from "./pages/UserProfile";
import Login from "./pages/Login";
import Teamselection from "./pages/teamselection/Teamselection";
import RaceRouter from "./pages/RaceOverview/RaceRouter";
import JoinRace from "./pages/RaceOverview/JoinRace";
import Stage from "./pages/Stage/Stage";
import TeamComparisonPage from "./pages/Statistics/TeamComparisonPage";
import MissedPoints from "./pages/Statistics/MissedPoints";

const Pages: JSX.Element[] = [
    <Route key="home" index element={<Home />} />,
    <Route key="login" path="login" element={<Login />} />,
    <Route key="profile" path="profile" element={<UserProfile />} />,
    <Route key="designsandbox" path="designsandbox" element={<DesignSandbox />} />,
    <Route key="teamselection" path="/teamselection/:raceId" element={<Teamselection />} />,
    <Route key="raceRouter" path="race/:raceId" element={<RaceRouter />} />,
    <Route key="stage" path="stage/:raceId/:stagenr" element={<Stage />} />,
    <Route key="teamcomparison" path="teamcomparison/:raceId" element={<TeamComparisonPage />} />,
    <Route key="missedpoints" path="missedpoints/:raceId" element={<MissedPoints />} />,
    <Route key="joinRace" path="joinrace/:raceId" element={<JoinRace />} />,
    <Route key="admin" path="admin" element={<Admin />} />,
    // TODO zorg dat al het overige naar home gaat
];

export default Pages;
