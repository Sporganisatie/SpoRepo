import { createBrowserRouter } from "react-router-dom";
import DesignSandbox from "./pages/DesignSandbox";
import Admin from "./pages/Admin";
import UserProfile from "./pages/UserProfile";
import Login from "./pages/Login";
import TeamSelection from "./pages/teamselection/Teamselection";
import RaceRouter from "./pages/RaceOverview/RaceRouter";
import JoinRace from "./pages/RaceOverview/JoinRace";
import Stage from "./pages/Stage/Stage";
import TeamComparisonPage from "./pages/Statistics/TeamComparisonPage";
import MissedPoints from "./pages/Statistics/MissedPoints";
import Uitvallers from "./pages/Statistics/Uitvallers";
import EtappeUitslagen from "./pages/Statistics/EtappeUitslagen/EtappeUitslagen";
import AllRiders from "./pages/Statistics/AllRiders";
import ScoreVerloopChart from "./pages/Charts/ScoreVerloopChart";
import RaceUitslagen from "./pages/Statistics/EtappeUitslagen/RaceUitslagen";
import RiderPage from "./pages/RiderPage/RiderPage";
import ScoreVerdelingChart from "./pages/Charts/ScoreVerdelingChart";
import RaceScoreVerloopChart from "./pages/Charts/RaceScoreVerloopChart";
import Klassementen from "./pages/Statistics/Klassementen";
import RulesPopup from "./pages/RulesPopup";
import { Root } from "./App";
import Home from "./components/Home";
import Uniekheid from "./pages/Statistics/Uniekheid";
import RaceScoreVerdelingChart from "./pages/Charts/RaceScoreVerdelingChart";
import Overlap from "./pages/Statistics/Overlap";
import RaceWrap from "./pages/RaceWrap/RaceWrap";

const router = createBrowserRouter([
    {
        path: "/",
        Component: Root,
        children: [
            {
                path: "/",
                Component: Home,
            },
            {
                path: "login",
                Component: Login,
            },
            {
                path: "profile",
                Component: UserProfile,
            },
            {
                path: "designsandbox",
                Component: DesignSandbox,
            },
            {
                path: ":raceId/teamselection",
                Component: TeamSelection,
            },
            {
                path: ":raceId/race",
                Component: RaceRouter,
            },
            {
                path: ":raceId/stage/:stagenr",
                Component: Stage,
            },
            {
                path: ":raceId/racewrap",
                Component: RaceWrap,
            },
            {
                path: ":raceId/joinrace",
                Component: JoinRace,
            },
            {
                path: "rider/:riderId",
                Component: RiderPage,
            },
            {
                path: "admin",
                Component: Admin,
            },
            {
                path: "regelspunten",
                Component: RulesPopup,
            },
            // statistics
            {
                path: ":raceId/teamcomparison",
                Component: TeamComparisonPage,
            },
            {
                path: ":raceId/missedpoints",
                Component: MissedPoints,
            },
            {
                path: ":raceId/uitvallers",
                Component: Uitvallers,
            },
            {
                path: ":raceId/etappeUitslagen",
                Component: EtappeUitslagen,
            },
            {
                path: "raceUitslagen/:raceName?",
                Component: RaceUitslagen,
            },
            {
                path: ":raceId/allRiders",
                Component: AllRiders,
            },
            {
                path: ":raceId/klassementen",
                Component: Klassementen,
            },
            {
                path: ":raceId/teamoverlap",
                Component: Overlap,
            },
            {
                path: ":raceId/uniekheid",
                Component: Uniekheid,
            },
            // charts
            {
                path: "charts/scoreverloop",
                Component: RaceScoreVerloopChart,
            },
            {
                path: ":raceId/charts/scoreverloop",
                Component: ScoreVerloopChart,
            },
            {
                path: "charts/scoreverdeling",
                Component: RaceScoreVerdelingChart,
            },
            {
                path: ":raceId/charts/scoreverdeling",
                Component: ScoreVerdelingChart,
            },
        ],
    },
]);

export default router;
