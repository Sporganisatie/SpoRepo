import { Link } from "react-router-dom";
import "./navbar.css";
import StatsDropdown from "./Dropdowns/StatistiekenDropdown";
import ChartsDropdown from "./Dropdowns/ChartsDropdown";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faLaptop, faShieldAlt } from "@fortawesome/free-solid-svg-icons";
import { useBudgetContext, useBudgetDispatch } from "../shared/BudgetContextProvider";
import type { AuthToken } from "../../models/AuthToken";
import jwt_decode from "jwt-decode";
import Switch from "../ui/switch/Switch";
import { useRaceContext } from "../shared/RaceContextProvider";
import RaceDropdown from "./Dropdowns/RaceDropdown";

const Navbar = () => {
  const race = useRaceContext();
  const budget = useBudgetContext();
  const dispatch = useBudgetDispatch();
  const isAdmin =
    localStorage.getItem("authToken") &&
    jwt_decode<AuthToken>(localStorage.getItem("authToken") ?? "").admin === true;
  return (
    <div>
      <div className="navbar">
        <Link className="navbar_link" to={"/"}>
          <span>Current stage</span>
        </Link>
        <ChartsDropdown raceSelected={race > 0} />
        <StatsDropdown raceSelected={race > 0} />
        <Link className="navbar_link" to="/regelspunten">
          <span>Regels/Punten</span>
        </Link>
        {isAdmin && (
          <Link className="navbar_link" to="/admin">
            <FontAwesomeIcon icon={faShieldAlt} />
          </Link>
        )}
        {isAdmin && (
          <Link className="navbar_link" to="/designsandbox">
            <FontAwesomeIcon icon={faLaptop} />
          </Link>
        )}
        {localStorage.getItem("authToken") &&
          jwt_decode<AuthToken>(localStorage.getItem("authToken") ?? "").id <= 5 && (
            <Switch
              value={budget}
              handleOnChange={() => dispatch({})}
              sliderContent="€"
              hotkey="B"
            />
          )}
        <RaceDropdown />
      </div>
    </div>
  );
};

export default Navbar;
