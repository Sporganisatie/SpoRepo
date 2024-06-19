import { createBrowserRouter } from "react-router-dom";
import DesignSandbox from "./pages/DesignSandbox";
import Admin from "./pages/Admin";
import UserProfile from "./pages/UserProfile";
import Login from "./pages/Login";
import TeamSelection from "./pages/teamselection/TeamSelection";
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
import TeamOverlap from "./pages/Statistics/TeamOverlap";
import RaceScoreVerdelingChart from "./pages/Charts/RaceScoreVerdelingChart";

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
        path: "teamselection/:raceId",
        Component: TeamSelection,
      },
      {
        path: "race/:raceId",
        Component: RaceRouter,
      },
      {
        path: "stage/:raceId/:stagenr",
        Component: Stage,
      },
      {
        path: "joinrace/:raceId",
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
        path: "teamcomparison/:raceId",
        Component: TeamComparisonPage,
      },
      {
        path: "missedpoints/:raceId",
        Component: MissedPoints,
      },
      {
        path: "uitvallers/:raceId",
        Component: Uitvallers,
      },
      {
        path: "etappeUitslagen/:raceId",
        Component: EtappeUitslagen,
      },
      {
        path: "raceUitslagen",
        Component: RaceUitslagen,
      },
      {
        path: "allRiders/:raceId",
        Component: AllRiders,
      },
      {
        path: "klassementen/:raceId",
        Component: Klassementen,
      },
      {
        path: "teamoverlap/:raceId",
        Component: TeamOverlap,
      },
      // charts
      {
        path: "charts/scoreverloop",
        Component: RaceScoreVerloopChart,
      },
      {
        path: "charts/scoreverloop/:raceId",
        Component: ScoreVerloopChart,
      },
      {
        path: "charts/scoreverdeling",
        Component: RaceScoreVerdelingChart,
      },
      {
        path: "charts/scoreverdeling/:raceId",
        Component: ScoreVerdelingChart,
      },
    ],
  },
]);

export default router;
