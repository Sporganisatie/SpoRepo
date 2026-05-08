import { useMemo } from "react";
import { Link, useLocation } from "react-router-dom";
import "./navbar.css";
import StatsDropdown from "./Dropdowns/StatistiekenDropdown";
import ChartsDropdown from "./Dropdowns/ChartsDropdown";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCircleInfo,
  faPersonBiking,
  faShieldAlt,
} from "@fortawesome/free-solid-svg-icons";
import { useBudgetContext, useBudgetDispatch } from "../shared/BudgetContextProvider";
import type { AuthToken } from "../../models/AuthToken";
import jwt_decode from "jwt-decode";
import Switch from "../ui/switch/Switch";
import { useRaceContext } from "../shared/RaceContextProvider";
import RaceDropdown from "./Dropdowns/RaceDropdown";

const Navbar = () => {
  const race = useRaceContext();
  const onTeamSelection = useLocation().pathname.endsWith("/teamselection");
  const budget = useBudgetContext();
  const dispatch = useBudgetDispatch();
  const claims = useMemo<AuthToken | null>(() => {
    const token = localStorage.getItem("authToken");
    return token ? jwt_decode<AuthToken>(token) : null;
  }, []);
  const isAdmin = claims?.admin === true;
  const showBudgetSwitch = claims != null && claims.id <= 5;
  return (
    <div>
      <div className="navbar">
        <Link className="navbar_link" to={"/"} title="Huidige etappe">
          <FontAwesomeIcon icon={faPersonBiking} className="nav-on-mobile" />
          <span className="nav-on-desktop">Huidige etappe</span>
        </Link>
        <ChartsDropdown raceSelected={race > 0} />
        <StatsDropdown raceSelected={race > 0} />
        {isAdmin && (
          <Link className="navbar_link" to="/admin">
            <FontAwesomeIcon icon={faShieldAlt} />
          </Link>
        )}
        {showBudgetSwitch && (
          <Switch
            value={budget}
            handleOnChange={() => dispatch({})}
            sliderContent="€"
            hotkey="B"
          />
        )}
        <RaceDropdown />
        <Link className="navbar_link navbar_link--end" to="/regelspunten" title="Regels/Punten">
          <FontAwesomeIcon icon={faCircleInfo} />
        </Link>
        {race > 0 && onTeamSelection && (
          <Link className="navbar-stage-cta" to={`/${race}/stage/1`}>
            Naar Etappe 1 →
          </Link>
        )}
      </div>
    </div>
  );
};

export default Navbar;
