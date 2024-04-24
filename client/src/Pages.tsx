import { createBrowserRouter } from "react-router-dom";
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
import Uitvallers from "./pages/Statistics/Uitvallers";
import EtappeUitslagen from "./pages/Statistics/EtappeUitslagen/EtappeUitslagen";
import AllRiders from "./pages/Statistics/AllRiders";
import ScoreVerloopChart from "./pages/Charts/ScoreVerloopChart";
import RaceUitslagen from "./pages/Statistics/EtappeUitslagen/RaceUitslagen";
import RiderPage from "./pages/RiderPage";
import ScoreVerdelingChart from "./pages/Charts/ScoreVerdelingChart";
import { Root } from "./App";
import Home from "./components/Home";

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
        Component: Teamselection,
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
      // charts
      {
        path: "charts/scoreverloop/:raceId",
        Component: ScoreVerloopChart,
      },
      {
        path: "charts/scoreverdeling/:raceId",
        Component: ScoreVerdelingChart,
      },
    ],
  },
]);

export default router;
